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
using DBUtility;
using System.Data.SqlClient;

namespace Aircall.admin
{
    public partial class RenewCancel_List : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
                    else if (Session["msg"].ToString() == "cancelsuccess")
                    {
                        dvMessage.InnerHtml = "<strong>Cancel the subscription successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "cancelfailed")
                    {
                        dvMessage.InnerHtml = "<strong>Cancel the subscription failed.</strong>";
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
            FillSearch();

            var sql = "dbo.uspa_RenewCancel_Subscription";
            var clientName = this.txtClient.Text;
            var status = this.drpStatus.SelectedValue;
            var paymentMethod = this.drpPaymentMethod.SelectedValue;
            SqlParameter[] paramArr = new SqlParameter[]
            {
                new SqlParameter("@ClientName", clientName),
                new SqlParameter("@Status", "All"),
                new SqlParameter("@PaymentMethod", paymentMethod),
                new SqlParameter("@SubscriptionStatus", status),
            };

            var instance = new SQLDBHelper();
            var ds = instance.QueryBySP(sql, paramArr);
            instance.Close();
            var dtSubscription = ds.Tables[0];

            if (dtSubscription.Rows.Count > 0)
            {
                lstUnitSubscription.DataSource = dtSubscription;
                lstUnitSubscription.DataBind();
            }
        }

        private void FillSearch()
        {
            if (Request.QueryString["CName"] != null)
            {
                this.txtClient.Text = Request.QueryString["CName"];
            }

            if (Request.QueryString["Status"] != null)
            {
                this.drpStatus.SelectedValue = Request.QueryString["Status"];
            }

            if (Request.QueryString["PaymentMethod"] != null)
            {
                this.drpPaymentMethod.SelectedValue = Request.QueryString["PaymentMethod"];
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
            string Param = "?Search=Y";

            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param += "&CName=" + txtClient.Text.Trim();

            Param += "&Status=" + this.drpStatus.SelectedValue;
            Param += "&PaymentMethod=" + this.drpPaymentMethod.SelectedValue;

            Response.Redirect(Application["SiteAddress"] + "admin/RenewCancel_List.aspx" + Param);
        }

        protected void lstUnitSubscription_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                HtmlTableRow row = new HtmlTableRow();
                row = e.Item.FindControl("trSub") as HtmlTableRow;
                DataRow dr = (e.Item.DataItem as DataRowView).Row;
                Button btnCancel = e.Item.FindControl("btnCancel") as Button;
                if (dr["SubscriptionStatus"].ToString() == "Cancelled")
                {
                    row.Attributes["class"] = row.Attributes["class"] + " failed-billing";
                    btnCancel.Visible = false;
                }
                else
                {
                    btnCancel.Visible = true;
                }
            }
        }

        protected void lstUnitSubscription_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "CancelSubscription")
            {
                var subscriptionId = e.CommandArgument.ToString();
                bool isSuccess = false;
                string errCode = "";
                string errText = "";
                var helper = new AuthorizeNetLib.AuthorizeNetHelper();
                if (!string.IsNullOrEmpty(subscriptionId))
                {
                    helper.CancelSubscription(subscriptionId, ref isSuccess, ref errCode, ref errText);
                }
                else
                {
                    isSuccess = true;
                }
                if (isSuccess)
                {
                    var sql = "";
                    if (!string.IsNullOrEmpty(subscriptionId))
                    {
                        sql = string.Format("update ClientUnitSubscription set IsCancelled = '1' where AuthorizeNetSubscriptionId = '{0}'", subscriptionId);
                    }
                    else
                    {
                        HiddenField hdnId = e.Item.FindControl("hdnId") as HiddenField;
                        var id = hdnId.Value;
                        sql = string.Format("update ClientUnitSubscription set IsCancelled = '1' where Id = {0}", id);
                    }
                    var instance = new DBUtility.SQLDBHelper();
                    instance.ExecuteSQL(sql, null);
                    instance.Close();
                    Session["msg"] = "cancelsuccess";
                    string URL = Application["SiteAddress"] + "admin/RenewCancel_List.aspx";
                    Response.Redirect(URL);
                }
                else
                {
                    Session["msg"] = "cancelfailed";
                    string URL = Application["SiteAddress"] + "admin/RenewCancel_List.aspx";
                    Response.Redirect(URL);
                }
            }
        }
    }
}