using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class Order_View : System.Web.UI.Page
    {
        IOrderService objOrderService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["OrderId"]))
                {
                    GetOrderByOrderId();
                }
            }
        }

        private void GetOrderByOrderId()
        {
            int OrderId = Convert.ToInt32(Request.QueryString["OrderId"].ToString());
            DataTable dtOrder = new DataTable();
            objOrderService = ServiceFactory.OrderService;
            objOrderService.GetOrderById(OrderId, ref dtOrder);
            if (dtOrder.Rows.Count > 0)
            {
                ltrOrderNo.Text = dtOrder.Rows[0]["OrderNumber"].ToString();
                ltrAddedDate.Text = DateTime.Parse(dtOrder.Rows[0]["AddedDate"].ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt");
                ltrClientName.Text = dtOrder.Rows[0]["ClientName"].ToString();
                ltrEmail.Text = dtOrder.Rows[0]["Email"].ToString();
                ltrAddress.Text = dtOrder.Rows[0]["BillingAddress"].ToString();
                ltrState.Text = dtOrder.Rows[0]["StateName"].ToString();
                ltrCity.Text = dtOrder.Rows[0]["CityName"].ToString();
                ltrZip.Text = dtOrder.Rows[0]["BillingZipcode"].ToString();
                ltrEmployee.Text = dtOrder.Rows[0]["EmployeeName"].ToString();
                ltrAmount.Text = dtOrder.Rows[0]["OrderAmount"].ToString();
                ltrChargeBy.Text = dtOrder.Rows[0]["ChargeBy"].ToString();
                if (dtOrder.Rows[0]["ChargeBy"].ToString() == General.ChargeBy.Check.GetEnumDescription())
                {
                    ltrCheckNo.Text = dtOrder.Rows[0]["ChequeNo"].ToString();
                    ltrCheckDate.Text = Convert.ToDateTime(dtOrder.Rows[0]["ChequeDate"].ToString()).ToString("MM/dd/yyyy");
                    ltrAccNotes.Text = dtOrder.Rows[0]["AccountingNotes"].ToString();
                    if (!string.IsNullOrEmpty(dtOrder.Rows[0]["ChqueImageFront"].ToString()))
                    {
                        ltrFront.Text = dtOrder.Rows[0]["ChqueImageFront"].ToString();
                        ltrBack.Text = dtOrder.Rows[0]["ChequeImageBack"].ToString();
                        lnkFront.HRef = Application["SiteAddress"] + "uploads/checkImages/" + dtOrder.Rows[0]["ChqueImageFront"].ToString();
                        lnkBack.HRef = Application["SiteAddress"] + "uploads/checkImages/" + dtOrder.Rows[0]["ChequeImageBack"].ToString();    
                    }
                }
                else
                    dvCheck.Visible = false;
                ltrTransaction.Text = dtOrder.Rows[0]["TransactionId"].ToString();
                ltrRecommendation.Text = dtOrder.Rows[0]["CustomerRecommendation"].ToString();
                if (!string.IsNullOrEmpty(dtOrder.Rows[0]["ClientSignature"].ToString()))
                    imgSignature.ImageUrl = Application["SiteAddress"] + "uploads/clientSignature/" + dtOrder.Rows[0]["ClientSignature"].ToString();
                else
                    imgSignature.ImageUrl = Application["SiteAddress"] + "uploads/clientSignature/NoImage.png";
                
            }
            DataTable dtOrderItems = new DataTable();
            objOrderService.GetOrderItemByOrderId(OrderId, ref dtOrderItems);
            if (dtOrderItems.Rows.Count>0)
            {
                lstPartList.DataSource = dtOrderItems;
            }
            lstPartList.DataBind();
        }
    }
}