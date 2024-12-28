namespace UsaVisa.AppObjects
{
    public class ProxyInfo
    {
        public readonly string ProxyIp, ProxyPort, ProxyUserId, ProxyUserPw;

        public ProxyInfo(string proxyIp, string proxyPort, string proxyUserId, string proxyUserPw)
        {
            ProxyIp = proxyIp;
            ProxyPort = proxyPort;
            ProxyUserId = proxyUserId;
            ProxyUserPw = proxyUserPw;
        }
    }
}
