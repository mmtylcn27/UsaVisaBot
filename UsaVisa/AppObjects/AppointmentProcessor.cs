using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using UsaVisa.UsaVisaObjects;

namespace UsaVisa.AppObjects
{
    public class AppointmentProcessor
    {
        public event Action<DateTime> FoundMinDateEvent;
        public event Action<DateTime> ReAppointmentEvent;
        
        private readonly string _mail, _password;
        private readonly DateTime _minDate, _maxDate;
        private readonly int _scheduleId, _facilityId;
        private readonly bool _reAppointment;
        private readonly ProxyInfo _proxyInfo;
        private readonly Action<string> _log, _errorLog;

        public AppointmentProcessor(string mail, string password, DateTime minDate, DateTime maxDate, int scheduleId, int facilityId, bool reAppointment, ProxyInfo proxyInfo, Action<string> log, Action<string> errorLog)
        {
            _mail = mail;
            _password = password;
            _minDate = minDate;
            _maxDate = maxDate;
            _scheduleId = scheduleId;
            _facilityId = facilityId;
            _reAppointment = reAppointment;
            _proxyInfo = proxyInfo;
            _log = log;
            _errorLog = errorLog;
        }

        public Task<(DateTime date, string time)> CheckAppointment(TimeSpan reCheckTime, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                CookieInfo? cookie = null, token = null;
                var isLogin = false;
                var url = string.Empty;
                var starTime = DateTime.Now;
                var lastMinDate = DateTime.MaxValue;
                var currentAppointmentDate = DateTime.MaxValue;
                
                for(;; _log($@"wait {reCheckTime}"), await Task.Delay(reCheckTime, cancellationToken).ConfigureAwait(false))
                {
                    if (!isLogin)
                    {
                        _log(@"Login start");

                        var getLoginPage = await GetLoginPage(_proxyInfo, _errorLog).ConfigureAwait(false);

                        if (getLoginPage.Equals(default) || !getLoginPage.cookie.HasValue || !getLoginPage.token.HasValue)
                        {
                            _errorLog($@"getLoginPage: {getLoginPage}");
                            continue;
                        }

                        var getLogin = await GetLogin(_proxyInfo, getLoginPage.cookie.Value, getLoginPage.token.Value, _mail, _password, _errorLog).ConfigureAwait(false);

                        if (!getLogin.HasValue)
                        {
                            _errorLog($@"getLogin: {getLogin}");
                            continue;
                        }

                        var getAccountPage = await GetAccountPage(_proxyInfo, getLogin.Value, _errorLog).ConfigureAwait(false);

                        if (getAccountPage.Equals(default) || !getAccountPage.cookie.HasValue || string.IsNullOrEmpty(getAccountPage.url))
                        {
                            _errorLog($@"getAccountPage: {getAccountPage}");
                            continue;
                        }

                        var getContinueActionsPage = await GetContinueActionsPage(_proxyInfo, getAccountPage.cookie.Value, _scheduleId, getAccountPage.url, _errorLog).ConfigureAwait(false);

                        if (getContinueActionsPage.Equals(default) || !getContinueActionsPage.cookie.HasValue || string.IsNullOrEmpty(getContinueActionsPage.url))
                        {
                            _errorLog($@"getContinueActionsPage: {getContinueActionsPage}");
                            continue;
                        }

                        var getAppointmentPage = await GetAppointmentPage(_proxyInfo, getContinueActionsPage.cookie.Value, _scheduleId, getContinueActionsPage.url, _errorLog).ConfigureAwait(false);

                        if (getAppointmentPage.Equals(default) || !getAppointmentPage.cookie.HasValue || !getAppointmentPage.token.HasValue || string.IsNullOrEmpty(getAppointmentPage.url))
                        {
                            _errorLog($@"getAppointmentPage: {getAppointmentPage}");
                            continue;
                        }

                        cookie = getAppointmentPage.cookie.Value;
                        token = getAppointmentPage.token.Value;
                        url = getAppointmentPage.url;
                        currentAppointmentDate = getAccountPage.currentAppointmentDate;
                        isLogin = true;

                        _log(@"Login success");
                        ReAppointmentEvent?.Invoke(currentAppointmentDate);
                    }

                    var getDate = await GetDate(_proxyInfo, cookie.Value, token.Value, _scheduleId, _facilityId, url, _errorLog).ConfigureAwait(false);

                    if (getDate.Equals(default) || !getDate.cookie.HasValue)
                    {
                        isLogin = false;

                        _errorLog($@"getDate: {getDate}");
                        continue;
                    }

                    if (getDate.openDays is null)
                    {
                        isLogin = false;

                        _errorLog(@"getDate.openDays is null");
                        continue;
                    }

                    foreach (var day in getDate.openDays)
                    {
                        _log($@"Open date: {day.Date.ToShortDateString()}");

                        if (day.Date >= lastMinDate)
                            continue;

                        lastMinDate = day.Date;
                        FoundMinDateEvent?.Invoke(lastMinDate);
                    }
                    
                    cookie = getDate.cookie.Value;

                    var filterDate = getDate.openDays.Where(x => x.Date.Date >= _minDate.Date && x.Date.Date <= _maxDate.Date).Select(x => x.Date).OrderBy(x => x.Date);

                    foreach (var date in filterDate)
                    {
                        _log($@"Check appointment date: {date.ToShortDateString()} from time");

                        var getTime = await GetTime(_proxyInfo, cookie.Value, token.Value, date, _scheduleId, _facilityId, url, _errorLog).ConfigureAwait(false);

                        if (getTime.Equals(default) || !getTime.cookie.HasValue)
                        {
                            _errorLog($@"getTime: {getTime}");
                            continue;
                        }

                        cookie = getTime.cookie.Value;

                        if (getTime.openTimes is null)
                        {
                            _errorLog(@"getTime.openTimes is null");
                            continue;
                        }

                        foreach (var time in getTime.openTimes.AvailableTimes)
                        {
                            _log($@"Start appointment date: {date.ToShortDateString()} time: {time}");

                            var (getAppointmentSuccess, getAppointmentContent) = await GetAppointment(_proxyInfo, cookie.Value, token.Value, _scheduleId, _facilityId, date, time, _errorLog).ConfigureAwait(false);

                            if (getAppointmentSuccess)
                            {
                                _log($@"Success appointment date: {date.ToShortDateString()} time: {time}");
                                _log($@"Appointment content: {getAppointmentContent}");

                                return (date, time);
                            }

                            _log($@"Failed appointment date: {date.ToShortDateString()} time: {time}");
                        }
                    }

                    if (_reAppointment)
                    {
                        var currAppDate = currentAppointmentDate;

                        _log($@"[_reAppointment] -> Current appointment date: {currAppDate.ToShortDateString()} time: {currAppDate.ToShortTimeString()}");
                        filterDate = getDate.openDays.Where(x => x.Date.Date < currAppDate.Date).Select(x => x.Date).OrderBy(x => x.Date);

                        foreach (var date in filterDate)
                        {
                            _log($@"[_reAppointment] -> Check appointment date: {date.ToShortDateString()} from time");

                            var getTime = await GetTime(_proxyInfo, cookie.Value, token.Value, date, _scheduleId, _facilityId, url, _errorLog).ConfigureAwait(false);

                            if (getTime.Equals(default) || !getTime.cookie.HasValue)
                            {
                                _errorLog($@"[_reAppointment] -> getTime: {getTime}");
                                continue;
                            }

                            cookie = getTime.cookie.Value;

                            if (getTime.openTimes is null)
                            {
                                _errorLog(@"[_reAppointment] -> getTime.openTimes is null");
                                continue;
                            }

                            var isAppointment = false;

                            foreach (var time in getTime.openTimes.AvailableTimes)
                            {
                                _log($@"[_reAppointment] -> Start appointment date: {date.ToShortDateString()} time: {time}");

                                var (getAppointmentSuccess, getAppointmentContent) = await GetAppointment(_proxyInfo, cookie.Value, token.Value, _scheduleId, _facilityId, date, time, _errorLog).ConfigureAwait(false);

                                if (getAppointmentSuccess)
                                {
                                    _log($@"[_reAppointment] -> Success appointment date: {date.ToShortDateString()} time: {time}");
                                    _log($@"Appointment content: {getAppointmentContent}");

                                    isLogin = false;
                                    isAppointment = true;
                                    break;
                                }

                                _log($@"[_reAppointment] -> Failed appointment date: {date.ToShortDateString()} time: {time}");
                            }

                            if (isAppointment)
                                break;
                        }
                    }

                    _log($@"Found min date: {lastMinDate.ToShortDateString()}");
                    _log($@"Running time: {DateTime.Now - starTime}");
                }
            }, cancellationToken);
        }

        private const string Host = "ais.usvisa-info.com";
        private const string BaseUrl = "https://" + Host;
        private const string SignUrl = BaseUrl + "/tr-tr/niv/users/sign_in";
        private const string AccountUrl = BaseUrl + "/tr-tr/niv/account";
        private const string ScheduleUrl = BaseUrl + "/tr-tr/niv/schedule";

        private static RestClient CreateClient(ProxyInfo proxyInfo)
        {
            var options = new RestClientOptions
            {
                Expect100Continue = false, 
                Encoding = Encoding.UTF8,
                MaxTimeout = 60000,
                CachePolicy = new CacheControlHeaderValue { NoCache = true },
                Proxy = proxyInfo is null ? null : new WebProxy
                {
                    Address = new Uri($"http://{proxyInfo.ProxyIp}:{proxyInfo.ProxyPort}"),
                    Credentials = string.IsNullOrEmpty(proxyInfo.ProxyUserId) ? null : new NetworkCredential(proxyInfo.ProxyUserId, proxyInfo.ProxyUserPw)
                }
            };

            var restClient = new RestClient(options);
            
            restClient.AddDefaultHeader("Accept-Encoding", "gzip, deflate");
            restClient.AddDefaultHeader("Accept-Language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7,zh-CN;q=0.6,zh-TW;q=0.5,zh;q=0.4");
            restClient.AddDefaultHeader("Connection", "keep-alive");
            restClient.AddDefaultHeader("Host", Host);
            restClient.AddDefaultHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");

            return restClient;
        }

        private static CookieInfo? ExtractCookie(IEnumerable<HeaderParameter> headers)
        {
            try
            {
                if (headers is null)
                    return null;

                foreach (var header in headers)
                {
                    if (string.IsNullOrEmpty(header.Name))
                        continue;

                    if (header.Name != "Set-Cookie")
                        continue;

                    if (!(header.Value is string))
                        continue;

                    var splitValue = header.Value.ToString().Split('=');

                    if (splitValue.Length < 2)
                        continue;

                    var splitCookieValue = splitValue[1].Split(';');

                    if (splitCookieValue.Length < 1)
                        continue;

                    return new CookieInfo(splitValue[0], splitCookieValue[0]);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static CookieInfo? ExtractToken(string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                    return null;

                var htmlAgility = new HtmlAgilityPack.HtmlDocument();
                htmlAgility.LoadHtml(content);

                var csrfElement = htmlAgility.DocumentNode.SelectSingleNode("//meta[@name='csrf-token']");

                if (csrfElement is null)
                    return null;

                return new CookieInfo("X-CSRF-Token", csrfElement.GetAttributeValue("content", string.Empty));
            }
            catch
            {
                return null;
            }
        }

        private static string GroupUrl(string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                    return string.Empty;

                var htmlAgility = new HtmlAgilityPack.HtmlDocument();
                htmlAgility.LoadHtml(content);

                var continueElement = htmlAgility.DocumentNode.SelectSingleNode("//a[@href[contains(., '/tr-tr/niv/groups')]]");

                return continueElement is null ? string.Empty : $"{BaseUrl}{continueElement.GetAttributeValue("href", string.Empty)}";
            }
            catch
            {
                return string.Empty;
            }
        }

        private static DateTime UserAppointmentDate(string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                    return DateTime.MinValue;

                var htmlAgility = new HtmlAgilityPack.HtmlDocument();
                htmlAgility.LoadHtml(content);

               var appointmentElement = htmlAgility.DocumentNode.SelectSingleNode("//p[@class='consular-appt']");

               if (appointmentElement is null)
                   return DateTime.MinValue;

               var regex = new Regex(@"\d{1,2}\s*\S*\w*\W*\d{4}\s*\S*\w*\W*\S*");
               var match = regex.Match(appointmentElement.InnerText);

               if (!match.Success)
                   return DateTime.MinValue;

               return DateTime.TryParse(match.Value, out var appointmentDate) ? appointmentDate : DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }


        private static async Task<(CookieInfo? cookie, CookieInfo? token)> GetLoginPage(ProxyInfo proxyInfo, Action<string> errorLog)
        {
            try
            {
                using (var client = CreateClient(proxyInfo))
                {
                    client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");

                    var getLoginPage = await client.ExecuteAsync(new RestRequest(SignUrl), Method.Get).ConfigureAwait(false);

                    if (!getLoginPage.IsSuccessful)
                    {
                        errorLog($@"GetLoginPage request failed. Error: {getLoginPage.ErrorMessage}");
                        return default;
                    }

                    var getCookie = ExtractCookie(getLoginPage.Headers);
                    var getToken = ExtractToken(getLoginPage.Content);

                    return (getCookie, getToken);
                }
            }
            catch (Exception ex)
            {
                errorLog($@"GetLoginPage exception: {ex.Message}");
                return default;
            }
        }

        private static async Task<CookieInfo?> GetLogin(ProxyInfo proxyInfo, CookieInfo cookie, CookieInfo token, string mail, string password, Action<string> errorLog)
        {
            try
            {
                using (var client = CreateClient(proxyInfo))
                {
                    client.AddDefaultHeader("Accept", "*/*;q=0.5, text/javascript, application/javascript, application/ecmascript, application/x-ecmascript");
                    client.AddDefaultHeader("Origin", BaseUrl);
                    client.AddDefaultHeader("Referer", SignUrl);
                    client.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    client.AddDefaultHeader("Cookie", $"{cookie.Name}={cookie.Value}");
                    client.AddDefaultHeader(token.Name, token.Value);
                    client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");

                    var request = new RestRequest(SignUrl, Method.Post);

                    request.AddParameter("user[email]", mail, ParameterType.GetOrPost);
                    request.AddParameter("user[password]", password, ParameterType.GetOrPost);
                    request.AddParameter("policy_confirmed", "1", ParameterType.GetOrPost);
                    request.AddParameter("commit", HttpUtility.UrlEncode("Oturum Aç", Encoding.UTF8), ParameterType.GetOrPost);

                    var getLogin = await client.ExecuteAsync(request, Method.Post).ConfigureAwait(false);

                    if (!(getLogin.IsSuccessful))
                    {
                        errorLog($@"GetLogin request failed. Error: {getLogin.ErrorMessage}");
                        return null;
                    }

                    if (!string.IsNullOrEmpty(getLogin.Content))
                    {
                        if (getLogin.Content.Contains("account"))
                            return ExtractCookie(getLogin.Headers);

                        errorLog($@"GetLogin content 'account' not found content: {getLogin.Content}");
                        return null;
                    }

                    errorLog("GetLogin content is null or empty");
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorLog($@"GetLogin exception: {ex.Message}");
                return null;
            }
        }

        private static async Task<(CookieInfo? cookie, string url, DateTime currentAppointmentDate)> GetAccountPage(ProxyInfo proxyInfo, CookieInfo cookie, Action<string> errorLog)
        {
            try
            {
                using (var client = CreateClient(proxyInfo))
                {
                    client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    client.AddDefaultHeader("Origin", BaseUrl);
                    client.AddDefaultHeader("Referer", SignUrl);
                    client.AddDefaultHeader("Cookie", $"{cookie.Name}={cookie.Value}");

                    var getAccountPage = await client.ExecuteAsync(new RestRequest(AccountUrl), Method.Get).ConfigureAwait(false);

                    if (!getAccountPage.IsSuccessful)
                    {
                        errorLog($@"GetAccountPage request failed. Error: {getAccountPage.ErrorMessage}");
                        return default;
                    }

                    var getCookie = ExtractCookie(getAccountPage.Headers);
                    var url = GroupUrl(getAccountPage.Content);
                    var currentAppointmentDate = UserAppointmentDate(getAccountPage.Content);

                    return (getCookie, url, currentAppointmentDate);
                }
            } 
            catch (Exception ex)
            {
                errorLog($@"GetAccountPage exception: {ex.Message}");
                return default;
            }
        }

        private static async Task<(CookieInfo? cookie, string url)> GetContinueActionsPage(ProxyInfo proxyInfo, CookieInfo cookie, int scheduleId, string refererUrl, Action<string> errorLog)
        {
            try
            {
                using (var client = CreateClient(proxyInfo))
                {
                    client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    client.AddDefaultHeader("Referer", refererUrl);
                    client.AddDefaultHeader("Cookie", $"{cookie.Name}={cookie.Value}");

                    var url = $"{ScheduleUrl}/{scheduleId}/continue_actions";
                    var getContinueAccountPage = await client.ExecuteAsync(new RestRequest(url), Method.Get).ConfigureAwait(false);

                    if (!getContinueAccountPage.IsSuccessful)
                    {
                        errorLog($@"GetContinueActionsPage request failed. Error: {getContinueAccountPage.ErrorMessage}");
                        return default;
                    }

                    var getCookie = ExtractCookie(getContinueAccountPage.Headers);

                    return (getCookie, url);
                }
            }
            catch (Exception ex)
            {
                errorLog($@"GetContinueActionsPage exception: {ex.Message}");
                return default;
            }
        }

        private static async Task<(CookieInfo? cookie, CookieInfo? token, string url)> GetAppointmentPage(ProxyInfo proxyInfo, CookieInfo cookie, int scheduleId, string refererUrl, Action<string> errorLog)
        {
            try
            {
                using (var client = CreateClient(proxyInfo))
                {
                    client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    client.AddDefaultHeader("Referer", refererUrl);
                    client.AddDefaultHeader("Cookie", $"{cookie.Name}={cookie.Value}");

                    var url = $"{ScheduleUrl}/{scheduleId}/appointment";
                    var getAppointmentPage = await client.ExecuteAsync(new RestRequest(url), Method.Get).ConfigureAwait(false);

                    if (!getAppointmentPage.IsSuccessful)
                    {
                        errorLog($@"GetAppointmentPage request failed. Error: {getAppointmentPage.ErrorMessage}");
                        return default;
                    }

                    var getCookie = ExtractCookie(getAppointmentPage.Headers);
                    var getToken = ExtractToken(getAppointmentPage.Content);

                    return (getCookie, getToken, url);
                }
            }
            catch (Exception ex)
            {
                errorLog($@"GetAppointmentPage exception: {ex.Message}");
                return default;
            }
        }

        private static async Task<(List<OpenDay> openDays, CookieInfo? cookie)> GetDate(ProxyInfo proxyInfo, CookieInfo cookie, CookieInfo token, int scheduleId, int facilityId, string refererUrl, Action<string> errorLog)
        {
            try
            {
                using (var client = CreateClient(proxyInfo))
                {
                    client.AddDefaultHeader("Accept", "application/json, text/javascript, */*; q=0.01");
                    client.AddDefaultHeader("Referer", refererUrl);
                    client.AddDefaultHeader("Cookie", $"{cookie.Name}={cookie.Value}");
                    client.AddDefaultHeader(token.Name, token.Value);
                    client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");

                    var request = new RestRequest($"{ScheduleUrl}/{scheduleId}/appointment/days/{facilityId}.json?appointments[expedite]=false");
                    var getDate = await client.ExecuteAsync(request, Method.Get).ConfigureAwait(false);

                    if (!(getDate.IsSuccessful))
                    {
                        errorLog($@"GetDate request failed. Error: {getDate.ErrorMessage}");
                        return default;
                    }

                    if (string.IsNullOrEmpty(getDate.Content))
                    {
                        errorLog("GetDate content is null or empty");
                        return default;
                    }

                    var getCookie = ExtractCookie(getDate.Headers);
                    List<OpenDay> openDays;

                    try
                    {
                        openDays = JsonConvert.DeserializeObject<List<OpenDay>>(getDate.Content);

                        if (openDays is null)
                            errorLog($@"GetDate deserialize error content: {getDate.Content}");
                    }
                    catch (Exception ex)
                    {
                        errorLog($@"GetDate deserialize exception: {ex.Message}");
                        openDays = null;
                    }

                    return (openDays, getCookie);
                }
            }
            catch (Exception ex)
            {
                errorLog($@"GetDate exception: {ex.Message}");
                return default;
            }
        }

        private static async Task<(OpenTime openTimes, CookieInfo? cookie)> GetTime(ProxyInfo proxyInfo, CookieInfo cookie, CookieInfo token, DateTime date, int scheduleId, int facilityId, string refererUrl, Action<string> errorLog)
        {
            try
            {
                using (var client = CreateClient(proxyInfo))
                {
                    client.AddDefaultHeader("Accept", "application/json, text/javascript, */*; q=0.01");
                    client.AddDefaultHeader("Referer", refererUrl);
                    client.AddDefaultHeader("Cookie", $"{cookie.Name}={cookie.Value}");
                    client.AddDefaultHeader(token.Name, token.Value);
                    client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");

                    var request = new RestRequest($"{ScheduleUrl}/{scheduleId}/appointment/times/{facilityId}.json?date={date:yyyy-MM-dd}&appointments[expedite]=false");
                    var getTime = await client.ExecuteAsync(request, Method.Get).ConfigureAwait(false);

                    if (!(getTime.IsSuccessful))
                    {
                        errorLog($@"GetTime request failed. Error: {getTime.ErrorMessage}");
                        return default;
                    }

                    if (string.IsNullOrEmpty(getTime.Content))
                    {
                        errorLog("GetTime content is null or empty");
                        return default;
                    }

                    var getCookie = ExtractCookie(getTime.Headers);
                    OpenTime openTimes;

                    try
                    {
                        openTimes = JsonConvert.DeserializeObject<OpenTime>(getTime.Content);

                        if (openTimes is null)
                            errorLog($@"GetTime deserialize error content: {getTime.Content}");
                    }
                    catch (Exception ex)
                    {
                        errorLog($@"GetTime deserialize exception: {ex.Message}");
                        openTimes = null;
                    }

                    return (openTimes, getCookie);
                }
            }
            catch (Exception ex)
            {
                errorLog($@"GetTime exception: {ex.Message}");
                return default;
            }
        }

        private static async Task<(bool success, string pageContent)> GetAppointment(ProxyInfo proxyInfo, CookieInfo cookie, CookieInfo token, int scheduleId, int facilityId, DateTime date, string time, Action<string> errorLog)
        {
            try
            {
                using (var client = CreateClient(proxyInfo))
                {
                    var url = $"{ScheduleUrl}/{scheduleId}/appointment";

                    client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    client.AddDefaultHeader("Origin", BaseUrl);
                    client.AddDefaultHeader("Referer", url);
                    client.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded");
                    client.AddDefaultHeader("Cookie", $"{cookie.Name}={cookie.Value}");

                    var request = new RestRequest(url, Method.Post);

                    request.AddParameter("authenticity_token", token.Value, ParameterType.GetOrPost);
                    request.AddParameter("confirmed_limit_message", "1", ParameterType.GetOrPost);
                    request.AddParameter("use_consulate_appointment_capacity", "true", ParameterType.GetOrPost);
                    request.AddParameter("appointments[consulate_appointment][facility_id]", facilityId, ParameterType.GetOrPost);
                    request.AddParameter("appointments[consulate_appointment][date]", date.ToString("yyyy-MM-dd"), ParameterType.GetOrPost);
                    request.AddParameter("appointments[consulate_appointment][time]", time, ParameterType.GetOrPost);

                    var getAppointment = await client.ExecuteAsync(request, Method.Post).ConfigureAwait(false);

                    if (!(getAppointment.IsSuccessful))
                    {
                        errorLog($@"GetAppointment request failed. Error: {getAppointment.ErrorMessage}");
                        return (false, string.Empty);
                    }

                    if (!string.IsNullOrEmpty(getAppointment.Content))
                        return (true, getAppointment.Content);

                    errorLog("GetAppointment content is null or empty");
                    return (false, string.Empty);
                }
            }
            catch (Exception ex)
            {
                errorLog($@"GetAppointment exception: {ex.Message}");
                return (false, string.Empty);
            }
        }
    }
}
