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
    public partial class ChangeServiceDate : System.Web.UI.Page
    {
        IServiceDateChangeService objServiceDateChangeService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindServices();
            }
        }

        private void BindServices()
        {
            DataTable dtService = new DataTable();
            objServiceDateChangeService = ServiceFactory.ServiceDateChangeService;
            objServiceDateChangeService.GetAllServices(ref dtService);
            if (dtService.Rows.Count > 0)
            {
                drpServiceCase.DataSource = dtService;
                drpServiceCase.DataTextField = "ServiceCaseNumber";
                drpServiceCase.DataValueField = "Id";
                drpServiceCase.DataBind();
            }
            drpServiceCase.Items.Insert(0, new ListItem("Select Service", "0"));
        }

        protected void drpServiceCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpServiceCase.SelectedValue!="0")
            {
                objServiceDateChangeService = ServiceFactory.ServiceDateChangeService;
                DataTable dtService = new DataTable();
                objServiceDateChangeService.GetServiceById(Convert.ToInt64(drpServiceCase.SelectedValue), ref dtService);
                if (dtService.Rows.Count>0)
                {
                    if (!string.IsNullOrEmpty(dtService.Rows[0]["ExpectedStartDate"].ToString()))
                        txtExpStart.Text = Convert.ToDateTime(dtService.Rows[0]["ExpectedStartDate"].ToString()).ToString("MM/dd/yyyy");
                    if (!string.IsNullOrEmpty(dtService.Rows[0]["ExpectedEndDate"].ToString()))
                        txtExpEnd.Text = Convert.ToDateTime(dtService.Rows[0]["ExpectedEndDate"].ToString()).ToString("MM/dd/yyyy");
                }
            }
            else
            {
                txtExpStart.Text = "";
                txtExpEnd.Text = "";
            }
        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                objServiceDateChangeService = ServiceFactory.ServiceDateChangeService;
                objServiceDateChangeService.UpdateService(Convert.ToInt64(drpServiceCase.SelectedValue), Convert.ToDateTime(txtExpStart.Text), Convert.ToDateTime(txtExpEnd.Text));

                Response.Redirect(Application["SiteAddress"] + "admin/ChangeServiceDate.aspx");
            }
        }
    }
}