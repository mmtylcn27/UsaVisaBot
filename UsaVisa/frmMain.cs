using System;
using System.Threading;
using System.Windows.Forms;
using UsaVisa.AppObjects;

namespace UsaVisa
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource _cancellationTokenSource;
        private DateTime _lastMinDate, _lastReAppointmentDate;

        public MainForm()
        {
            InitializeComponent();
            _lastMinDate = _lastReAppointmentDate = DateTime.MinValue;
        }

        private void Log(string message)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                txtLog.AppendText($"[{DateTime.Now.ToLongTimeString()}]: {message}{Environment.NewLine}");
            });
        }

        private void ErrorLog(string message)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                txtErrorLog.AppendText($"[{DateTime.Now.ToLongTimeString()}]: {message}{Environment.NewLine}");
            });
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            var mail = txtMail.Text;
            var password = txtPassword.Text;
            var scheduleId = Convert.ToInt32(txtScheduleId.Text);
            var facilityId = Convert.ToInt32(txtFacilityId.Text);
            var minDate = dtMinDate.Value.Date;
            var maxDate = dtMaxDate.Value.Date;
            var reCheckTime = TimeSpan.FromMilliseconds(Convert.ToInt32(txtWaitMs.Text));
            var reAppointment = chReAppointment.Checked;

            var proxyInfo = string.IsNullOrEmpty(txtProxyIp.Text) ? null : new ProxyInfo(txtProxyIp.Text, txtProxyPort.Text, txtProxyUserId.Text, txtProxyUserPw.Text);

            try
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                _cancellationTokenSource = new CancellationTokenSource();

                var processor = new AppointmentProcessor(mail, password, minDate, maxDate, scheduleId, facilityId, reAppointment, proxyInfo, Log, ErrorLog);
               
                processor.FoundMinDateEvent += dateTime =>
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        _lastMinDate = dateTime;
                        Text = $@"MinDate: Date[{_lastMinDate.ToShortDateString()}] Time[{_lastMinDate.ToShortTimeString()}] CurrentDate: Date[{_lastReAppointmentDate.ToShortDateString()}] Time[{_lastReAppointmentDate.ToShortTimeString()}]";
                    });
                };
                
                processor.ReAppointmentEvent += dateTime =>
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        _lastReAppointmentDate = dateTime;
                        Text = $@"MinDate: Date[{_lastMinDate.ToShortDateString()}] Time[{_lastMinDate.ToShortTimeString()}] CurrentDate: Date[{_lastReAppointmentDate.ToShortDateString()}] Time[{_lastReAppointmentDate.ToShortTimeString()}]";
                    });
                };

                var (date, time) = await processor.CheckAppointment(reCheckTime, _cancellationTokenSource.Token);

                MessageBox.Show(this, $@"Appointment! Date: {date} Time: {time}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $@"Error: {ex.Message}");
            }
            finally
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
            => _cancellationTokenSource?.Cancel();
    }
}
