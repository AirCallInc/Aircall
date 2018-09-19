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
    public partial class SalesRequest_List : System.Web.UI.Page
    {
        ISalesVisitRequestService objSalesVisitRequestService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Sales Employee Successfully assigned for Client Request.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindSalesVisitRequests();
            }
        }

        private void BindSalesVisitRequests()
        {
            objSalesVisitRequestService = ServiceFactory.SalesVisitRequestService;
            DataTable dtSalesReq = new DataTable();
            string ClientName = string.Empty;
            string EmpName = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["ClientName"]))
            {
                ClientName = Request.QueryString["ClientName"].ToString();
                txtClient.Text = ClientName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EmpName"]))
            {
                EmpName = Request.QueryString["EmpName"].ToString();
                txtEmployee.Text = EmpName;
            }
            objSalesVisitRequestService.GetAllSalesVisitRequest(ClientName,EmpName,ref dtSalesReq);
            if (dtSalesReq.Rows.Count > 0)
                lstSalesRequest.DataSource = dtSalesReq;
            lstSalesRequest.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param = "?ClientName=" + txtClient.Text.Trim();
            if (!string.IsNullOrEmpty(txtEmployee.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&EmpName=" + txtEmployee.Text.Trim();
                else
                    Param = "?EmpName=" + txtEmployee.Text.Trim();
            }

            Response.Redirect(Application["SiteAddress"] + "admin/SalesRequest_List.aspx" + Param);
        }

        protected void dataPagerSalesRequest_PreRender(object sender, EventArgs e)
        {
            dataPagerSalesRequest.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindSalesVisitRequests();
        }
    }
}