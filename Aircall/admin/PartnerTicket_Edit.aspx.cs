using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class PartnerTicket_Edit : System.Web.UI.Page
    {
        IPartnerTicketRequestService objPartnerTicketRequestService;
        IPartnerTicketConversationService objPartnerTicketConversationService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["TicketId"].ToString()))
                {
                    int TicketId = Convert.ToInt32(Request.QueryString["TicketId"].ToString());
                    objPartnerTicketRequestService = ServiceFactory.PartnerTicketRequestService;
                    DataTable dtTicket = new DataTable();

                    objPartnerTicketRequestService.getTicketDetailByTicketId(TicketId, ref dtTicket);
                    if (dtTicket.Rows.Count > 0)
                    {
                        lblSubject.Text = dtTicket.Rows[0]["Subject"].ToString().Trim();
                        lblAddDate.Text = Convert.ToDateTime(dtTicket.Rows[0]["AddedDate"].ToString().Trim()).ToString();
                        lblNote.Text = dtTicket.Rows[0]["Notes"].ToString().Trim();
                        lblType.Text = dtTicket.Rows[0]["TicketType"].ToString().Trim();
                        lblPartner.Text = dtTicket.Rows[0]["FirstName"].ToString().Trim() + " " + dtTicket.Rows[0]["LastName"].ToString().Trim();
                        MessageConversation();
                        if (Convert.ToBoolean(dtTicket.Rows[0]["Status"].ToString()) == false)
                        {
                            Conversation.Attributes["style"] = "display:none";
                        }
                    }
                }
            }
        }

        private void MessageConversation()
        {
            objPartnerTicketConversationService = ServiceFactory.PartnerTicketConversationService;
            int TicketId = Convert.ToInt32(Request.QueryString["TicketId"].ToString());
            DataTable dtConversation = new DataTable();

            objPartnerTicketConversationService.GetConversationByTicketId(TicketId, ref dtConversation);
            if (dtConversation.Rows.Count > 0)
            {
                lstConversion.DataSource = dtConversation;
            }
            lstConversion.DataBind();
        }

        protected void btnstatus_Click(object sender, EventArgs e)
        {
            try
            {
                objPartnerTicketRequestService = ServiceFactory.PartnerTicketRequestService;

                int TicketId = Convert.ToInt32(Request.QueryString["TicketId"].ToString());
                DataTable dtTicket = new DataTable();
                objPartnerTicketRequestService.UpdateStatusById(TicketId);
                Session["msg"] = "close";
                Response.Redirect(Application["SiteAddress"] + "admin/PartnerTicket_List.aspx");
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void btnsave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        objPartnerTicketConversationService = ServiceFactory.PartnerTicketConversationService;
                        BizObjects.PartnerTicketConversation objConversation = new BizObjects.PartnerTicketConversation();
                        DataTable dtMessage = new DataTable();
                        objConversation.Message = txtMessage.Text.ToString().Trim();
                        objConversation.TicketId = Convert.ToInt32(Request.QueryString["TicketId"].ToString());
                        objConversation.SubmittedBy = objLoginModel.Id;
                        objConversation.SubmittedByType = objLoginModel.RoleId;
                        objConversation.MessageDate = DateTime.UtcNow;
                        objPartnerTicketConversationService.AddConversation(ref objConversation);

                        Response.Redirect(Application["SiteAddress"] + "admin/PartnerTicket_Edit.aspx?TicketId=" + Request.QueryString["TicketId"].ToString());
                    }
                    else
                        Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void lstConversion_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                DataRow dr = (DataRow)(e.Item.DataItem as DataRowView).Row;
                Image img = (Image)e.Item.FindControl("Image1");
                string path;
                if (dr["Role"].ToString().ToLower() == "admin")
                    path = Application["SiteAddress"] + "uploads/profile/";
                else
                    path = Application["SiteAddress"] + "uploads/profile/partner/";

                if (!string.IsNullOrEmpty(dr["Image"].ToString()))
                    img.ImageUrl = path + dr["Image"].ToString();
                else
                    img.ImageUrl = path + "defultimage.jpg";

                Literal ltrMessageDate = (Literal)e.Item.FindControl("ltrMessageDate");
                ltrMessageDate.Text = Convert.ToDateTime(dr["MessageDate"]).ToString();
            }
        }
    }
}