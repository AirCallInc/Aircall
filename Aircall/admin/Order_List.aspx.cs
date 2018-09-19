using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;
using System.Web.UI.HtmlControls;

namespace Aircall.admin
{
    public partial class Order_List : System.Web.UI.Page
    {
        IOrderService objOrderService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Order added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindOrders();
            }
        }

        private void BindOrders()
        {
            objOrderService = ServiceFactory.OrderService;
            DataTable dtOrders = new DataTable();
            
            string ClientName = string.Empty;
            string EmpName = string.Empty;
            string StartDate = string.Empty;
            string EndDate = string.Empty;

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
            if (!string.IsNullOrEmpty(Request.QueryString["StartDate"]))
            {
                StartDate = Request.QueryString["StartDate"].ToString();
                txtStart.Value = StartDate;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EndDate"]))
            {
                EndDate = Request.QueryString["EndDate"].ToString();
                txtEnd.Value = EndDate;
            }

            objOrderService.GetAllOrders(ClientName,EmpName,StartDate,EndDate,ref dtOrders);
            if (dtOrders.Rows.Count > 0)
            {
                lstOrders.DataSource = dtOrders;
                lstOrders.DataBind();
            }
        }

        protected void dataPagerOrder_PreRender(object sender, EventArgs e)
        {
            dataPagerOrder.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindOrders();
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
            if (!string.IsNullOrEmpty(txtStart.Value.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&StartDate=" + txtStart.Value.Trim();
                else
                    Param = "?StartDate=" + txtStart.Value.Trim();
            }
            if (!string.IsNullOrEmpty(txtEnd.Value.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&EndDate=" + txtEnd.Value.Trim();
                else
                    Param = "?EndDate=" + txtEnd.Value.Trim();
            }

            Response.Redirect(Application["SiteAddress"] + "admin/Order_List.aspx" + Param);
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            bool Delete = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;
            int UserId = objLoginModel.Id;
            int RoleId = objLoginModel.RoleId;

            BizObjects.Orders objOrders = new BizObjects.Orders();
            objOrderService = ServiceFactory.OrderService;

            for (int i = 0; i <= lstOrders.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstOrders.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdOrderId = (HiddenField)lstOrders.Items[i].FindControl("hdOrderId");
                    if (!string.IsNullOrEmpty(hdOrderId.Value))
                    {
                        objOrders.Id = Convert.ToInt32(hdOrderId.Value);
                        objOrders.DeletedBy = UserId;
                        objOrders.DeletedByType = RoleId;
                        objOrders.DeletedDate = DateTime.UtcNow;
                        objOrderService.DeleteOrderById(ref objOrders);
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
            BindOrders();
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }
    }
}