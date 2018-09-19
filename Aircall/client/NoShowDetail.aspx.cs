using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class NoShowDetail : System.Web.UI.Page
    {
        IClientService objClientService = ServiceFactory.ClientService;
        IServiceNoShowService objServiceNoShowService = ServiceFactory.ServiceNoShowService;
        ISiteSettingService objSettings = ServiceFactory.SiteSettingService;
        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                if (!IsPostBack)
                {
                    if (!string.IsNullOrWhiteSpace(Request.QueryString["nid"]))
                    {
                        int RequestId;// = int.Parse(Request.QueryString["rid"].ToString());
                        if (!int.TryParse(Request.QueryString["nid"], out RequestId))
                        {
                            Response.Redirect("dashboard.aspx", false);
                        }
                        int NotificationId = int.Parse(Request.QueryString["nid"].ToString());
                        DataTable dt = new DataTable();
                        objServiceNoShowService.GetNoShowDetailsByNotificationId(NotificationId, (Session["ClientLoginCookie"] as LoginModel).Id, ref dt);

                        
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];

                            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                            objUserNotificationService.UpdateStatusByClientIdNotificationIdMessageType(ClientId, long.Parse(row["ServiceId"].ToString()), General.NotificationType.RateService.GetEnumDescription());

                            ltrAmount.Text = "$ " + row["NoShowAmount"].ToString();
                            ltrServiceNo.Text = row["ServiceCaseNumber"].ToString();
                            ltrScheduleDate.Text = DateTime.Parse(row["ScheduleDate"].ToString()).ToString("dd MMMM yyyy");
                            ltrEmp.Text = row["FirstName"].ToString() + " " + row["LastName"].ToString();
                            ltrReson.Text = row["WorkPerformed"].ToString();
                            ltrMessage.Text = (decimal.Parse(row["NoShowAmount"].ToString()) <= 0 ? General.GetSitesettingsValue("NoShowNoPaymentMessage") : General.GetSitesettingsValue("NoShowPaymentMessage"));
                            hdnServiceType.Value = row["ServiceType"].ToString();
                            if (decimal.Parse(row["NoShowAmount"].ToString()) <= 0)
                            {
                                btnPayment.Visible = false;
                            }
                        }
                        else
                        {
                            Response.Redirect("dashboard.aspx", false);
                        }
                    }
                    else
                    {
                        Response.Redirect("dashboard.aspx", false);
                    }
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }

        protected void btnPayment_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ServiceId");
            dt.Columns.Add("ClientId");
            dt.Columns.Add("ServiceCaseNumber");
            dt.Columns.Add("ScheduleDate");
            dt.Columns.Add("WorkPerformed");
            dt.Columns.Add("NoShowAmount");
            dt.Columns.Add("ClientName");
            dt.Columns.Add("ServiceType");

            DataTable dtClient = new DataTable();
            var ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            objClientService.GetClientById(ClientId, ref dtClient);

            dt.Rows.Add(dt.NewRow());
            dt.Rows[0]["ServiceId"] = int.Parse(Session["ServiceId"].ToString());
            dt.Rows[0]["ClientId"] = ClientId;
            dt.Rows[0]["ServiceCaseNumber"] = ltrServiceNo.Text;
            dt.Rows[0]["ScheduleDate"] = ltrScheduleDate.Text;
            dt.Rows[0]["WorkPerformed"] = ltrReson.Text;
            dt.Rows[0]["NoShowAmount"] = ltrAmount.Text;
            dt.Rows[0]["ClientName"] = dtClient.Rows[0]["ClientName"].ToString();
            dt.Rows[0]["ServiceType"] = hdnServiceType.Value;
            Session["PaymentDetail"] = dt;

            Response.Redirect("OtherPayment.aspx");
        }
    }
}