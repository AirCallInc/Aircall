using JdSoft.Apple.Apns.Notifications;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Aircall.Common
{
    public class SendNotifications
    {
        public static string SendAndroidNotification(string Deviceid, string message, string CustomData, string from)
        {
            String sResponseFromServer = "";
            if (Deviceid != "nodevicetoken" && !string.IsNullOrEmpty(Deviceid) && Deviceid != "-")
            {
                string GoogleAppID = System.Configuration.ConfigurationManager.AppSettings["GoogleAppID"].ToString();
                //message = HttpContext.Current.Server.UrlEncode(from) + " has sent you a message";
                string SENDER_ID = System.Configuration.ConfigurationManager.AppSettings["SENDERID"].ToString();
                dynamic value = message;
                WebRequest tRequest = default(WebRequest);
                tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
                string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + Convert.ToString(value) + "&registration_id=" + Deviceid + "" + CustomData + "";
                Console.WriteLine(postData);
                Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;

                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();

                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);
                sResponseFromServer = tReader.ReadToEnd();
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
            }
            return sResponseFromServer;
        }

        public static void SendIphoneNotification(int BadgeCount, string DeviceId, string message, List<NotificationModel> objArray, string to)
        {
            if (DeviceId != "nodevicetoken" && !string.IsNullOrEmpty(DeviceId) && DeviceId != "-" && !DeviceId.Contains("null"))
            {
                bool sandBox = Convert.ToBoolean(ConfigurationManager.AppSettings["sandBox"].ToString());
                int numConnections = 1;
                // you can change the number of connections here
                string p12FileName = string.Empty;
                string p12Password = string.Empty;
                if (to == "client")
                {
                    p12FileName = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["p12ClientFileName"].ToString());
                    p12Password = ConfigurationManager.AppSettings["p12ClientPassword"].ToString();
                }
                else
                {
                    p12FileName = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["p12EmployeeFileName"].ToString());
                    p12Password = ConfigurationManager.AppSettings["p12EmployeePassword"].ToString();
                }

                NotificationService notificationService = new JdSoft.Apple.Apns.Notifications.NotificationService(sandBox, p12FileName, p12Password, numConnections);

                //message = to + " has sent you a message";
                Notification notification = new Notification(DeviceId);
                notification.Payload.Alert.Body = message;

                foreach (var item in objArray)
                {
                    notification.Payload.CustomItems.Add(item.Key, item.Value);
                }

                notificationService.NotificationFailed += notificationService_NotificationFailed;
                notificationService.BadDeviceToken += notificationService_BadDeviceToken;
                notificationService.Error += notificationService_Error;
                notificationService.NotificationSuccess += notificationService_NotificationSuccess;
                notification.Payload.Badge = BadgeCount;
                if ((notificationService.QueueNotification(notification)))
                {
                    // queued the notification
                }
                else
                {
                    // failed to queue
                }
            }
        }

        static void notificationService_NotificationSuccess(object sender, Notification notification)
        {
            //ExceptionUtility.LogException(new Exception("Notification Sent successfully"), "Notification Sent successfully", HttpContext.Current);
        }

        static void notificationService_Error(object sender, Exception ex)
        {
            //ExceptionUtility.LogException(ex, "False Notification Iphone", HttpContext.Current);
        }

        static void notificationService_NotificationFailed(object sender, Notification failed)
        {
            //ExceptionUtility.LogException(new Exception(), "False Notification Iphone", HttpContext.Current);
        }

        static void notificationService_BadDeviceToken(object sender, BadDeviceTokenException ex)
        {
            //ExceptionUtility.LogException(ex, "False Notification Iphone", HttpContext.Current);
        }
    }
}