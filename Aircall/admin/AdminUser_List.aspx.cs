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
    public partial class AdminUser_List : System.Web.UI.Page
    {
        IUsersService objUsersService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>User updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>User added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindAdminUserlist();
            }
        }

        private void BindAdminUserlist()
        {
            string AdminName = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["AdminName"]))
            {
                AdminName = Request.QueryString["AdminName"].ToString();
                txtAdmin.Text = AdminName;
            }

            objUsersService = ServiceFactory.UsersService;
            DataTable dtUsers = new DataTable();
            objUsersService.GetAllAdminUsersByName(AdminName, ref dtUsers);
            if (dtUsers.Rows.Count > 0)
            {
                lstUsers.DataSource = dtUsers;
            }
            lstUsers.DataBind();
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }

        protected void lnkActive_Click(object sender, EventArgs e)
        {
            try
            {
                bool Active = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;
                objUsersService = ServiceFactory.UsersService;

                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                for (int i = 0; i <= lstUsers.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstUsers.Items[i].FindControl("chkcheck");
                    if (chkUsers.Checked)
                    {
                        HiddenField hdnUserId = (HiddenField)lstUsers.Items[i].FindControl("hdnUserId");
                        if (!string.IsNullOrEmpty(hdnUserId.Value) && Convert.ToInt32(hdnUserId.Value) != objLoginModel.Id)
                        {
                            objUsersService.SetStatus(true, Convert.ToInt32(hdnUserId.Value));
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
                BindAdminUserlist();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            try
            {
                bool InActive = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;
                objUsersService = ServiceFactory.UsersService;

                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                for (int i = 0; i <= lstUsers.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstUsers.Items[i].FindControl("chkcheck");
                    if (chkUsers.Checked)
                    {
                        HiddenField hdnUserId = (HiddenField)lstUsers.Items[i].FindControl("hdnUserId");
                        if (!string.IsNullOrEmpty(hdnUserId.Value) && Convert.ToInt32(hdnUserId.Value) != objLoginModel.Id)
                        {
                            objUsersService.SetStatus(false, Convert.ToInt32(hdnUserId.Value));
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
                BindAdminUserlist();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool Deleted = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;
                objUsersService = ServiceFactory.UsersService;

                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                for (int i = 0; i <= lstUsers.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstUsers.Items[i].FindControl("chkcheck");
                    if (chkUsers.Checked)
                    {
                        HiddenField hdnUserId = (HiddenField)lstUsers.Items[i].FindControl("hdnUserId");
                        if (!string.IsNullOrEmpty(hdnUserId.Value) && Convert.ToInt32(hdnUserId.Value) != objLoginModel.Id)
                        {
                            objUsersService.DeleteUser(Convert.ToInt32(hdnUserId.Value));
                            Deleted = true;
                        }
                    }
                }
                if (Deleted)
                {
                    dvMessage.InnerHtml = "<strong>Record deleted successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                BindAdminUserlist();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtAdmin.Text.Trim()))
                Param = "?AdminName=" + txtAdmin.Text.Trim();
            Response.Redirect(Application["SiteAddress"] + "admin/AdminUser_List.aspx" + Param);
        }
    }
}