using Aircall.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin.controls
{
    public partial class sidebar : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoginSession"] != null)
            {
                //int RoleId = Convert.ToInt32(Request.Cookies["LoginCookie"]["RoleId"].ToString());
                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;
 
                int RoleId = objLoginModel.RoleId;
                if (General.UserRoles.WarehouseUser.GetEnumValue()==RoleId)
                {
                    liServices.Visible = false;
                    liClient.Visible = false;
                    liEmployee.Visible = false;
                    liPartner.Visible = false;
                    liPlan.Visible = false;
                    liUser.Visible = false;
                    liLocation.Visible = false;
                    liInventory.Visible = true;
                    liRequest.Visible = false;
                    liRequest.Visible = false;
                    liCMS.Visible = false;
                    liNews.Visible = false;
                    liOrders.Visible = false;
                    liOthers.Visible = false;
                    liSalesReport.Visible = false;
                    liUnitReport.Visible = false;
                    liRatingReport.Visible = false;
                    liPartnerReport.Visible = false;
                    liRecurringReport.Visible = false;
                }
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