using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class ContactRequest_List : System.Web.UI.Page
    {
        IContactRequestService objContactRequestService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "sent")
                    {
                        dvMessage.InnerHtml = "<strong>Contact Response request was sent successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindContactRequest();
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }

        private void BindContactRequest()
        {
            objContactRequestService = ServiceFactory.ContactRequestService;
            DataTable dtRequests = new DataTable();
            string Name = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["Name"]))
            {
                Name = Request.QueryString["Name"].ToString();
                txtName.Text = Name;
            }
            objContactRequestService.GetAllContactRequest(Name,ref dtRequests);
            if (dtRequests.Rows.Count > 0)
            {
                lstContactRequest.DataSource = dtRequests;
            }
            lstContactRequest.DataBind();
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool Delete = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;

                objContactRequestService = ServiceFactory.ContactRequestService;

                for (int i = 0; i <= lstContactRequest.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkRequest = (HtmlInputCheckBox)lstContactRequest.Items[i].FindControl("chkcheck");
                    if (chkRequest.Checked)
                    {
                        HiddenField hdnId = (HiddenField)lstContactRequest.Items[i].FindControl("hdnId");
                        if (!string.IsNullOrEmpty(hdnId.Value))
                        {
                            objContactRequestService.DeleteContactRequest(Convert.ToInt32(hdnId.Value));
                            Delete = true;
                        }
                    }
                }
                if (Delete)
                {
                    dvMessage.InnerHtml = "<strong>Record deleted successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                BindContactRequest();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void dataPagerContactReq_PreRender(object sender, EventArgs e)
        {
            dataPagerContactReq.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindContactRequest();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtName.Text.Trim()))
                Param = "?Name=" + txtName.Text.Trim();

            Response.Redirect(Application["SiteAddress"] + "admin/ContactRequest_List.aspx" + Param);
        }
    }
}