using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;

namespace Aircall.admin
{
    public partial class PartnerTicket_List : System.Web.UI.Page
    {
        IPartnerTicketRequestService objPartnerTicketRequestService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "close")
                    {
                        dvMessage.InnerHtml = "<strong>Ticket closed successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                GetAllPartnerTicket();
            }
        }

        private void GetAllPartnerTicket()
        {
            objPartnerTicketRequestService = ServiceFactory.PartnerTicketRequestService;
            DataTable dtPartnerTicket = new DataTable();

            string PartnerName = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["PartnerName"]))
            {
                PartnerName = Request.QueryString["PartnerName"].ToString();
                txtPartner.Text = PartnerName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StartDate"]))
            {
                StartDate = Request.QueryString["StartDate"].ToString();
                txtStart.Value = StartDate;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EndDate"]))
            {
                EndDate = Request.QueryString["EndDate"].ToString();
                txtEnd.Value = EndDate;
            }
            objPartnerTicketRequestService.GetAllTicket(PartnerName, StartDate, EndDate, ref dtPartnerTicket);
            if (dtPartnerTicket.Rows.Count > 0)
            {
                lstPartnerTicket.DataSource = dtPartnerTicket;

            }
            lstPartnerTicket.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtPartner.Text.Trim()))
                Param = "?PartnerName=" + txtPartner.Text.Trim();
           
            if (!string.IsNullOrEmpty(txtStart.Value.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&StartDate=" + txtStart.Value.Trim();
                else
                    Param = "?StartDate=" + txtStart.Value.Trim();
            }
            if (!string.IsNullOrEmpty(txtEnd.Value.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&EndDate=" + txtEnd.Value.Trim();
                else
                    Param = "?EndDate=" + txtEnd.Value.Trim();
            }

            Response.Redirect(Application["SiteAddress"] + "admin/PartnerTicket_List.aspx" + Param);
        }

        protected void dataPagerPartnerTicket_PreRender(object sender, EventArgs e)
        {
            dataPagerPartnerTicket.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            GetAllPartnerTicket();
        }
    }
}