using Aircall.Common;
using Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class update_payment_method : System.Web.UI.Page
    {
        #region "Declaration"
        IClientService objClientService = ServiceFactory.ClientService;
        IClientPaymentMethodService objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
        IStripeErrorLogService objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["ClientLoginCookie"] != null)
                {
                    if (Request.QueryString["cid"] != null)
                    {
                        int RequestId;// = int.Parse(Request.QueryString["rid"].ToString());
                        if (!int.TryParse(Request.QueryString["cid"], out RequestId))
                        {
                            Response.Redirect("dashboard.aspx", false);
                        }
                        int ClientPaymentId = Convert.ToInt32(Request.QueryString["cid"].ToString());
                        DataTable dtCardInfo = new DataTable();
                        objClientPaymentMethodService.GetClientPaymentMethodById(ClientPaymentId, ref dtCardInfo);
                        if (dtCardInfo.Rows.Count > 0)
                        {
                            if (dtCardInfo.Rows[0]["CardType"].ToString() == "Visa")
                                rblVisa.Checked = true;
                            else if (dtCardInfo.Rows[0]["CardType"].ToString() == "MasterCard")
                                rblMaster.Checked = true;
                            else if (dtCardInfo.Rows[0]["CardType"].ToString() == "Discover")
                                rblDiscover.Checked = true;
                            else
                                rblAmex.Checked = true;

                            txtName.Text = dtCardInfo.Rows[0]["NameOnCard"].ToString();
                            txtCardNumber.Text = dtCardInfo.Rows[0]["CardNumber"].ToString();
                            txtMonth.Text = dtCardInfo.Rows[0]["ExpiryMonth"].ToString();
                            txtYear.Text = dtCardInfo.Rows[0]["ExpiryYear"].ToString();
                            chkIsDefault.Checked = Convert.ToBoolean(dtCardInfo.Rows[0]["IsDefaultPayment"].ToString());
                            
                            btnUpdate.Visible = true;
                            txtCardNumber.Enabled = false;                            
                        }
                    }
                    else
                    {
                        Response.Redirect("dashboard.aspx");
                    }
                }
                else
                    Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
        }
        #endregion

        #region "Events"        
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Request.QueryString["cid"] != null)
                    {
                        BizObjects.ClientPaymentMethod objClientPaymentMethod = new BizObjects.ClientPaymentMethod();
                        int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                        DataTable dtClient = new DataTable();
                        objClientService.GetClientById(ClientId, ref dtClient);

                        DataTable dtCardInfo = new DataTable();
                        objClientPaymentMethodService.GetClientPaymentMethodById(Convert.ToInt32(Request.QueryString["cid"].ToString()), ref dtCardInfo);

                        objClientPaymentMethod.Id = Convert.ToInt32(Request.QueryString["cid"].ToString());
                        objClientPaymentMethod.ClientId = ClientId;
                        if (rblVisa.Checked)
                            objClientPaymentMethod.CardType = "Visa";
                        else if (rblMaster.Checked)
                            objClientPaymentMethod.CardType = "MasterCard";
                        else if (rblDiscover.Checked)
                            objClientPaymentMethod.CardType = "Discover";
                        else
                            objClientPaymentMethod.CardType = "AMAX";
                        string cardStr = txtCardNumber.Text.Substring(txtCardNumber.Text.Trim().Length - 4);
                        objClientPaymentMethod.NameOnCard = txtName.Text.Trim();
                        //objClientPaymentMethod.CardNumber = cardStr.PadLeft(16, '*');
                        objClientPaymentMethod.ExpiryMonth = Convert.ToInt16(txtMonth.Text.Trim());
                        objClientPaymentMethod.ExpiryYear = Convert.ToInt32(txtYear.Text.Trim());
                        objClientPaymentMethod.IsDefaultPayment = chkIsDefault.Checked;
                        objClientPaymentMethod.UpdatedBy = ClientId;
                        objClientPaymentMethod.UpdatedByType = General.UserRoles.Client.GetEnumValue();
                        objClientPaymentMethod.UpdatedDate = DateTime.Now;
                        try
                        {
                            //stripe card update
                            var myCard = new StripeCardUpdateOptions();

                            myCard.Name = txtName.Text.Trim();
                            myCard.ExpirationYear = txtYear.Text.Trim();
                            myCard.ExpirationMonth = txtMonth.Text.Trim();

                            var cardService = new StripeCardService();
                            StripeCard stripeCard = cardService.Update(dtClient.Rows[0]["StripeCustomerId"].ToString(), dtCardInfo.Rows[0]["StripeCardId"].ToString(), myCard);
                            if (string.IsNullOrEmpty(stripeCard.Id))
                            {
                                dvMessage.InnerHtml = "Invalid card.";
                                dvMessage.Attributes.Add("class", "error");
                                dvMessage.Visible = true;
                                return;
                            }
                            //objClientPaymentMethod.StripeCardId = stripeCard.Id;
                        }
                        catch (StripeException stex)
                        {
                            BizObjects.StripeErrorLog objStripeErrorLog = new BizObjects.StripeErrorLog();
                            objStripeErrorLog.ChargeId = stex.StripeError.ChargeId;
                            objStripeErrorLog.Code = stex.StripeError.Code;
                            objStripeErrorLog.DeclineCode = stex.StripeError.DeclineCode;
                            objStripeErrorLog.ErrorType = stex.StripeError.ErrorType;
                            objStripeErrorLog.Error = stex.StripeError.Error;
                            objStripeErrorLog.ErrorSubscription = stex.StripeError.ErrorSubscription;
                            objStripeErrorLog.Message = stex.StripeError.Message;
                            objStripeErrorLog.Parameter = stex.StripeError.Parameter;
                            objStripeErrorLog.Userid = ClientId;
                            objStripeErrorLog.UnitId = 0;
                            objStripeErrorLog.DateAdded = DateTime.Now;

                            objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                            dvMessage.InnerHtml = stex.StripeError.Message.ToString();
                            dvMessage.Attributes.Add("class", "error");
                            dvMessage.Visible = true;
                            return;
                        }
                        objUserNotificationService.DeleteNotificationByCommonIdType(Convert.ToInt32(Request.QueryString["cid"].ToString()), General.NotificationType.CreditCardExpiration.GetEnumDescription());
                        objClientPaymentMethodService.UpdateClientPaymentMethod(ref objClientPaymentMethod);
                        dvMessage.InnerHtml = "Card saved Successfully.";
                        dvMessage.Attributes.Add("class", "success");
                        dvMessage.Visible = true;
                        Response.Redirect("dashboard.aspx");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
            }
        }
        #endregion
    }
}