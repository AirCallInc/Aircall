using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class Profile : System.Web.UI.Page
    {
        IUsersService objUsersService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (Request.Cookies["LoginCookie"] != null)
                //{
                if (Session["LoginSession"] != null)
                {
                    if (Session["msg"] != null)
                    {
                        if (Session["msg"].ToString() == "edit")
                        {
                            dvMessage.InnerHtml = "<strong>Your Profile updated successfully.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-success");
                            dvMessage.Visible = true;
                        }
                        Session["msg"] = null;
                    }
                    BindUserInfo();
                }
                else
                {
                    Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
                }
            }
        }

        private void BindUserInfo()
        {
            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;
            //string Username = Request.Cookies["LoginCookie"]["Username"].ToString();
            string Username = objLoginModel.Username;
            objUsersService = ServiceFactory.UsersService;
            DataTable dtUser = new DataTable();
            objUsersService.GetUserInfoByUsername(Username, ref dtUser);
            if (dtUser.Rows.Count > 0)
            {
                txtFirstname.Text = dtUser.Rows[0]["FirstName"].ToString();
                txtLastname.Text = dtUser.Rows[0]["LastName"].ToString();
                txtUsername.Text = dtUser.Rows[0]["UserName"].ToString();
                txtEmail.Text = dtUser.Rows[0]["Email"].ToString();
                if (!string.IsNullOrEmpty(dtUser.Rows[0]["Image"].ToString()))
                {
                    lnkImage.HRef = Application["SiteAddress"] + "uploads/profile/" + dtUser.Rows[0]["Image"].ToString();
                    lnkImage.Visible = true;
                }
                hdnImage.Value = dtUser.Rows[0]["Image"].ToString();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    //if (Request.Cookies["LoginCookie"] != null)
                    //{
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;
                        //string Username = Request.Cookies["LoginCookie"]["Username"].ToString();
                        string Username = objLoginModel.Username;
                        string image = string.Empty;
                        if (fImage.HasFile)
                        {
                            string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                            if (!AllowedFileExtensions.Contains(fImage.FileName.Substring(fImage.FileName.LastIndexOf('.'))))
                            {
                                dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                            else
                            {
                                string filePath = Path.Combine(Server.MapPath("~/uploads/profile/"), fImage.FileName.Replace(' ', '_'));
                                fImage.SaveAs(filePath);
                                image = fImage.FileName.Replace(' ', '_');
                            }
                        }
                        else
                        {
                            image = hdnImage.Value;
                        }

                        objUsersService = ServiceFactory.UsersService;
                        objUsersService.UpdateUserProfile(Username, txtFirstname.Text.Trim(), txtLastname.Text.Trim(), txtEmail.Text.Trim(), image);

                        Session.Remove("LoginSession");

                        LoginModel objUpdatedLoginModel = new LoginModel();
                        objUpdatedLoginModel.Id = objLoginModel.Id;
                        objUpdatedLoginModel.RoleId = objLoginModel.RoleId;
                        objUpdatedLoginModel.FullName = txtFirstname.Text.Trim() + " " + txtLastname.Text.Trim();
                        objUpdatedLoginModel.Username = txtUsername.Text.Trim();

                        if (string.IsNullOrEmpty(image))
                            objUpdatedLoginModel.Image = Application["SiteAddress"] + "admin/img/avatar1_small.jpg";
                        else
                            objUpdatedLoginModel.Image = Application["SiteAddress"] + "uploads/profile/" + image;

                        Session["LoginSession"] = objUpdatedLoginModel;
                        //Response.Cookies["LoginCookie"]["UserId"] = Request.Cookies["LoginCookie"]["UserId"].ToString();
                        //Response.Cookies["LoginCookie"]["RoleId"] = Request.Cookies["LoginCookie"]["RoleId"].ToString();
                        //Response.Cookies["LoginCookie"]["FullName"] = txtFirstname.Text.Trim() + " " + txtLastname.Text.Trim();
                        //Response.Cookies["LoginCookie"]["Username"] = txtUsername.Text.Trim();

                        //if (string.IsNullOrEmpty(image))
                        //    Response.Cookies["LoginCookie"]["Image"] = Application["SiteAddress"] + "admin/img/avatar1_small.jpg";
                        //else
                        //    Response.Cookies["LoginCookie"]["Image"] = Application["SiteAddress"] + "uploads/profile/" + image;

                        Session["msg"] = "edit";
                        Response.Redirect(Application["SiteAddress"] + "admin/Profile.aspx");
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
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