using Aircall.Common;
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
    public partial class States_list : System.Web.UI.Page
    {
        IStateService objStateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>State updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>State added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindStates();
            }
        }
        private void BindStates()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtStates = new DataTable();
            objStateService.GetAllStates(false, true,ref dtStates);
            if (dtStates.Rows.Count > 0)
            {
                lstState.DataSource = dtStates;
            }
            lstState.DataBind();
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }

        protected void lnkActive_Click(object sender, EventArgs e)
        {
            bool Active = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objStateService = ServiceFactory.StateService;
            for (int i = 0; i <= lstState.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstState.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnStateId = (HiddenField)lstState.Items[i].FindControl("hdnStateId");
                    if (!string.IsNullOrEmpty(hdnStateId.Value))
                    {
                        objStateService.SetStatus(true, Convert.ToInt32(hdnStateId.Value));
                        Active = true;
                    }
                }
            }
            if (Active)
            {
                dvMessage.InnerHtml = "<strong>Record updated successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindStates();
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            bool InActive = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objStateService = ServiceFactory.StateService;
            for (int i = 0; i <= lstState.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstState.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnStateId = (HiddenField)lstState.Items[i].FindControl("hdnStateId");
                    if (!string.IsNullOrEmpty(hdnStateId.Value))
                    {
                        objStateService.SetStatus(false, Convert.ToInt32(hdnStateId.Value));
                        InActive = true;
                    }
                }
            }
            if (InActive)
            {
                dvMessage.InnerHtml = "<strong>Record updated successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindStates();
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            bool Delete = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            
            BizObjects.State objState = new BizObjects.State();
            objStateService = ServiceFactory.StateService;

            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            int UserId=objLoginModel.Id;
            int RoleId = objLoginModel.RoleId;
            
            for (int i = 0; i <= lstState.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstState.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnStateId = (HiddenField)lstState.Items[i].FindControl("hdnStateId");
                    if (!string.IsNullOrEmpty(hdnStateId.Value))
                    {
                        objState.Id=Convert.ToInt32(hdnStateId.Value);
                        objState.DeletedBy=UserId;
                        objState.DeletedByType=RoleId;
                        objState.DeletedDate=DateTime.UtcNow;
                        objStateService.DeleteState(ref objState);
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
            BindStates();
        }
    }
}