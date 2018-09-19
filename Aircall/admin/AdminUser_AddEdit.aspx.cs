using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class AdminUser_AddEdit : System.Web.UI.Page
    {
        IRolesService objRolesService;
        IUsersService objUsersService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindRolesRadioButton();

                if (Request.QueryString["Id"] != null)
                {
                    BindUserById();
                }
            }
        }

        private void BindUserById()
        {
            int UserId = Convert.ToInt32(Request.QueryString["Id"].ToString());
            objUsersService = ServiceFactory.UsersService;
            DataTable dtUser = new DataTable();
            objUsersService.GetUserById(UserId, ref dtUser);
            if (dtUser.Rows.Count > 0)
            {
                btnAdd.Text = "Update";
                //btnUpdate.Visible = true;

                drpRole.SelectedValue = dtUser.Rows[0]["RoleId"].ToString();
                txtFirstname.Text = dtUser.Rows[0]["FirstName"].ToString();
                txtLastname.Text = dtUser.Rows[0]["LastName"].ToString();
                txtUsername.Text = dtUser.Rows[0]["UserName"].ToString();
                txtUsername.Enabled = false;
                rqfvPassword.Enabled = false;
                hdnPassword.Value = dtUser.Rows[0]["Password"].ToString();
                txtEmail.Text = dtUser.Rows[0]["Email"].ToString();
                if (!string.IsNullOrEmpty(dtUser.Rows[0]["Image"].ToString()))
                {
                    lnkImage.HRef = Application["SiteAddress"] + "uploads/profile/" + dtUser.Rows[0]["Image"].ToString();
                    lnkImage.Visible = true;
                }
                hdnImage.Value = dtUser.Rows[0]["Image"].ToString();
                chkActive.Checked = Convert.ToBoolean(dtUser.Rows[0]["IsActive"].ToString());
            }
        }

        private void BindRolesRadioButton()
        {
            objRolesService = ServiceFactory.RolesService;
            DataTable dtRoles = new DataTable();
            objRolesService.GetAdminUserRoles(ref dtRoles);
            if (dtRoles.Rows.Count > 0)
            {
                drpRole.DataSource = dtRoles;
                drpRole.DataTextField = dtRoles.Columns["Name"].ToString();
                drpRole.DataValueField = dtRoles.Columns["Id"].ToString();
                drpRole.DataBind();
            }
            drpRole.Items.Insert(0, new ListItem("Select Role", "0"));
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    objUsersService = ServiceFactory.UsersService;
                    DataTable dtUser = new DataTable();

                    //Check Username is exist in system
                    int UserId = 0;
                    if (string.IsNullOrEmpty(Request.QueryString["Id"]))
                    {
                        objUsersService.GetUserInfoByUsername(txtUsername.Text.Trim(), ref dtUser);
                        if (dtUser.Rows.Count > 0)
                        {
                            dvMessage.InnerHtml = "<strong>Username already exist, please use a different Username.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                        UserId = 0;
                    }
                    else
                        UserId = Convert.ToInt32(Request.QueryString["Id"].ToString());

                    objUsersService.GetUserInfoByEmail(txtEmail.Text.Trim(), UserId, ref dtUser);
                    if (dtUser.Rows.Count > 0)
                    {
                        dvMessage.InnerHtml = "<strong>Email already exist, please use a different email.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }


                    string image = string.Empty;
                    if (fpImage.HasFile)
                    {
                        string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                        if (!AllowedFileExtensions.Contains(fpImage.FileName.Substring(fpImage.FileName.LastIndexOf('.'))))
                        {
                            dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                        else
                        {
                            string filePath = Path.Combine(Server.MapPath("~/uploads/profile/"), fpImage.FileName.Replace(' ', '_'));
                            fpImage.SaveAs(filePath);
                            image = fpImage.FileName.Replace(' ', '_');
                        }
                    }
                    else
                    {
                        image = hdnImage.Value;
                    }


                    //User information
                    BizObjects.Users objAdminUser = new BizObjects.Users();
                    objAdminUser.RoleId = Convert.ToInt32(drpRole.SelectedItem.Value);
                    objAdminUser.FirstName = txtFirstname.Text.Trim();
                    objAdminUser.LastName = txtLastname.Text.Trim();
                    objAdminUser.UserName = txtUsername.Text.Trim();

                    objAdminUser.Email = txtEmail.Text.Trim();
                    objAdminUser.Image = image;
                    objAdminUser.IsActive = chkActive.Checked;
                    objAdminUser.AddedDate = DateTime.UtcNow;

                    bool PasswordChanged = false;
                    string Emailbody = string.Empty;
                    if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
                    {
                        objAdminUser.Id = Convert.ToInt32(Request.QueryString["Id"].ToString());

                        if (!string.IsNullOrEmpty(txtPassword.Text.Trim()))
                        {
                            using (MD5 md5Hash = MD5.Create())
                            {
                                objAdminUser.Password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                            }
                            PasswordChanged = true;
                        }
                        else
                        {
                            objAdminUser.Password = hdnPassword.Value;
                        }
                        objAdminUser.UpdatedDate = DateTime.UtcNow;
                        objUsersService.UpdateUser(ref objAdminUser);

                        if (PasswordChanged)
                        {
                            //Send email to user for update password
                            Emailbody = "User Password: " + txtPassword.Text.Trim();
                            //Email.Send("testlocalcoding@gmail.com", "this.admin", "User Password changed", txtEmail.Text.Trim(), Emailbody, "smtp.gmail.com", 587, true);
                        }
                        Session["msg"] = "edit";
                        Response.Redirect(Application["SiteAddress"] + "admin/AdminUser_List.aspx");
                    }
                    else
                    {
                        using (MD5 md5Hash = MD5.Create())
                        {
                            objAdminUser.Password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                        }
                        var objEmailTemplateService = ServiceFactory.EmailTemplateService;
                        DataTable dtEmailTemplate = new DataTable();
                        objEmailTemplateService.GetByName("ContactUsReply", ref dtEmailTemplate);
                        if (dtEmailTemplate.Rows.Count > 0)
                        {
                            Emailbody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();

                            objUsersService.AddUser(ref objAdminUser);
                            //Send email to added user                            
                            Emailbody = Emailbody.Replace("{{FirstName}}", txtFirstname.Text.Trim());
                            Email.SendEmail(dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString(), txtEmail.Text.Trim(), "", "", Emailbody);
                            //Email.Send("testlocalcoding@gmail.com", "this.admin", "User Added into Syaytm", txtEmail.Text.Trim(), Emailbody, "smtp.gmail.com", 587, true);
                        }
                        Session["msg"] = "add";
                        Response.Redirect(Application["SiteAddress"] + "admin/AdminUser_List.aspx");
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