using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Services;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using Aircall.Common;

namespace Aircall.partner
{
    public partial class Ticket_List : System.Web.UI.Page
    {
        IPartnerTicketRequestService objPartnerTicketRequestService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PartnerLoginCookie"] != null)
                {
                    if (Request.QueryString["msg"] == "close")
                    {
                        dvMessage.InnerHtml = "<strong>Ticket has been closed successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    BindPartnerTicketRequest();
                }
                else
                {
                    Response.Redirect(Application["SiteAddress"] + "partner/Login.aspx");
                }
            }
        }

        private void BindPartnerTicketRequest()
        {
            int PartnerId = (Session["PartnerLoginCookie"] as LoginModel).Id;//Convert.ToInt32(Request.Cookies["PartnerLoginCookie"]["PartnerId"].ToString());
            objPartnerTicketRequestService = ServiceFactory.PartnerTicketRequestService;
            DataTable dtTicket = new DataTable();
            objPartnerTicketRequestService.GetAllTicketByPartnerId(PartnerId, ref dtTicket);
            if (dtTicket.Rows.Count > 0)
            {
                lstTicket.DataSource = dtTicket;
            }
            lstTicket.DataBind();
        }

        protected void lstTicket_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                HtmlAnchor EditTag = e.Item.FindControl("EditTag") as HtmlAnchor;
                HtmlAnchor ViewTag = e.Item.FindControl("ViewTag") as HtmlAnchor;
                DataRow row = (e.Item.DataItem as DataRowView).Row;

                Literal ltrAddedDate = (Literal)e.Item.FindControl("ltrAddedDate");

                ltrAddedDate.Text = DateTime.Parse(row["AddedDate"].ToString()).ToLocalTime().ToString("MM/dd/yyyy HH:mm:ss tt");
                if (Convert.ToBoolean(row["Status"].ToString()) == false)
                {
                    EditTag.Attributes["style"] = "display:none";
                    ViewTag.HRef = Application["SiteAddress"] + "partner/Ticket_Update.aspx?TicketId=" + row["Id"].ToString();
                }
                else
                {
                    ViewTag.Attributes["style"] = "display:none";
                    EditTag.HRef = Application["SiteAddress"] + "partner/Ticket_Update.aspx?TicketId=" + row["Id"].ToString();
                }
            }
            //if (e.Item.ItemType == ListViewItemType.DataItem)
            //{
            //    HtmlAnchor ViewTag = e.Item.FindControl("ViewTag") as HtmlAnchor;
            //    DataRow row = (e.Item.DataItem as DataRowView).Row;

            //    if (Convert.ToBoolean(row["Status"].ToString()) == true)
            //    {
            //        ViewTag.Attributes["style"] = "display:none";
            //    }
            //    else
            //    {
            //        ViewTag.HRef = Application["SiteAddress"] + "partner/Ticket_Update.aspx?TicketId=" + row["Id"].ToString();
            //    }
            //}
        }
    }
}