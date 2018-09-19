using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;
using System.Configuration;

namespace Aircall.client
{
    public partial class dashboard : System.Web.UI.Page
    {
        #region "Declaration"
        IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
        IServicesService objServicesService = ServiceFactory.ServicesService;
        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
        IClientAddressService objClientAddressService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["ClientLoginCookie"] != null)
            {
                ltrClientName.Text = (Session["ClientLoginCookie"] as LoginModel).FullName.Split((" ").ToCharArray())[0];
                if (!IsPostBack)
                {
                    int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                    BindClientUnits(ClientId);
                    var AddressId = 0;
                    DataTable dt = new DataTable();
                    objClientAddressService = ServiceFactory.ClientAddressService;
                    DataTable dtAddress = new DataTable();
                    objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtAddress);
                    if (dtAddress.Rows.Count > 0)
                    {
                        if (dtAddress.Rows.Count == 1)
                        {

                            AddressId = int.Parse(dtAddress.Rows[0]["Id"].ToString());
                        }
                        else
                        {
                            var rows = dtAddress.Select(" IsDefaultAddress = true ");
                            if (rows.Length > 0)
                            {
                                AddressId = int.Parse(rows[0]["Id"].ToString());
                            }
                        }
                        BindNotifications(ClientId, AddressId);
                    }

                    //DataTable dtClientUnits = new DataTable();
                    //objClientUnitService.GetClientUnitsByClientId(ClientId, ref dtClientUnits);
                    //DataRow[] FailedUnits = dtClientUnits.Select("PaymentStatus='" + General.UnitPaymentTypes.NotReceived.GetEnumDescription() + "' OR PaymentStatus='" + General.UnitPaymentTypes.PaymentFailed.GetEnumDescription() + "' And AddedByType=" + General.UserRoles.Client.GetEnumValue().ToString());
                    //if (FailedUnits.Count() > 0)
                    //    hdnFailedUnit.Value = "1";
                    //else
                    //    hdnFailedUnit.Value = "0";
                    hdnFailedUnit.Value = "0";

                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }
        #endregion

