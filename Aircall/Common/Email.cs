using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Services;
using System.Data;
using System.Configuration;

namespace Aircall.Common
{
    public class Email
    {
        public static void Send(string Username, string Password, string Subject, string EmailTo, string Body, string Host, int Port, bool EnableSSL)
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();

            mail.From = new MailAddress(Username, "Aircall System");
            mail.Subject = Subject;
            mail.To.Add(EmailTo);
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

        public static void SendEmail(string Subject, string EmailTo, string CCEmail, string BCCEmail,string Body)
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
            catch (Exception Ex)
            {
                
            }
        }
    }
}