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
    public partial class BillingHistory_View : System.Web.UI.Page
    {
        IBillingHistoryService objBillingHistoryService;
        IOrderService objOrderService;
        IUserNotificationService objUserNotificationService;
        protected void Page_Load(object sender, EventArgs e)
        {
            objBillingHistoryService = ServiceFactory.BillingHistoryService;
            objUserNotificationService = ServiceFactory.UserNotificationService;
            if (!IsPostBack)
            {
                if (Request.QueryString["bid"] != null)
                {
                    int RequestId;
                    if (!int.TryParse(Request.QueryString["bid"], out RequestId))
                    {
                        Response.Redirect("billing-history.aspx", false);
                    }
                    int BillingId = int.Parse(Request.QueryString["bid"].ToString());
                    DataTable dt = new DataTable();
                    objBillingHistoryService.GetBillingHistoryById(BillingId, ref dt);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        var billingType = row["BillingType"].ToString();
                        ltrBillingType.Text = row["BillingType"].ToString();
                        this.ltrAccountingNotes.Text = row["AccountingNotes"].ToString();
                        this.divCardNumber.Visible = false;
                        this.divCheckNumbers.Visible = false;
                        this.divPONumber.Visible = false;
                        this.btnSave.Visible = false;
                        this.drpStatus.Enabled = false;
                        if (billingType == "Recurring Purchase")
                        {
                            this.divCardNumber.Visible = true;
                        }
                        else
                        {
                            this.divCheckNumbers.Visible = true;
                            this.btnSave.Visible = true;
                            this.drpStatus.Enabled = true;
                            if (billingType == "PO")
                            {
                                this.divPONumber.Visible = true;
                                this.ltrPO.Text = row["PO"].ToString();
                            }
                            string checkNumbers = row["CheckNumbers"].ToString();
                            string checkAmounts = row["CheckNumbers"].ToString();
                            FillChecks(checkNumbers, checkAmounts);
                        }
                        var clientUnitIds = row["ClientUnitIds"].ToString();
                        FillUnits(clientUnitIds);
                        ltrServiceNo.Text = row["ServiceCaseNumber"].ToString();
                        ltrOrderNo.Text = row["OrderNumber"].ToString();
                        ltrPlan.Text = row["PackageName"].ToString();
                        //ltrUnit.Text = row["UnitName"].ToString();
                        ltrPaymentByNumber.Text = row["CardNumber"].ToString();
                        ltrPaymentMethod.Text = row["PaymentMethod"].ToString();
                        ltrPaymentMethod.Text = "Card Number";
                        ltrTransactionId.Text = row["TransactionId"].ToString();
                        if (string.IsNullOrEmpty(ltrTransactionId.Text))
                        {
                            ltrTransactionId.Text = row["InvoiceNumber"].ToString();
                        }
                        drpStatus.SelectedValue = Convert.ToBoolean(row["IsPaid"]) ? "Paid" : "UnPaid";
                        this.hdfOriginalStatus.Value = drpStatus.SelectedValue;
                        this.hdfClientId.Value = row["ClientId"].ToString();
                        this.hdfClientName.Value = row["ClientName"].ToString();
                        this.ltrClientName.Text = row["ClientName"].ToString();
                        this.hdfCompany.Value = row["Company"].ToString();
                        this.ltrCompany.Text = row["Company"].ToString();
                        this.hdfCheckNumbers.Value = row["CheckNumbers"].ToString();
                        this.hdfTransactionDate.Value = Convert.ToDateTime(row["TransactionDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        //Code Commented on 19-07-2017
                        //ltrDate.Text = DateTime.Parse(row["TransactionDate"].ToString()).ToLocalTime().ToString("MMMM dd, yyyy");
                        //ltrTime.Text = DateTime.Parse(row["TransactionDate"].ToString()).ToLocalTime().ToString("hh:mm tt");

                        ltrDate.Text = DateTime.Parse(row["TransactionDate"].ToString()).ToString("MMMM dd, yyyy");
                        ltrTime.Text = DateTime.Parse(row["TransactionDate"].ToString()).ToString("hh:mm tt");

                        ltrAmount.Text = "$ " + row["PurchasedAmount"].ToString();

                        objOrderService = ServiceFactory.OrderService;
                        DataTable dtOrderItem = new DataTable();

                        int OrderId = int.Parse(row["OrderId"].ToString());

                        objOrderService.GetOrderItemByOrderId(OrderId, ref dtOrderItem);

                        if (dtOrderItem.Rows.Count > 0)
                        {
                            lstParts.DataSource = dtOrderItem;
                            lstParts.DataBind();
                        }
                        if (row["PackageName"].ToString().Contains("Part Order"))
                        {
                            dvPart.Visible = true;
                            dvPart1.Visible = true;
                            dvNoShow.Visible = false;
                            dvUnit.Visible = false;
                            dvUnit1.Visible = false;
                        }
                        else if (row["PackageName"].ToString().Contains("No Show"))
                        {
                            dvPart.Visible = false;
                            dvPart1.Visible = false;
                            dvNoShow.Visible = true;
                            dvUnit.Visible = false;
                            dvUnit1.Visible = false;
                        }
                        else
                        {
                            dvPart.Visible = false;
                            dvPart1.Visible = false;
                            dvNoShow.Visible = false;
                            //dvUnit.Visible = true;
                            //dvUnit1.Visible = true;
                        }
                    }
                }
                else
                {
                    Response.Redirect("BillingHistory_List.aspx");
                }
            }
        }

        private void FillUnits(string clientUnitIds)
        {
            var objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dt = null;
            objClientUnitService.GetUnits(clientUnitIds, ref dt);
            this.lstUnits.DataSource = dt;
            this.lstUnits.DataBind();
        }

        private void FillChecks(string checkNumbers, string checkAmounts)
        {
            if (!string.IsNullOrEmpty(checkNumbers))
            {
                var arr1 = checkNumbers.Split(',');
                var arr2 = checkNumbers.Split(',');
                var list = new List<CheckItem>();
                for (int i = 0; i < arr1.Length; i++)
                {
                    var checkNumber = arr1[i];
                    var checkAmount = Convert.ToDecimal(arr2[i]);
                    list.Add(new CheckItem { Sr = i + 1, CheckNumber = checkNumber, Amount = checkAmount });
                }
                for (var i = arr1.Length; i < 5; i++)
                {
                    var checkNumber = "";
                    decimal? checkAmount = null;
                    list.Add(new CheckItem { Sr = i + 1, CheckNumber = checkNumber, Amount = checkAmount });
                }
                this.lstChecks.DataSource = list;
                this.lstChecks.DataBind();
            }
            else
            {
                var list = new List<CheckItem>();
                for (var i = 0; i < 5; i++)
                {
                    var checkNumber = "";
                    decimal? checkAmount = null;
                    list.Add(new CheckItem { Sr = i + 1, CheckNumber = checkNumber, Amount = checkAmount });
                }
                this.lstChecks.DataSource = list;
                this.lstChecks.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var faildesc = "";
            var sendEmail = false;
            if (this.drpStatus.SelectedValue == "Paid")
            {
                faildesc = "Payment Success!";
            }
            else
            {
                faildesc = "Payment Failed!";
                if (this.hdfOriginalStatus.Value == "Paid")
                {
                    sendEmail = true;
                }
            }
            var sql = string.Format("update BillingHistory set IsPaid = '{0}', faildesc = '{2}' where Id = {1}", this.drpStatus.SelectedValue == "Paid" ? "1" : "0", Request.QueryString["bid"], faildesc);
            var helper = new DBUtility.SQLDBHelper();
            helper.ExecuteSQL(sql, null);
            helper.Close();
            if (sendEmail)
            {
                SendEmailToAdmin();
            }
            Response.Redirect("BillingHistory_List.aspx");
        }

        private void SendEmailToAdmin()
        {
            try
            {
                var subject = "Payment Failed";
                var body = "";
                var path = AppDomain.CurrentDomain.BaseDirectory + "EmailTemplates\\PaymentFailed.html";
                using (var sr = new System.IO.StreamReader(path))
                {
                    body = sr.ReadToEnd();
                    sr.Close();
                }
                body = body.Replace("{{ClientName}}", this.hdfClientName.Value);
                body = body.Replace("{{Company}}", this.hdfCompany.Value);
                body = body.Replace("{{PaymentMethod}}", this.ltrBillingType.Text);
                body = body.Replace("{{Amount}}", this.ltrAmount.Text);
                body = body.Replace("{{TransactionTime}}", this.hdfTransactionDate.Value);
                var additionalInformation = "";
                if (this.ltrPO.Text != "")
                {
                    additionalInformation += "PO: " + this.ltrPO.Text;
                }
                if (additionalInformation != "")
                {
                    additionalInformation += " ";
                }
                additionalInformation += "Check Numbers: " + this.hdfCheckNumbers.Value;
                body = body.Replace("{{AdditionalInformation}}", additionalInformation);
                body = body.Replace("{{Status}}", "UnPaid");
                var objSiteSettingService = ServiceFactory.SiteSettingService;
                DataTable dtSiteSetting = new DataTable();
                objSiteSettingService.GetSiteSettingByName("AdminEmail", ref dtSiteSetting);
                var to = dtSiteSetting.Rows[0]["Value"].ToString();
                var cc = "";
                Email.SendEmail(subject, to, cc, "", body);
            }
            catch (Exception ex)
            {

            }
        }
    }
}