        #region "Functions"
        private void BindNotifications(int clientId, int addressId)
        {
            DataTable dtService = new DataTable();
            objUserNotificationService.GetAllNotificationForDashboardByUserId(clientId, addressId, ref dtService);
            //objServicesService.GetServiceForClient(ClientId, General.ServiceTypes.WaitingApproval.GetEnumDescription(), SelectRow, ref dtService);
            if (dtService.Rows.Count > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("Day");
                dt.Columns.Add("Month");
                dt.Columns.Add("Year");
                dt.Columns.Add("ScheduleStartTime");
                dt.Columns.Add("ScheduleEndTime");
                dt.Columns.Add("ServiceId");
                dt.Columns.Add("Message");

                //dt = dtService.Clone();
                var rows = dtService.Select(" dispOrd = 1 ");
                foreach (DataRow row in rows)
                {
                    var Day = DateTime.Parse(row["ScheduleDate"].ToString()).ToString("dd");
                    var Month = DateTime.Parse(row["ScheduleDate"].ToString()).ToString("MMMM");
                    var Year = DateTime.Parse(row["ScheduleDate"].ToString()).ToString("yyyy");
                    var ServiceId = int.Parse(row["CommonId"].ToString());
                    DataTable dts = new DataTable();
                    objServicesService.GetServiceForClientById(ServiceId, (Session["ClientLoginCookie"] as LoginModel).Id, ref dts);
                    var ScheduleStartTime = dts.Rows[0]["ScheduleStartTime"].ToString();
                    var ScheduleEndTime = dts.Rows[0]["ScheduleEndTime"].ToString();

                    dt.Rows.Add(dt.NewRow());
                    dt.Rows[dt.Rows.Count - 1]["Id"] = row["Id"].ToString();
                    dt.Rows[dt.Rows.Count - 1]["Day"] = Day;
                    dt.Rows[dt.Rows.Count - 1]["Month"] = Month;
                    dt.Rows[dt.Rows.Count - 1]["Year"] = Year;
                    dt.Rows[dt.Rows.Count - 1]["ScheduleStartTime"] = ScheduleStartTime;
                    dt.Rows[dt.Rows.Count - 1]["ScheduleEndTime"] = ScheduleEndTime;
                    dt.Rows[dt.Rows.Count - 1]["ServiceId"] = ServiceId;
                    dt.Rows[dt.Rows.Count - 1]["Message"] = row["Message"].ToString();
                }
                lstNextScheduleService.DataSource = dt;

                DataTable dtSchedule = new DataTable();
                dtSchedule.Columns.Add("Id");
                dtSchedule.Columns.Add("Date");
                dtSchedule.Columns.Add("ServiceId");
                dtSchedule.Columns.Add("Message");
                dtSchedule.Columns.Add("EmpImgURL");

                var rowSchedule = dtService.Select(" dispOrd = 2 ");
                foreach (DataRow row in rowSchedule)
                {
                    var ServiceId = int.Parse(row["CommonId"].ToString());
                    dtSchedule.Rows.Add(dtSchedule.NewRow());
                    dtSchedule.Rows[dtSchedule.Rows.Count - 1]["Id"] = row["Id"].ToString();
                    dtSchedule.Rows[dtSchedule.Rows.Count - 1]["ServiceId"] = ServiceId;
                    dtSchedule.Rows[dtSchedule.Rows.Count - 1]["Message"] = row["Message"].ToString();
                    if (string.IsNullOrEmpty(row["EmplyeePIC"].ToString()))
                    {
                        dtSchedule.Rows[dtSchedule.Rows.Count - 1]["EmpImgURL"] = Application["SiteAddress"] + "uploads/profile/employee/place-holder-img.png";
                    }
                    else
                    {
                        dtSchedule.Rows[dtSchedule.Rows.Count - 1]["EmpImgURL"] = ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + row["EmplyeePIC"].ToString();
                    }
                }
                lstSchedule.DataSource = dtSchedule;

                DataTable dtCC = new DataTable();
                dtCC.Columns.Add("Id");
                dtCC.Columns.Add("Date");
                dtCC.Columns.Add("ServiceId");
                dtCC.Columns.Add("Message");
                var rowCC = dtService.Select(" dispOrd = 3 ");
                foreach (DataRow row in rowCC)
                {
                    var ServiceId = int.Parse(row["CommonId"].ToString());
                    dtCC.Rows.Add(dtCC.NewRow());
                    dtCC.Rows[dtCC.Rows.Count - 1]["Id"] = row["Id"].ToString();
                    dtCC.Rows[dtCC.Rows.Count - 1]["ServiceId"] = ServiceId;
                    dtCC.Rows[dtCC.Rows.Count - 1]["Message"] = row["Message"].ToString();
                }
                lstCCExp.DataSource = dtCC;

                DataTable dtNoShow = new DataTable();
                dtNoShow.Columns.Add("Id");
                dtNoShow.Columns.Add("Date");
                dtNoShow.Columns.Add("ServiceId");
                dtNoShow.Columns.Add("Message");
                dtNoShow.Columns.Add("EmpImgURL");
                var rowNoShow = dtService.Select(" dispOrd = 4 ");
                foreach (DataRow row in rowNoShow)
                {
                    var ServiceId = int.Parse(row["CommonId"].ToString());
                    dtNoShow.Rows.Add(dtNoShow.NewRow());
                    dtNoShow.Rows[dtNoShow.Rows.Count - 1]["Id"] = row["Id"].ToString();
                    dtNoShow.Rows[dtNoShow.Rows.Count - 1]["ServiceId"] = ServiceId;
                    dtNoShow.Rows[dtNoShow.Rows.Count - 1]["Message"] = row["Message"].ToString();
                    if (string.IsNullOrEmpty(row["EmplyeePIC"].ToString()))
                    {
                        dtNoShow.Rows[dtNoShow.Rows.Count - 1]["EmpImgURL"] = Application["SiteAddress"] + "uploads/profile/employee/place-holder-img.png";
                    }
                    else
                    {
                        dtNoShow.Rows[dtNoShow.Rows.Count - 1]["EmpImgURL"] = ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + row["EmplyeePIC"].ToString();
                    }
                }
                lstNoShow.DataSource = dtNoShow;

                DataTable dtPlanExp = new DataTable();
                dtPlanExp.Columns.Add("Id");
                dtPlanExp.Columns.Add("Date");
                dtPlanExp.Columns.Add("ServiceId");
                dtPlanExp.Columns.Add("Message");
                var rowPlanExp = dtService.Select(" dispOrd = 5 ");
                foreach (DataRow row in rowPlanExp)
                {
                    var ServiceId = int.Parse(row["CommonId"].ToString());
                    dtPlanExp.Rows.Add(dtPlanExp.NewRow());
                    dtPlanExp.Rows[dtPlanExp.Rows.Count - 1]["Id"] = row["Id"].ToString();
                    dtPlanExp.Rows[dtPlanExp.Rows.Count - 1]["ServiceId"] = ServiceId;
                    dtPlanExp.Rows[dtPlanExp.Rows.Count - 1]["Message"] = row["Message"].ToString();
                }
                lstPlanExp.DataSource = dtPlanExp;
            }
            lstNextScheduleService.DataBind();
            lstSchedule.DataBind();
            lstCCExp.DataBind();
            lstNoShow.DataBind();
            lstPlanExp.DataBind();
        }
        private void BindClientUnits(int ClientId)
        {
            DataTable dtUnits = new DataTable();
            objClientUnitService.GetClientUnitsForPortal(ClientId, ref dtUnits);
            if (dtUnits.Rows.Count > 0)
            {
                lstUnits.DataSource = dtUnits;
            }
            else
            {
                lstUnits.DataSource = "";
            }
            lstUnits.DataBind();
        }
        private void BindUpcomingService(int ClientId)
        {
            DataTable dtService = new DataTable();
            int SelectRow = 3;
            objServicesService.GetServiceForClient(ClientId, General.ServiceTypes.WaitingApproval.GetEnumDescription(), SelectRow, ref dtService);
            if (dtService.Rows.Count > 0)
            {
                lstNextScheduleService.DataSource = dtService;
            }
            lstNextScheduleService.DataBind();
        }

