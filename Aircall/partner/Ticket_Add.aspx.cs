using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Services;
using System.Web.UI.WebControls;
using System.Data;
using Aircall.Common;

namespace Aircall.partner
{
    public partial class Ticket_Add : System.Web.UI.Page
    {
        IPartnerTicketRequestService objPartnerTicketRequestService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PartnerLoginCookie"] != null)
                {
                    BindTickets();
                }
                else
                {
                    Response.Redirect(Application["SiteAddress"] + "partner/Login.aspx");
                }

            }
        }

        private void BindTickets()
        {
            var purpose = DurationExtensions.GetValues<General.TicketTypes>();
            List<string> TicketList = new List<string>();
            foreach (var item in purpose)
            {
                General.TicketTypes p = (General.TicketTypes)item;
                TicketList.Add(p.GetEnumDescription());
            }

            ddlTicket.DataSource = TicketList;
            ddlTicket.DataBind();
            ddlTicket.Items.Insert(0, new ListItem("Select Ticket Type", ""));
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["PartnerLoginCookie"] != null)
                    {
                        objPartnerTicketRequestService = ServiceFactory.PartnerTicketRequestService;
                        BizObjects.PartnerTicketRequest objTicket = new BizObjects.PartnerTicketRequest();
                        DataTable dtTicket = new DataTable();

                        objTicket.PartnerId = (Session["PartnerLoginCookie"] as LoginModel).Id;

                        objTicket.TicketType = Convert.ToString(ddlTicket.SelectedValue);
                        objTicket.Subject = txtSubject.Text.ToString().Trim();
                        objTicket.Notes = txtNote.Text.ToString().Trim();
                        objTicket.AddedDate = DateTime.UtcNow;

                        objPartnerTicketRequestService.AddPartnerTicket(ref objTicket);
                        Response.Redirect(Application["SiteAddress"] + "partner/Ticket_List.aspx?msg=add");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }
    }
}
