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
    public partial class my_units : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService = ServiceFactory.ClientUnitService;
        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
        IClientService objClientService = ServiceFactory.ClientService;
        DataTable dtClient = new DataTable();
        IClientAddressService objClientAddressService = ServiceFactory.ClientAddressService;
        decimal total = 0m;
        int ClientId = 0;
        int DefaultAddress = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                BindClientAddress();

                if (!IsPostBack)
                {
                    BindUnitData();
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
        }

        private void BindClientAddress()
        {
            ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            DataTable dtAddress = new DataTable();
            objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtAddress);
            if (dtAddress.Rows.Count > 0)
            {
                DataTable dt = dtAddress.Clone();
                var rows = dtAddress.Select(" ShowInList = true ");
                var defaultadd = dtAddress.Select(" IsDefaultAddress = true ");
                if (defaultadd.Length > 0)
                {
                    DefaultAddress = Convert.ToInt32(defaultadd[0]["Id"]);
                    if (AddressExpression == 0)
                    {
                        AddressExpression = DefaultAddress;
                    }
                }
                foreach (var row in rows)
                {
                    dt.Rows.Add(row.ItemArray);
                }

                lstAddress.DataSource = dt;

            }
            lstAddress.DataBind();
        }

        protected void dataPagerRequest_PreRender(object sender, EventArgs e)
        {
            it.PageSize = int.Parse(General.GetSitesettingsValue("ClientPortalPageSize"));
            BindUnitData();
        }

        private void BindUnitData()
        {
            try
            {
                ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

                objClientService.GetClientById(ClientId, ref dtClient);

                //objClientUnitService.GetClientUnitByClientAndAddressIdForPortal(ClientId, DefaultAddress, ref dtClient);
                objClientUnitService.GetClientUnitByClientAndAddressIdForPortal(ClientId, AddressExpression, ref dtClient);
                DataTable dt = dtClient.Clone();

                //var rows = dtClient.Select(" PaymentStatus='" + General.UnitPaymentTypes.Received.GetEnumDescription() + "' ");
                var rows = dtClient.Select(" 1=1");
                foreach (DataRow row in rows)
                {
                    dt.Rows.Add(row.ItemArray);
                }

                lstSummary.DataSource = dt;
                lstSummary.DataBind();

                DataTable dtClientUnits = new DataTable();
                objClientUnitService.GetClientUnitsByClientId(ClientId, ref dtClientUnits);

                DataRow[] FailedUnits = dtClientUnits.Select("PaymentStatus='" + General.UnitPaymentTypes.NotReceived.GetEnumDescription() + "' OR PaymentStatus='" + General.UnitPaymentTypes.PaymentFailed.GetEnumDescription() + "' And AddedByType=" + General.UserRoles.Client.GetEnumValue().ToString());
                if (FailedUnits.Count() > 0)
                    hdnFailedUnit.Value = "1";
                else
                    hdnFailedUnit.Value = "0";
            }
            catch (Exception ex)
            {
            }
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
                    //objClientUnitService.DeleteClientUnit(UnitId);
                }
                return true;
            }
            else
                return false;
        }

        protected void lstAddress_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                if (e.CommandName == "SelectAddress" && e.CommandArgument.ToString() != "0")
                {
                    dtClient = new DataTable();
                    var AddressId = Convert.ToInt32(e.CommandArgument);
                    AddressExpression = AddressId;
                    //ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                    //objClientUnitService.GetClientUnitByClientAndAddressIdForPortal(ClientId, AddressExpression, ref dtClient);
                    //DataTable dt = dtClient.Clone();

                    //var rows = dtClient.Select(" PaymentStatus='" + General.UnitPaymentTypes.Received.GetEnumDescription() + "' ");
                    //foreach (DataRow row in rows)
                    //{
                    //    dt.Rows.Add(row.ItemArray);
                    //}

                    //lstSummary.DataSource = dt;
                    //lstSummary.DataBind();
                    BindClientAddress();
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }
        protected void lstAddress_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var row = (e.Item.DataItem as DataRowView).Row;
                LinkButton lnkDelete = e.Item.FindControl("lnkDelete") as LinkButton;
            }
        }

        protected int AddressExpression
        {
            get
            {
                if (Session["AddressExpression"] == null)
                    Session["AddressExpression"] = "0";
                return int.Parse(Session["AddressExpression"].ToString());
            }
            set { Session["AddressExpression"] = value; }
        }
        protected void lstSummary_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                Literal litUnitName = e.Item.FindControl("litUnitName") as Literal;
                Literal litPlan = e.Item.FindControl("litPlan") as Literal;
                Literal litStatus = e.Item.FindControl("litStatus") as Literal;


                DataRow row = (e.Item.DataItem as DataRowView).Row;
                General.UnitStatus u = (General.UnitStatus)int.Parse(row["Status"].ToString());
                //var desc = row["Description"].ToString();
                litUnitName.Text = row["UnitName"].ToString();
                litPlan.Text = row["PlanTypeName"].ToString();
                litStatus.Text = u.GetEnumDescription();
            }
        }
    }
}