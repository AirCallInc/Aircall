using Aircall.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin.controls
{
    public partial class header : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Request.Cookies["LoginCookie"]!=null)
            //{
            if (Session["LoginSession"] != null)
            {
                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                //string Username = Request.Cookies["LoginCookie"]["Username"].ToString();
                //ltrFullname.Text = Request.Cookies["LoginCookie"]["FullName"].ToString();
                //imgUser.Src = Request.Cookies["LoginCookie"]["Image"].ToString();

                string Username = objLoginModel.Username;
                ltrFullname.Text = objLoginModel.FullName;
                imgUser.Src = objLoginModel.Image;
            }
            else
            {
                Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Remove("LoginSession");
            //Session.Abandon();
            //HttpCookie LoginCookie = new HttpCookie("LoginCookie");
            //LoginCookie.Expires = DateTime.Now.AddHours(-1);
            //Response.Cookies.Add(LoginCookie);
            Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
        }
    }
}