        [System.Web.Services.WebMethod]
        public static bool DeleteFailedPaymentUnits()
        {
            if (HttpContext.Current.Session["ClientLoginCookie"] != null)
            {
                int ClientId = (HttpContext.Current.Session["ClientLoginCookie"] as LoginModel).Id;
                DataTable dtClientUnits = new DataTable();
                IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
                objClientUnitService.GetClientUnitsByClientId(ClientId, ref dtClientUnits);
                IUserNotificationService objUserNotificationService1 = ServiceFactory.UserNotificationService;
                objUserNotificationService1.DeleteNotificationByUserIdType(ClientId, 4, General.NotificationType.PaymentFailed.GetEnumDescription(),0);
                DataRow[] FailedUnits = dtClientUnits.Select("PaymentStatus='NotReceived' OR PaymentStatus='Failed'");
                foreach (var item in FailedUnits)
                {
                    int UnitId = Convert.ToInt32(item["Id"].ToString());
                    objClientUnitService.DeleteClientUnit(UnitId);
                }
                return true;
            }
            else
                return false;
        }
        #endregion

        #region "Events"
        protected void lstNextScheduleService_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                try
                {
                    BizObjects.Services objServices = new BizObjects.Services();
                    if (e.CommandName == "View" && e.CommandArgument.ToString() != "0")
                    {
                        long ServiceId = Convert.ToInt64((e.Item.FindControl("hfServiceId") as HiddenField).Value);
                        Response.Redirect(Application["SiteAddress"] + "client/ServiceApprove.aspx?Id=" + ServiceId.ToString() + "&nid=" + e.CommandArgument.ToString(), false);
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }
        #endregion

        protected void lstNextScheduleService_ItemDataBound(object sender, ListViewItemEventArgs e)
        {

        }

        protected void lstNoShow_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            try
            {
                BizObjects.Services objServices = new BizObjects.Services();
                if (e.CommandName == "View" && e.CommandArgument.ToString() != "0")
                {
                    long ServiceId = Convert.ToInt64((e.Item.FindControl("hfServiceId") as HiddenField).Value);
                    Session["ServiceId"] = ServiceId;
                    Session["NotificationId"] = e.CommandArgument.ToString();
                    Response.Redirect(Application["SiteAddress"] + "client/NoShowDetail.aspx?nid=" + e.CommandArgument.ToString(), false);
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
            }
        }

        protected void lstSchedule_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            try
            {
                BizObjects.Services objServices = new BizObjects.Services();
                if (e.CommandName == "View" && e.CommandArgument.ToString() != "0")
                {
                    long ServiceId = Convert.ToInt64((e.Item.FindControl("hfServiceId") as HiddenField).Value);
                    Session["ServiceId"] = ServiceId;
                    Session["NotificationId"] = e.CommandArgument.ToString();
                    Response.Redirect(Application["SiteAddress"] + "client/ServiceScheduleDetail.aspx?Id=" + ServiceId.ToString(), false);
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
            }
        }

        protected void lstCCExp_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            try
            {
                BizObjects.Services objServices = new BizObjects.Services();
                if (e.CommandName == "View" && e.CommandArgument.ToString() != "0")
                {
                    long ServiceId = Convert.ToInt64((e.Item.FindControl("hfServiceId") as HiddenField).Value);
                    Response.Redirect(Application["SiteAddress"] + "client/update-payment-method.aspx?cid=" + ServiceId.ToString(), false);
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
            }
        }

        protected void lstPlanExp_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            try
            {
                BizObjects.Services objServices = new BizObjects.Services();
                if (e.CommandName == "View" && e.CommandArgument.ToString() != "0")
                {
                    long ServiceId = Convert.ToInt64((e.Item.FindControl("hfServiceId") as HiddenField).Value);
                    Response.Redirect(Application["SiteAddress"] + "client/PlanRenewSummary.aspx?uid=" + ServiceId.ToString(), false);
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "error");
                dvMessage.Visible = true;
            }
        }
    }
}