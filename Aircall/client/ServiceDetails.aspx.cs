using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BizObjects;
using Aircall.Common;

namespace Aircall.client
{
    public partial class ServiceDetails : System.Web.UI.Page
    {
        IServiceUnitService objServiceUnitService = ServiceFactory.ServiceUnitService;
        IServicesService objServicesService = ServiceFactory.ServicesService;
        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
        IEmployeeService objEmployeeService = ServiceFactory.EmployeeService;

        DataTable dtResult = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["sid"] == null)
                {
                    Response.Redirect("past_services.aspx");
                }
                else
                {
                    long ServiceId;// = Convert.ToInt64(Request.QueryString["sid"].ToString());
                    if (!long.TryParse(Request.QueryString["sid"], out ServiceId))
                    {
                        Response.Redirect("past_services.aspx", false);
                    }
                    objServicesService.GetCompletedServiceDetailsById(ServiceId, (Session["ClientLoginCookie"] as LoginModel).Id, ref dtResult);
                    if (dtResult.Rows.Count > 0)
                    {
                        int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                        objUserNotificationService.UpdateStatusByClientIdNotificationIdMessageType(ClientId, ServiceId, General.NotificationType.RateService.GetEnumDescription());

                        DataRow row = dtResult.Rows[0];
                        DataTable dt = new DataTable();
                        objServiceUnitService.GetServiceUnitsForPortal(ServiceId, ref dt);


                        ltrServiceNo.Text = row["ServiceCaseNumber"].ToString();
                        ltrAddress.Text = row["ClientAddress"].ToString();
                        ltrEmpName.Text = row["EmployeeName"].ToString();
                        hdnEmployeeId.Value = row["EmployeeId"].ToString();
                        ltrServiceDate.Text = Convert.ToDateTime(row["ScheduleDate"].ToString()).ToString("MM/dd/yyyy");
                        ltrPurposeOfVisit.Text = row["PurposeOfVisit"].ToString();

                        ltrServiceStartTime.Text = row["WorkStartedTime"].ToString();
                        ltrServiceCompletedTime.Text = row["WorkCompletedTime"].ToString();

                        ltrAssignedStartTime.Text = row["ScheduleStartTime"].ToString();
                        ltrAssignedEndTime.Text = DateTime.Parse(row["ScheduleEndTime"].ToString()).AddHours(1).ToString("hh:mm tt");
                        ltrAssignedTotalTime.Text = row["AssignedTotalTime"].ToString();
                        ltrExtraTime.Text = row["ExtraTime"].ToString() == "0" || row["ExtraTime"].ToString() == "" ? "" : row["ExtraTime"].ToString() + " Minutes"; ;

                        if (ltrServiceStartTime.Text == ltrAssignedStartTime.Text && ltrServiceCompletedTime.Text == ltrAssignedEndTime.Text)
                        {
                            ServiceCompletedTimeBlock.Attributes.Add("style", "display:none");
                            ServiceStartTimeBlock.Attributes.Add("style", "display:none");
                        }

                        ltrWorkPerformed.Text = row["WorkPerformed"].ToString();
                        ltrRecommendation.Text = row["Recommendation"].ToString();
                        txtReview.Text = row["Review"].ToString();
                        dvRating.Attributes.Add("title", row["Rating"].ToString());
                        if (Convert.ToBoolean(row["IsNoShow"].ToString()))
                        {
                            dvRateBox.Attributes.Add("style", "display:none");
                            dvReveiwBox.Attributes.Add("style", "display:none");
                            ServiceCompletedTimeBlock.Attributes.Add("style", "display:none");
                            ServiceStartTimeBlock.Attributes.Add("style", "display:none");
                            //UnitNameBlock.Attributes.Add("style", "display:none");
                            dvNoteBox.Attributes.Add("style", "display:none");
                            btnSubmit.Visible = false;
                            txtReview.Visible = false;
                            ltrUnitName.Text = dt.Rows[0]["ServicePlans"].ToString();
                        }
                        else
                        {
                            ltrUnitName.Text = dt.Rows[0]["ServiceCompletedUnits"].ToString();
                        }
                        if (row["Rating"].ToString() == "0")
                        {
                            dvRating.Attributes.Add("class", "starbox3");
                        }
                        else
                        {
                            btnSubmit.Enabled = false;
                            btnSubmit.Visible = false;
                            txtReview.ReadOnly = true;
                            dvRating.Attributes.Add("class", "starbox2");
                        }
                        if (!string.IsNullOrWhiteSpace(row["Image"].ToString()))
                        {
                            imgTechPer.ImageUrl = ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + row["Image"].ToString();
                        }
                        else
                        {
                            imgTechPer.ImageUrl = "images/place-holder-img.png";
                        }
                    }
                    else
                    {
                        Response.Redirect("past_services.aspx");
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (string.IsNullOrEmpty(hdnRate.Value))
                {
                    lblRequired.Visible = true;
                    return;
                }
                if (Request.QueryString["sid"] != null && Session["ClientLoginCookie"] != null)
                {
                    long ServiceId = Convert.ToInt64(Request.QueryString["sid"].ToString());
                    ServiceRatingReview review = new ServiceRatingReview();
                    review.Review = string.IsNullOrWhiteSpace(txtReview.Text) ? "No Review Added" : txtReview.Text;
                    review.ReviewDate = DateTime.UtcNow;

                    if (string.IsNullOrEmpty(hdnRate.Value))
                        review.Rate = 0;
                    else
                        review.Rate = decimal.Parse(hdnRate.Value) * 5;
                    review.ServiceId = ServiceId;

                    ServiceRatingReviewService srrs = new ServiceRatingReviewService();
                    srrs.AddServiceRatingReview(ref review);

                    objUserNotificationService.DeleteNotificationByCommonIdType(ServiceId, General.NotificationType.RateService.GetEnumDescription());

                    if (!string.IsNullOrEmpty(hdnEmployeeId.Value))
                    {
                        int EmployeeId = Convert.ToInt32(hdnEmployeeId.Value);
                        objEmployeeService = ServiceFactory.EmployeeService;
                        DataTable dtEmployee = new DataTable();
                        objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                        if (dtEmployee.Rows.Count > 0)
                        {
                            long NotificationId = 0;
                            int BadgeCount = 0;
                            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                            string message = "Client " + (Session["ClientLoginCookie"] as LoginModel).FullName + " has submited ratings for your service.";
                            objUserNotification.UserId = EmployeeId;
                            objUserNotification.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.CommonId = ServiceId;
                            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                            objUserNotification.MessageType = General.NotificationType.RateService.GetEnumDescription();
                            objUserNotification.AddedDate = DateTime.UtcNow;

                            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                            DataTable dtBadgeCount = new DataTable();
                            objUserNotificationService.GetBadgeCount(EmployeeId, General.UserRoles.Employee.GetEnumValue(), ref dtBadgeCount);
                            BadgeCount = dtBadgeCount.Rows.Count;

                            Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.RateService.GetEnumValue(), CommonId = ServiceId };
                            List<NotificationModel> notify = new List<NotificationModel>();
                            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                            if (!string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceType"].ToString()) &&
                                    !string.IsNullOrEmpty(dtEmployee.Rows[0]["DeviceToken"].ToString()) &&
                                     dtEmployee.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                            {
                                if (dtEmployee.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                {
                                    SendNotifications.SendIphoneNotification(BadgeCount, dtEmployee.Rows[0]["DeviceToken"].ToString(), message, notify, "employee");
                                }
                            }
                        }
                    }

                    Response.Redirect("past_services.aspx");
                }
            }
        }
    }
}