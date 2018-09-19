using Aircall.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Web.UI.HtmlControls;

namespace Aircall.admin
{
    public partial class ClientUnitSubscription_List_UnSubmitted : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService;
        IClientUnitSubscriptionService objClientUnitSubscriptionService;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                objClientUnitService = ServiceFactory.ClientUnitService;
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Unit Subscription updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "PChange")
                    {
                        dvMessage.InnerHtml = "<strong>Payment method changed successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Add the subscription successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindSubscriptions();
            }
        }

        private void BindSubscriptions()
        {
            FillStatusDropdown();
            FillPaymentMethodDropdown();

            if (Session["SubscriptionSearch"] != null)
            {
                Session["SubscriptionSearch"] = null;
                Session.Remove("SubscriptionSearch");
            }

            objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
            DataTable dtSubscription = new DataTable();
            string ClientName = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["CName"]))
            {
                ClientName = Request.QueryString["CName"].ToString();
                txtClient.Text = ClientName;
            }

            SubscriptionSearch objSubscriptionSearch = new SubscriptionSearch();

            objSubscriptionSearch.ClientName = txtClient.Text.Trim();

            Session["SubscriptionSearch"] = objSubscriptionSearch;

            objClientUnitSubscriptionService.GetClientUnitUnPaidSubscription(ClientName, ref dtSubscription);
            if (dtSubscription.Rows.Count > 0)
            {
                lstUnitSubscription.DataSource = dtSubscription;
                lstUnitSubscription.DataBind();
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            
        }

        private void FillStatusDropdown()
        {
            
        }

        private void FillPaymentMethodDropdown()
        {
            
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (Session["SubscriptionSearch"] != null)
            {
                Session["SubscriptionSearch"] = null;
                Session.Remove("SubscriptionSearch");
            }

            SubscriptionSearch objSubscriptionSearch = new SubscriptionSearch();

            objSubscriptionSearch.ClientName = txtClient.Text.Trim();

            Session["SubscriptionSearch"] = objSubscriptionSearch;

            string Param = string.Empty;
            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param = "?CName=" + txtClient.Text.Trim();

            Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List_UnSubmitted.aspx" + Param);
        }
    }
}
