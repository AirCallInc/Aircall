using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;
using Stripe;

namespace Aircall.admin
{
    public partial class ChangePaymentMethod : System.Web.UI.Page
    {
        IClientPaymentMethodService objClientPaymentMethodService;
        IClientUnitSubscriptionService objClientUnitSubscriptionService;
        IClientService objClientService;
        IStripeErrorLogService objStripeErrorLogService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMonthYearDropdown();
                if (!string.IsNullOrEmpty(Request.QueryString["SubId"]))
                {
                    DataTable dtSubscription = new DataTable(); 
                    Int64 Id = Convert.ToInt64(Request.QueryString["SubId"].ToString());
                    objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
                    objClientUnitSubscriptionService.GetClientUnitSubscriptionById(Id, ref dtSubscription);
                    if (dtSubscription.Rows.Count>0)
                    {
                        ltrClientName.Text = dtSubscription.Rows[0]["ClientName"].ToString();
                        hdnClientId.Value = dtSubscription.Rows[0]["Id"].ToString();
                        ltrUnit.Text = dtSubscription.Rows[0]["UnitName"].ToString();
                        hdnUnitId.Value = dtSubscription.Rows[0]["UnitId"].ToString(); 
                        ltrDueDate.Text = Convert.ToDateTime(dtSubscription.Rows[0]["PaymentDueDate"].ToString()).ToString("MM/dd/yyyy");
                        ltrCurrentPaymentMethod.Text = dtSubscription.Rows[0]["PaymentMethod"].ToString();
                        int CCCount = Convert.ToInt32(dtSubscription.Rows[0]["CCCount"].ToString());

                        var values = DurationExtensions.GetValues<General.PaymentMethod>();
                        List<string> data = new List<string>();
                        foreach (var item in values)
                        {
                            General.PaymentMethod p = (General.PaymentMethod)item;
                            //if (p.GetEnumDescription() == General.PaymentMethod.CC.GetEnumDescription())
                            //{
                            //    if (p.GetEnumDescription()!=dtSubscription.Rows[0]["PaymentMethod"].ToString() && CCCount > 0)
                            //    {
                            //        data.Add(p.GetEnumDescription());
                            //        continue;
                            //    }
                                    
                            //    else
                            //        continue;
                            //}
                            if (p.GetEnumDescription()!=dtSubscription.Rows[0]["PaymentMethod"].ToString())
                            {
                                data.Add(p.GetEnumDescription());
                            }
                        }
                        drpPayment.DataSource = data;
                        drpPayment.DataBind();
                        drpPayment.Items.Insert(0, new ListItem("Select", "0"));
                    }
                }
            }
        }

        private void BindMonthYearDropdown()
        {
            drpMonth.Enabled = true;
            drpYear.Enabled = true;

            DataTable dtMonth = new DataTable();
            dtMonth.Columns.Add("Month");
            for (int i = 01; i <= 12; i++)
            {
                dtMonth.Rows.Add(dtMonth.NewRow());
                dtMonth.Rows[dtMonth.Rows.Count - 1]["Month"] = i.ToString("00");
            }
            drpMonth.DataSource = dtMonth;
            drpMonth.DataTextField = "Month";
            drpMonth.DataValueField = "Month";
            drpMonth.DataBind();

            DataTable dtYear = new DataTable();
            dtYear.Columns.Add("Year");
            for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 20; i++)
            {
                dtYear.Rows.Add(dtYear.NewRow());
                dtYear.Rows[dtYear.Rows.Count - 1]["Year"] = i.ToString();
            }
            drpYear.DataSource = dtYear;
            drpYear.DataTextField = "Year";
            drpYear.DataValueField = "Year";
            drpYear.DataBind();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["SubId"]) && !string.IsNullOrEmpty(hdnUnitId.Value))
                    {
                        int UnitId = Convert.ToInt32(hdnUnitId.Value);
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;
                        string StripeCustomerId = string.Empty;
                        int CardId = 0;

                        if (drpPayment.SelectedValue.ToString()==General.PaymentMethod.CC.GetEnumDescription())
                        {
                            int ClientId = 0;

                            ClientId = Convert.ToInt32(hdnClientId.Value);
                            DataTable dtClient = new DataTable();
                            objClientService = ServiceFactory.ClientService;
                            objClientService.GetClientById(ClientId, ref dtClient);
                            if (dtClient.Rows.Count > 0)
                                StripeCustomerId = dtClient.Rows[0]["StripeCustomerId"].ToString();

                            //Add New Client Card Start
                            objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
                            if (drpCard.SelectedValue == "-1")
                            {
                                BizObjects.ClientPaymentMethod objClientPaymentMethod = new BizObjects.ClientPaymentMethod();

                                objClientPaymentMethod.ClientId = ClientId;
                                if (rblVisa.Checked)
                                    objClientPaymentMethod.CardType = "Visa";
                                else if (rblMaster.Checked)
                                    objClientPaymentMethod.CardType = "MasterCard";
                                else if (rblDiscover.Checked)
                                    objClientPaymentMethod.CardType = "Discover";
                                else
                                    objClientPaymentMethod.CardType = "AmericanExpress";

                                string cardStr = txtCardNumber.Text.Substring(txtCardNumber.Text.Trim().Length - 4);
                                objClientPaymentMethod.NameOnCard = txtCardName.Text.Trim();
                                objClientPaymentMethod.CardNumber = cardStr.PadLeft(16, '*');
                                objClientPaymentMethod.ExpiryMonth = Convert.ToInt16(drpMonth.SelectedValue.ToString());//Convert.ToInt16(txtMonth.Text.Trim());
                                objClientPaymentMethod.ExpiryYear = Convert.ToInt32(drpYear.SelectedValue.ToString());//Convert.ToInt32(txtYear.Text.Trim());
                                objClientPaymentMethod.IsDefaultPayment = false;
                                objClientPaymentMethod.AddedBy = ClientId;
                                objClientPaymentMethod.AddedByType = General.UserRoles.Client.GetEnumValue();
                                objClientPaymentMethod.AddedDate = DateTime.UtcNow;

                                try
                                {
                                    var customerService = new StripeCustomerService();
                                    var myCustomer = customerService.Get(StripeCustomerId);

                                    // setting up the card
                                    var myCard = new StripeCardCreateOptions();

                                    myCard.SourceCard = new SourceCard()
                                    {
                                        Number = txtCardNumber.Text.Trim(),
                                        ExpirationYear = drpYear.SelectedValue.ToString(), //txtYear.Text.Trim(),
                                        ExpirationMonth = drpMonth.SelectedValue.ToString(), //txtMonth.Text.Trim(),
                                        Name = txtCardName.Text.Trim(),
                                        Cvc = txtCVV.Text.Trim()
                                    };

                                    var cardService = new StripeCardService();
                                    StripeCard stripeCard = cardService.Create(StripeCustomerId, myCard);
                                    if (string.IsNullOrEmpty(stripeCard.Id))
                                    {
                                        dvMessage.InnerHtml = "Invalid card.";
                                        dvMessage.Attributes.Add("class", "alert alert-error");
                                        dvMessage.Visible = true;
                                        return;
                                    }
                                    //objClientPaymentMethod.StripeCardId = stripeCard.Id;
                                }
                                catch (StripeException stex)
                                {
                                    BizObjects.StripeErrorLog objStripeErrorLog = new BizObjects.StripeErrorLog();
                                    objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
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
                                    objStripeErrorLog.DateAdded = DateTime.UtcNow;

                                    objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);

                                    dvMessage.InnerHtml = stex.StripeError.Message.ToString();
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                                CardId = objClientPaymentMethodService.AddClientPaymentMethod(ref objClientPaymentMethod);
                            }
                            else
                                CardId = Convert.ToInt32(drpCard.SelectedValue.ToString());
                            //Add New Client Card End
                        }

                        objClientUnitSubscriptionService = ServiceFactory.ClientUnitSubscriptionService;
                        objClientUnitSubscriptionService.UpdatePaymentMethodByUnitId(UnitId, drpPayment.SelectedValue.ToString(), DateTime.UtcNow, CardId,objLoginModel.Id, objLoginModel.RoleId);
                        Session["msg"] = "PChange";
                        Redirect("PChange");
                        //Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List.aspx?msg=PChange");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>" + Ex.Message.ToString().Trim() + "</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void drpPayment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpPayment.SelectedValue.ToString() == General.PaymentMethod.CC.GetEnumDescription()
                && !string.IsNullOrEmpty(hdnClientId.Value))
            {
                dvCard.Visible = true;
                FillClientPaymentCard(Convert.ToInt32(hdnClientId.Value));
            }
            else
                dvCard.Visible = false;
        }

        private void FillClientPaymentCard(int ClientId)
        {
            drpCard.DataSource = "";
            drpCard.DataBind();

            drpCard.Items.Insert(0, new ListItem("Select Card", "0"));
            drpCard.Items.Insert(1, new ListItem("Enter New Card", "-1"));

            objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
            DataTable dtPaymentMethods = new DataTable();
            objClientPaymentMethodService.GetClientPaymentMethodByClientId(ClientId, ref dtPaymentMethods);
            if (dtPaymentMethods.Rows.Count > 0)
            {
                drpCard.DataSource = dtPaymentMethods;
                drpCard.DataTextField = dtPaymentMethods.Columns["CardNumber"].ToString();
                drpCard.DataValueField = dtPaymentMethods.Columns["Id"].ToString();
                drpCard.DataBind();
                drpCard.Items.Insert(0, new ListItem("Select Card", "0"));
                drpCard.Items.Insert(dtPaymentMethods.Rows.Count + 1, new ListItem("Enter New Card", "-1"));
            }
        }

        protected void drpCard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpCard.SelectedValue.ToString() == "-1")
                dvNewCard.Visible = true;
            else
                dvNewCard.Visible = false;
        }

        public void Redirect(string msg)
        {
            if (Session["SubscriptionSearch"] != null)
            {
                SubscriptionSearch objSubscriptionSearch = new SubscriptionSearch();
                objSubscriptionSearch = Session["SubscriptionSearch"] as SubscriptionSearch;

                string Param = string.Empty;
                if (!string.IsNullOrEmpty(objSubscriptionSearch.ClientName))
                    Param = "&CName=" + objSubscriptionSearch.ClientName;

                if (!string.IsNullOrEmpty(objSubscriptionSearch.StartDate.ToString()))
                {
                    if (!string.IsNullOrEmpty(Param))
                        Param = Param + "&SDate=" + objSubscriptionSearch.StartDate.ToString("MM/dd/yyyy");
                    else
                        Param = "&SDate=" + objSubscriptionSearch.StartDate.ToString("MM/dd/yyyy");
                }
                if (!string.IsNullOrEmpty(objSubscriptionSearch.EndDate.ToString()))
                {
                    if (!string.IsNullOrEmpty(Param))
                        Param = Param + "&EDate=" + objSubscriptionSearch.EndDate.ToString("MM/dd/yyyy");
                    else
                        Param = "&EDate=" + objSubscriptionSearch.EndDate.ToString("MM/dd/yyyy");
                }

                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&Status=" + objSubscriptionSearch.Status.ToString();
                else
                    Param = "&Status=" + objSubscriptionSearch.Status.ToString();

                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&PMethod=" + objSubscriptionSearch.PaymentMethod.ToString();
                else
                    Param = "&PMethod=" + objSubscriptionSearch.PaymentMethod.ToString();


                Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List.aspx?msg=" + msg + Param);
            }
            else
                Response.Redirect(Application["SiteAddress"] + "admin/ClientUnitSubscription_List.aspx?msg=" + msg);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Redirect("cancel");
        }
    }
}