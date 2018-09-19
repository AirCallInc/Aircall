using LogUtility;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DBUtility;
using AuthorizeNetLib;
using Services;
using System.Net.Mail;
using System.Net;

namespace AuthorizeNetScheduler
{
    public partial class AuthorizeNetService : ServiceBase
    {
        private const string _appName = "AuthorizeNetService";
        private Timer _timer;
        private int _interval = 1; //Unit is second
        private LogHelper _logHelper = new LogHelper();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AuthorizeNetService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (!EventLog.SourceExists(_appName))
            {
                EventLog.CreateEventSource(_appName, "Application");
            }

            _eventLog.Source = _appName;
            _eventLog.WriteEntry(_appName + " is starting...", EventLogEntryType.Information);

            _interval = Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
            _interval = _interval * 1000;
            _timer = new Timer(_interval);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);

            _timer.Enabled = true;
            _timer.Start();

            base.OnStart(args);
        }

        protected override void OnContinue()
        {
            _eventLog.Source = _appName;
            _eventLog.WriteEntry(_appName + " is on continue...", EventLogEntryType.Information);
            _timer.Start();

            base.OnContinue();
        }

        protected override void OnPause()
        {
            _eventLog.Source = _appName;
            _eventLog.WriteEntry(_appName + " is paused...", EventLogEntryType.Information);

            try
            {
                DisableTimers();
            }
            catch (Exception ex)
            {
                string errMsg = _logHelper.GetFullErrorMessage(ex);
                _eventLog.Source = _appName;
                _eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
                _logger.Error(errMsg);
            }

            base.OnPause();
        }

        protected override void OnStop()
        {
            try
            {
                DisableTimers();
            }
            catch (Exception ex)
            {
                string errMsg = _logHelper.GetFullErrorMessage(ex);
                _eventLog.Source = _appName;
                _eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
                _logger.Error(errMsg);
            }

            _eventLog.Source = _appName;
            _eventLog.WriteEntry(_appName + " is stopping...", EventLogEntryType.Information);

            base.OnStop();
        }

        private void DisableTimers()
        {
            try
            {
                if (_timer != null && _timer.Enabled)
                {
                    _timer.Stop();
                    _timer.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string errMsg = _logHelper.GetFullErrorMessage(ex);
                _eventLog.Source = _appName;
                _eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
                _logger.Error(errMsg);
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                StartWork();
            }
            catch (Exception ex)
            {
                string errMsg = _logHelper.GetFullErrorMessage(ex);
                _eventLog.Source = _appName;
                _eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
                _logger.Error(errMsg);
            }
        }

        public void StartWork()
        {
            try
            {
                SyncDataFromAuthorizeNet();
            }
            catch (Exception ex)
            {
                string errMsg = _logHelper.GetFullErrorMessage(ex);
                _eventLog.Source = _appName;
                _eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
                _logger.Error(errMsg);
            }
        }

        public void SyncDataFromAuthorizeNet()
        {
            try
            {
                var sql = @"select A.AuthorizeNetSubscriptionId, A.Id, A.ClientId, A.ClientUnitIds, A.OrderId, A.CardId from ClientUnitSubscription A inner join Client B on A.ClientId = B.Id 
                            where isnull(A.IsDeleted, '0') = '0' and isnull(B.IsDeleted, '0') = '0' 
                            and B.VersionFlag = '2.0' and isnull(A.AuthorizeNetSubscriptionId,'') <> ''";
                var instance = new SQLDBHelper();
                var ds = instance.Query(sql, null);
                instance.Close();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var authorizeNetSubscriptionId = dr["AuthorizeNetSubscriptionId"].ToString();
                        var clientUnitSubscriptionId = Convert.ToInt32(dr["Id"]);
                        var clientId = Convert.ToInt32(dr["ClientId"]);
                        var clientUnitIds = (string)dr["ClientUnitIds"];
                        var orderId = Convert.ToInt32(dr["OrderId"]);
                        var cardId = Convert.ToInt32(dr["CardId"]);
                        object[] paramArr = new object[] { authorizeNetSubscriptionId, clientUnitSubscriptionId, clientId, clientUnitIds, orderId, cardId };

                        BackgroundWorker bgWorker = new BackgroundWorker();
                        bgWorker.DoWork += new DoWorkEventHandler(SyncDataFromAuthorizeNet);
                        bgWorker.ProgressChanged += new ProgressChangedEventHandler(SyncDataFromAuthorizeNet);
                        bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SyncDataFromAuthorizeNet);
                        bgWorker.WorkerReportsProgress = true;
                        bgWorker.RunWorkerAsync(paramArr);
                    }
                }
            }
            catch (Exception ex)
            {
                string errMsg = _logHelper.GetFullErrorMessage(ex);
                _eventLog.Source = _appName;
                _eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
                _logger.Error(errMsg);
            }
        }

        private void SyncDataFromAuthorizeNet(object sender, DoWorkEventArgs e)
        {
            object[] paramArr = (object[])e.Argument;
            var authorizeNetSubscriptionId = (string)paramArr[0];
            var clientUnitSubscriptionId = Convert.ToInt32(paramArr[1]);
            var clientId = Convert.ToInt32(paramArr[2]);
            var clientUnitIds = (string)paramArr[3];
            var orderId = Convert.ToInt32(paramArr[4]);
            var cardId = Convert.ToInt32(paramArr[5]);
            SyncDataFromAuthorizeNet(authorizeNetSubscriptionId, clientUnitSubscriptionId, clientId, clientUnitIds, orderId, cardId);
        }

        private void SyncDataFromAuthorizeNet(object sender, ProgressChangedEventArgs e)
        {

        }

        private void SyncDataFromAuthorizeNet(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        public void SyncDataFromAuthorizeNet(string authorizeNetSubscriptionId, int clientUnitSubscriptionId, int clientId, string clientUnitIds, int orderId, int cardId)
        {
            try
            {
                var helper = new AuthorizeNetHelper();
                string errCode = "", errText = "";
                bool isSuccess = false;
                var transactions = helper.GetSubscriptionTransactions(authorizeNetSubscriptionId, ref isSuccess, ref errCode, ref errText);
                if (isSuccess && transactions != null && transactions.Count > 0)
                {
                    foreach (var tran in transactions)
                    {
                        int payNum = tran.subscription.payNum;
                        string authCode = tran.authCode;

                        BizObjects.BillingHistory objBillingHistory = new BizObjects.BillingHistory();
                        var objBillingHistoryService = ServiceFactory.BillingHistoryService;

                        objBillingHistory.ClientId = clientId;
                        objBillingHistory.ClientUnitIds = clientUnitIds;
                        objBillingHistory.ClientUnitSubscriptionId = clientUnitSubscriptionId;
                        objBillingHistory.PaidMonths = 1;
                        objBillingHistory.OrderId = orderId;
                        objBillingHistory.PackageName = "";
                        objBillingHistory.BillingType = "Recurring Purchase";
                        objBillingHistory.OriginalAmount = tran.authAmount;
                        objBillingHistory.PurchasedAmount = tran.authAmount;
                        objBillingHistory.IsSpecialOffer = false;
                        objBillingHistory.IsPaid = tran.responseCode == 1;
                        objBillingHistory.faildesc = tran.transactionStatus + " " + tran.responseReasonDescription;
                        objBillingHistory.TransactionId = tran.transId;
                        objBillingHistory.TransactionDate = DateTime.UtcNow.ToLocalTime();
                        objBillingHistory.AddedBy = 0;
                        objBillingHistory.AddedDate = DateTime.UtcNow.ToLocalTime();
                        objBillingHistory.CardId = cardId;
                        objBillingHistory.CheckNumbers = "";
                        objBillingHistory.CheckAmounts = "";
                        objBillingHistory.PO = "";
                        objBillingHistory.TransactionStatus = tran.transactionStatus;
                        objBillingHistory.ResponseReasonDescription = tran.responseReasonDescription;
                        objBillingHistory.ResponseCode = tran.responseCode;

                        if (objBillingHistory.IsPaid)
                        {
                            UpdatePaymentStatusToReceived(clientId, clientUnitIds);
                        }

                        var billignId = objBillingHistoryService.AddClientUnitBillingHistory(ref objBillingHistory);

                        if (billignId > 0)
                        {
                            UpdateBilling(billignId, payNum, authCode);
                        }

                        if (!objBillingHistory.IsPaid)
                        {
                            SendEmailToAdmin(objBillingHistory);
                        }
                    }
                }
                else
                {
                    string errMsg = errCode + " " + errText;
                    _eventLog.Source = _appName;
                    _eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
                    _logger.Error(errMsg);
                }
            }
            catch (Exception ex)
            {
                string errMsg = _logHelper.GetFullErrorMessage(ex);
                _eventLog.Source = _appName;
                _eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
                _logger.Error(errMsg);
            }
        }

        private void UpdatePaymentStatusToReceived(int clientId, string unitIds)
        {
            var sql = string.Format("update dbo.ClientUnit set PaymentStatus = '{2}' where ClientId = {0} and Id in ({1})", clientId, unitIds, "Received");
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            instance.Close();
        }

        private void SendEmailToAdmin(BizObjects.BillingHistory objBillingHistory)
        {
            try
            {
                if (HasSentFailedEmail(objBillingHistory.TransactionId))
                {
                    return;
                }

                var subject = "Payment Failed";
                var body = "";
                var path = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates\\PaymentFailed.html";
                using (var sr = new System.IO.StreamReader(path))
                {
                    body = sr.ReadToEnd();
                    sr.Close();
                }
                DataTable dtClient = new DataTable();
                var objClientService = ServiceFactory.ClientService;
                objClientService.GetClientById(objBillingHistory.ClientId, ref dtClient);
                var dr = dtClient.Rows[0];
                body = body.Replace("{{ClientName}}", dr["FirstName"].ToString() + " " + dr["LastName"].ToString());
                body = body.Replace("{{Company}}", dr["Company"].ToString());
                body = body.Replace("{{PaymentMethod}}", "Recurring Purchase");
                body = body.Replace("{{Amount}}", objBillingHistory.PurchasedAmount.ToString());
                body = body.Replace("{{TransactionTime}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                var additionalInformation = "";
                additionalInformation = objBillingHistory.faildesc;
                body = body.Replace("{{AdditionalInformation}}", additionalInformation);
                body = body.Replace("{{Status}}", "UnPaid");
                var objSiteSettingService = ServiceFactory.SiteSettingService;
                DataTable dtSiteSetting = new DataTable();
                objSiteSettingService.GetSiteSettingByName("AdminEmail", ref dtSiteSetting);
                var to = dtSiteSetting.Rows[0]["Value"].ToString();
                var cc = "";
                SendEmail(subject, to, cc, "", body);

                UpdateSentFailedEmail(objBillingHistory.TransactionId);
            }
            catch (Exception ex)
            {
                string errMsg = _logHelper.GetFullErrorMessage(ex);
                _eventLog.Source = _appName;
                _eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
                _logger.Error(errMsg);
            }
        }

        private bool HasSentFailedEmail(string transactionId)
        {
            var sql = string.Format("select top 1 IsSentFailedNotificationEmail from BillingHistory where TransactionId = '{0}'", transactionId);
            var helper = new SQLDBHelper();
            var ds = helper.Query(sql, null);
            helper.Close();
            var dr = ds.Tables[0].Rows[0];
            if (dr["IsSentFailedNotificationEmail"].ToString() == "")
            {
                return false;
            }
            return Convert.ToBoolean(dr["IsSentFailedNotificationEmail"]);
        }

        private void UpdateSentFailedEmail(string transactionId)
        {
            var sql = string.Format("update BillingHistory set IsSentFailedNotificationEmail = '1'  where TransactionId = '{0}'", transactionId);
            var helper = new SQLDBHelper();
            helper.ExecuteSQL(sql, null);
            helper.Close();
        }

        private void SendEmail(string Subject, string EmailTo, string CCEmail, string BCCEmail, string Body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient();

                ISiteSettingService objSiteSettingService;
                objSiteSettingService = ServiceFactory.SiteSettingService;
                DataTable dtResult = new DataTable();
                objSiteSettingService.GetSiteSettingByName("SMTPUser", ref dtResult);
                string Username = dtResult.Rows[0]["Value"].ToString();
                objSiteSettingService.GetSiteSettingByName("SMTPPassword", ref dtResult);
                string Password = dtResult.Rows[0]["Value"].ToString();
                objSiteSettingService.GetSiteSettingByName("SMTPHost", ref dtResult);
                string Host = dtResult.Rows[0]["Value"].ToString();
                objSiteSettingService.GetSiteSettingByName("SMTPPort", ref dtResult);
                int Port = Convert.ToInt32(dtResult.Rows[0]["Value"].ToString());
                objSiteSettingService.GetSiteSettingByName("SMTPSSL", ref dtResult);
                bool EnableSSL = Convert.ToBoolean(dtResult.Rows[0]["Value"].ToString());

                mail.From = new MailAddress(Username, ConfigurationManager.AppSettings["EmailDiaplayName"].ToString());
                mail.Subject = Subject;
                foreach (var s in EmailTo.Split(';'))
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        mail.To.Add(s);
                    }
                }
                //mail.To.Add(EmailTo);
                if (!string.IsNullOrEmpty(CCEmail))
                {
                    string[] CCEmailArr = CCEmail.Split(';');
                    foreach (var item in CCEmailArr)
                    {
                        mail.CC.Add(item.ToString());
                    }
                }
                if (!string.IsNullOrEmpty(BCCEmail))
                    mail.Bcc.Add(BCCEmail);
                mail.Body = Body;
                mail.Priority = MailPriority.High;
                mail.IsBodyHtml = true;

                client.Port = Port;
                client.EnableSsl = EnableSSL;
                if (String.IsNullOrEmpty(Username) && String.IsNullOrEmpty(Password))
                {
                    client.UseDefaultCredentials = true;
                }
                else
                {
                    client.UseDefaultCredentials = false;
                }
                client.Host = Host;
                client.Credentials = new NetworkCredential(Username, Password);
                client.Send(mail);
            }
            catch (Exception ex)
            {
                string errMsg = _logHelper.GetFullErrorMessage(ex);
                _eventLog.Source = _appName;
                _eventLog.WriteEntry(errMsg, EventLogEntryType.Error);
                _logger.Error(errMsg);
            }
        }

        private void UpdateBilling(int billignId, int payNum, string authCode)
        {
            var sql = string.Format("update BillingHistory set PayNum = {0}, AuthCode = '{1}' where Id = {2}", payNum, authCode, billignId);
            var insance = new SQLDBHelper();
            insance.ExecuteSQL(sql, null);
            insance.Close();
        }
    }
}
