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
    public partial class ClientUnitSubscription_List : System.Web.UI.Page
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
                    else if (Session["msg"].ToString() == "paysuccess")
                    {
                        dvMessage.InnerHtml = "<strong>Pay the subscription successfully.</strong>";
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

            var sql = "dbo.uspa_Search_Subscription";
            var clientName = this.txtClient.Text;
            var status = this.drpStatus.SelectedValue;
            var paymentMethod = this.drpPaymentMethod.SelectedValue;
            SqlParameter[] paramArr = new SqlParameter[]
            {
                new SqlParameter("@ClientName", clientName),
                new SqlParameter("@Status", status),
                new SqlParameter("@PaymentMethod", paymentMethod)
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

            Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List.aspx" + Param);
        }

        protected void lstUnitSubscription_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                HtmlTableRow row = new HtmlTableRow();
                row = e.Item.FindControl("trSub") as HtmlTableRow;
                DataRow dr = (e.Item.DataItem as DataRowView).Row;
                var paymentMethod = dr["PaymentMethod"].ToString();
                Button btnPay = e.Item.FindControl("btnPay") as Button;
                if (dr["Status"].ToString() == "Paid")
                {
                    btnPay.Visible = false;
                }
                else
                {
                    row.Attributes["class"] = row.Attributes["class"] + " failed-billing";
                    if (paymentMethod != "CC")
                    {
                        btnPay.Visible = true;
                    }
                    else
                    {
                        btnPay.Visible = false;
                    }
                }
            }
        }

        protected void lstUnitSubscription_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "Pay")
            {
                string URL = Application["SiteAddress"] + "admin/Pay.aspx?SubId=" + e.CommandArgument.ToString();
                Response.Redirect(URL);
            }
        }
    }
}
