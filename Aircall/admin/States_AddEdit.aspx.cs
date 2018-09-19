using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class States_AddEdit : System.Web.UI.Page
    {
        IStateService objStateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["StateId"]))
                {
                    BindStateById();
                }
            }
        }

        private void BindStateById()
        {
            btnSave.Text = "Update";
            //btnUpdate.Visible = true;

            int StateId = Convert.ToInt32(Request.QueryString["StateId"].ToString());
            DataTable dtState = new DataTable();
            objStateService = ServiceFactory.StateService;
            objStateService.GetStateById(StateId, ref dtState);
            if (dtState.Rows.Count > 0)
            {
                txtStateName.Text = dtState.Rows[0]["Name"].ToString();
                chkActive.Checked = Convert.ToBoolean(dtState.Rows[0]["DisplayStatus"].ToString());
                //chkPendingInActive.Checked = Convert.ToBoolean(dtState.Rows[0]["PendingInactive"].ToString());
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        objStateService = ServiceFactory.StateService;
                        
                        DataTable dtState = new DataTable();
                        int StateId = 0;
                        if (string.IsNullOrEmpty(Request.QueryString["StateId"]))
                            StateId = 0;
                        else
                            StateId = Convert.ToInt32(Request.QueryString["StateId"]);


                        objStateService.GetStateByName(txtStateName.Text.Trim(), StateId,ref dtState);
                        if (dtState.Rows.Count > 0)
                        {
                            dvMessage.InnerHtml = "<strong>State name already exists.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        BizObjects.State objState = new BizObjects.State();
                        objState.Name = txtStateName.Text.Trim();
                        objState.Status = chkActive.Checked;
                        //objState.PendingInactive = chkPendingInActive.Checked;
                        objState.AddedBy = objLoginModel.Id;
                        objState.AddedByType = objLoginModel.RoleId;
                        objState.AddedDate = DateTime.UtcNow;

                        if (!string.IsNullOrEmpty(Request.QueryString["StateId"]))
                        {
                            StateId = Convert.ToInt32(Request.QueryString["StateId"]);
                            objState.Id = StateId;
                            objState.UpdatedBy = objLoginModel.Id;
                            objState.UpdatedByType = objLoginModel.RoleId;
                            objState.UpdatedDate = DateTime.UtcNow;
                            objStateService.UpdateState(ref objState);
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/States_list.aspx");
                        }
                        else
                        {
                            objStateService.AddStates(ref objState);
                            Session["msg"] = "add";
                            Response.Redirect(Application["SiteAddress"] + "admin/States_list.aspx");
                        }
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "/admin/Login.aspx");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }
    }
}