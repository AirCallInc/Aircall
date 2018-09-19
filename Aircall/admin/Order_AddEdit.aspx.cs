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
using System.IO;
using System.Net;
using System.Text;

namespace Aircall.admin
{
    public partial class Order_AddEdit : System.Web.UI.Page
    {
        //IEmployeeService objEmployeeService;
        IClientService objClientService;
        IStateService objStateService;
        ICitiesService objCitiesService;
        IClientAddressService objClientAddressService;
        IPartsService objPartsService;
        IOrderService objOrderService;
        IOrderItemsService objOrderItemsService;
        IBillingHistoryService objBillingHistoryService;
        IStripeErrorLogService objStripeErrorLogService;
        IClientPaymentMethodService objClientPaymentMethodService;

        public static DataTable dtParts = new DataTable("Parts");
        public bool AllowCConFile;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMonthYearDropdown();
                AllowCConFile = true;
                dtParts.Rows.Clear();
                dtParts.Columns.Clear();
                dtParts.Columns.Add("Id");
                dtParts.Columns.Add("PartId");
                dtParts.Columns.Add("PartName");
                dtParts.Columns.Add("Quantity");
                dtParts.Columns.Add("Price");

                dtParts.AcceptChanges();

                FillStateDropdown();
                drpCity.Items.Insert(0, new ListItem("Select City", "0"));

                BindChargeByDropdown();
                //if (!string.IsNullOrEmpty(Request.QueryString["OrderId"]))
                //{
                //    int OrderId = Convert.ToInt32(Request.QueryString["OrderId"].ToString());
                //    DataTable dtOrders = new DataTable();
                //    objOrderService = ServiceFactory.OrderService;
                //    objOrderService.GetOrderById(OrderId, ref dtOrders);
                //    if (dtOrders.Rows.Count>0)
                //    {
                //        ltrSubtotal.Text = dtOrders.Rows[0]["OrderAmount"].ToString();
                //        //txtEmployee.Text = dtOrders.Rows[0]["EmployeeName"].ToString();
                //        txtClient.Text = dtOrders.Rows[0]["ClientName"].ToString();
                //        BindClientByName(dtOrders.Rows[0]["ClientName"].ToString());
                //        txtNotes.Text = dtOrders.Rows[0]["CustomerRecommendation"].ToString();
                //    }


                //    DataTable dtOrderItems = new DataTable();
                //    objOrderService.GetOrderItemByOrderId(OrderId, ref dtOrderItems);
                //    foreach (DataRow dr in dtOrderItems.Rows)
                //    {
                //        DataRow dr1 = dtParts.NewRow();
                //        dr1["Id"] = dtParts.Rows.Count + 1;
                //        dr1["PartId"] = dr["PartId"].ToString();
                //        dr1["PartName"] = dr["PartName"].ToString() + " - " + dr["PartSize"].ToString();
                //        dr1["Quantity"] = dr["Quantity"].ToString();
                //        dr1["Price"] = dr["Amount"].ToString();
                //        dtParts.Rows.Add(dr1);
                //    }
                //}
                lstParts.DataSource = dtParts;
                lstParts.DataBind();
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

        private void BindChargeByDropdown()
        {
            drpCharge.DataSource = "";
            drpCharge.DataBind();
            var values = DurationExtensions.GetValues<General.ChargeBy>();
            List<string> data = new List<string>();
            foreach (var item in values)
            {
                General.ChargeBy p = (General.ChargeBy)item;
                if (p.GetEnumDescription() == General.ChargeBy.CCOnFile.GetEnumDescription() && AllowCConFile == false)
                {
                    continue;
                }
                data.Add(p.GetEnumDescription());
            }
            drpCharge.DataSource = data;
            drpCharge.DataBind();
            drpCharge.Items.Insert(0, new ListItem("Select", "0"));
        }

        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtStates = new DataTable();
            objStateService.GetAllStates(true, true, ref dtStates);
            if (dtStates.Rows.Count > 0)
            {
                drpState.DataSource = dtStates;
                drpState.DataTextField = dtStates.Columns["Name"].ToString();
                drpState.DataValueField = dtStates.Columns["Id"].ToString();
            }
            drpState.DataBind();
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
        }

        private void BindCityFromState(int StateId)
        {
            objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCities = new DataTable();
            objCitiesService.GetAllCityByStateId(StateId, true, ref dtCities);
            if (dtCities.Rows.Count > 0)
            {
                drpCity.DataSource = dtCities;
                drpCity.DataValueField = dtCities.Columns["Id"].ToString();
                drpCity.DataTextField = dtCities.Columns["Name"].ToString();
            }
            else
                drpCity.DataSource = "";

            drpCity.DataBind();
            drpCity.Items.Insert(0, new ListItem("Select City", "0"));
        }

        //protected void lnkSearch_Click(object sender, EventArgs e)
        //{
        //    rblEmployee.DataSource = "";
        //    rblEmployee.DataBind();
        //    DataTable dtEmployee = new DataTable();
        //    objEmployeeService = ServiceFactory.EmployeeService;
        //    objEmployeeService.GetEmployeeByName(txtEmployee.Text.Trim(), false, ref dtEmployee);
        //    if (dtEmployee.Rows.Count > 0)
        //    {
        //        rblEmployee.DataSource = dtEmployee;
        //        rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
        //        rblEmployee.DataValueField = dtEmployee.Columns["Id"].ToString();
        //        rblEmployee.DataBind();
        //    }
        //}

        protected void lnkClientSearch_Click(object sender, EventArgs e)
        {
            BindClientByName(txtClient.Text.Trim());
        }

        private void BindClientByName(string ClientName)
        {
            rblClient.DataSource = "";
            rblClient.DataBind();
            DataTable dtClient = new DataTable();
            objClientService = ServiceFactory.ClientService;
            objClientService.GetClientByName(ClientName, ref dtClient);
            if (dtClient.Rows.Count > 0)
            {
                rblClient.DataSource = dtClient;
                rblClient.DataTextField = dtClient.Columns["ClientName"].ToString();
                rblClient.DataValueField = dtClient.Columns["Id"].ToString();
                rblClient.DataBind();
            }
        }

        protected void drpState_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCityFromState(Convert.ToInt32(drpState.SelectedValue.ToString()));
        }

        protected void rblClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtEmail.Text = "";
            objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtClientAddress = new DataTable();
            objClientAddressService.GetClientDefaultAddressByClientId(Convert.ToInt32(rblClient.SelectedValue.ToString()), ref dtClientAddress);
            if (dtClientAddress.Rows.Count > 0)
            {
                txtAddress.Text = dtClientAddress.Rows[0]["Address"].ToString();
                drpState.SelectedValue = dtClientAddress.Rows[0]["State"].ToString();
                BindCityFromState(Convert.ToInt32(dtClientAddress.Rows[0]["State"].ToString()));
                drpCity.SelectedValue = dtClientAddress.Rows[0]["City"].ToString();
                txtZip.Text = dtClientAddress.Rows[0]["ZipCode"].ToString();
            }
            objClientService = ServiceFactory.ClientService;
            DataTable dtClient = new DataTable();
            objClientService.GetClientById(Convert.ToInt32(rblClient.SelectedValue.ToString()), ref dtClient);
            if (dtClient.Rows.Count > 0)
                txtEmail.Text = dtClient.Rows[0]["Email"].ToString();

            objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;
            DataTable dtPaymentMethod = new DataTable();
            objClientPaymentMethodService.GetClientPaymentMethodByClientId(Convert.ToInt32(rblClient.SelectedValue.ToString()), ref dtPaymentMethod);
            if (dtPaymentMethod.Rows.Count == 0)
                AllowCConFile = false;
            else
                AllowCConFile = true;
            BindChargeByDropdown();

        }

        protected void lnkParts_Click(object sender, EventArgs e)
        {
            objPartsService = ServiceFactory.PartsService;
            DataTable dtParts = new DataTable();
            objPartsService.GetPartByPartName(txtPart.Text.Trim(), ref dtParts);
            if (dtParts.Rows.Count > 0)
            {
                rblParts.DataSource = dtParts;
                rblParts.DataTextField = dtParts.Columns["DisplayName"].ToString();
                rblParts.DataValueField = dtParts.Columns["Id"].ToString();
            }
            rblParts.DataBind();
        }

        protected void lnkAddPart_Click(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            if (rblParts.Items.Count == 0 || rblParts.SelectedIndex == -1)
            {
                dvMessage.InnerHtml = "<strong>Please Select items.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return;
            }
            bool PartFound = false;
            if (dtParts.Rows.Count > 0)
            {
                var rows = dtParts.Select("PartId=" + rblParts.SelectedValue.ToString());
                if (rows.Count() > 0)
                {
                    //foreach (DataRow row in dtParts.Rows)
                    //{
                    //if (row["PartId"].ToString() == rblParts.SelectedValue.ToString())
                    //{
                    PartFound = true;
                    dtParts.Rows[int.Parse(rows[0]["Id"].ToString()) - 1]["Quantity"] = (Convert.ToInt32(dtParts.Rows[int.Parse(rows[0]["Id"].ToString()) - 1]["Quantity"].ToString()) + Convert.ToInt32(txtQty.Text.Trim())).ToString();
                    //}
                    //}
                }

                //DataTable dtFoundPart = new DataTable();
                //DataView dv = new DataView(dtParts, "PartId=" + rblParts.SelectedValue.ToString(), "Id", DataViewRowState.CurrentRows);
                //dtFoundPart = dv.ToTable();
                //if (dtFoundPart.Rows.Count > 0)
                //{
                //    int Id = Convert.ToInt32(dtFoundPart.Rows[0]["Id"].ToString());
                //    PartFound = true;
                //    DataTable dtNewParts = new DataTable();
                //    dtNewParts = dtParts.Copy();
                //    dtParts.Rows.Clear();
                //    foreach (DataRow dr in dtNewParts.Rows)
                //    {
                //        DataRow dr1 = dtParts.NewRow();
                //        dr1["Id"] = dtParts.Rows.Count + 1;
                //        dr1["PartId"] = dr["PartId"].ToString();
                //        dr1["PartName"] = dr["PartName"].ToString();
                //        if (dr["Id"].ToString() == Id.ToString())
                //            dr1["Quantity"] = (Convert.ToInt32(dr["Quantity"].ToString()) + Convert.ToInt32(txtQty.Text.Trim())).ToString();
                //        else
                //            dr1["Quantity"] = dr["Quantity"].ToString();
                //        dr1["Price"] = dr["Price"].ToString();
                //        dtParts.Rows.Add(dr1);
                //    }
                //}
            }

            if (PartFound == false)
            {
                objPartsService = ServiceFactory.PartsService;
                DataTable dtPart = new DataTable();
                objPartsService.GetPartById(Convert.ToInt32(rblParts.SelectedValue.ToString()), ref dtPart);
                if (dtPart.Rows.Count > 0)
                {
                    DataRow dr = dtParts.NewRow();
                    dr["Id"] = dtParts.Rows.Count + 1;
                    dr["PartId"] = rblParts.SelectedValue.ToString();
                    dr["PartName"] = dtPart.Rows[0]["Name"].ToString() + " - " + dtPart.Rows[0]["Size"].ToString();
                    dr["Quantity"] = txtQty.Text.Trim();
                    dr["Price"] = dtPart.Rows[0]["SellingPrice"].ToString();
                    dtParts.Rows.Add(dr);
                }
            }
            lstParts.DataSource = dtParts;
            lstParts.DataBind();

            decimal Subtotal = 0;
            foreach (DataRow dr in dtParts.Rows)
            {
                Subtotal = Subtotal + (Convert.ToInt32(dr["Quantity"].ToString()) * Convert.ToDecimal(dr["Price"].ToString()));
            }
            ltrSubtotal.Text = Subtotal.ToString();

            txtPart.Text = "";
            rblParts.DataSource = "";
            rblParts.DataBind();
            txtQty.Text = "";
        }

        protected void drpCharge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpCharge.SelectedValue.ToString() == General.ChargeBy.Check.GetEnumDescription())
                dvCheck.Visible = true;
            else
                dvCheck.Visible = false;

            if (drpCharge.SelectedValue.ToString() == General.ChargeBy.NewCC.GetEnumDescription())
                dvCard.Visible = true;
            else
                dvCard.Visible = false;
        }

        protected void lstParts_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemovePart" && e.CommandArgument.ToString() != "0")
            {
                int Id = Convert.ToInt32(e.CommandArgument.ToString());
                dtParts.Rows.RemoveAt(Id - 1);
                for (int i = 0; i < dtParts.Rows.Count; i++)
                {
                    dtParts.Rows[i]["Id"] = (i + 1).ToString();
                }
            }
            if (e.CommandName == "UpdatePart" && e.CommandArgument.ToString() != "0")
            {
                int Id = Convert.ToInt32(e.CommandArgument.ToString());
                TextBox txtQty = (TextBox)e.Item.FindControl("txtQty");
                dtParts.Rows[Id - 1]["Quantity"] = txtQty.Text;
                //dtParts.AcceptChanges();
            }
            //DataTable dtNewParts = new DataTable();
            //dtNewParts = dtParts.Copy();
            //dtNewParts.AcceptChanges();
            //dtParts.Rows.Clear();
            //dtParts.AcceptChanges();
            //foreach (DataRow dr in dtNewParts.Rows)
            //{
            //    DataRow dr1 = dtParts.NewRow();
            //    dr1["Id"] = dtParts.Rows.Count + 1;
            //    dr1["PartId"] = dr["PartId"].ToString();
            //    dr1["PartName"] = dr["PartName"].ToString();
            //    dr1["Quantity"] = dr["Quantity"].ToString();
            //    dr1["Price"] = dr["Price"].ToString();
            //    dtParts.Rows.Add(dr1);
            //}
            lstParts.DataSource = dtParts;
            lstParts.DataBind();

            decimal Subtotal = 0;
            foreach (DataRow dr in dtParts.Rows)
            {
                Subtotal = Subtotal + (Convert.ToInt32(dr["Quantity"].ToString()) * Convert.ToDecimal(dr["Price"].ToString()));
            }
            ltrSubtotal.Text = Subtotal.ToString();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Session["LoginSession"] != null)
                {
                    string frontImageName = string.Empty;
                    string backImageName = string.Empty;
                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;
                    string filenameOriginal = string.Empty;
                    objClientPaymentMethodService = ServiceFactory.ClientPaymentMethodService;

                    if (dtParts.Rows.Count == 0)
                    {
                        dvMessage.InnerHtml = "<strong>Please Insert Parts.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }

                    //if (rblEmployee.Items.Count == 0 || rblEmployee.SelectedIndex == -1)
                    //{
                    //    dvMessage.InnerHtml = "<strong>Please Select Employee.</strong>";
                    //    dvMessage.Attributes.Add("class", "alert alert-error");
                    //    dvMessage.Visible = true;
                    //    return;
                    //}
                    if (rblClient.Items.Count == 0 || rblClient.SelectedIndex == -1)
                    {
                        dvMessage.InnerHtml = "<strong>Please Select Client.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-error");
                        dvMessage.Visible = true;
                        return;
                    }

                    if (fpUpdSignature.HasFile)
                    {
                        string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                        foreach (var item in fpUpdSignature.PostedFiles)
                        {
                            if (!AllowedFileExtensions.Contains(item.FileName.Substring(item.FileName.LastIndexOf('.'))))
                            {
                                dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }
                        foreach (var item in fpUpdSignature.PostedFiles)
                        {
                            filenameOriginal = DateTime.UtcNow.Ticks.ToString().Trim() + System.IO.Path.GetExtension(item.FileName);

                            string filePath = Path.Combine(Server.MapPath("~/uploads/clientSignature/"), filenameOriginal);
                            item.SaveAs(filePath);
                        }
                    }
                    //else
                    //{
                    //    dvMessage.InnerHtml = "<strong>Please upload Signature file.</strong>";
                    //    dvMessage.Attributes.Add("class", "alert alert-error");
                    //    dvMessage.Visible = true;
                    //    return;
                    //}

                    if (fpFront.HasFile)
                    {
                        if (fpBack.HasFile)
                        {
                            string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                            if (!AllowedFileExtensions.Contains(fpFront.FileName.Substring(fpFront.FileName.LastIndexOf('.')))
                                && !AllowedFileExtensions.Contains(fpBack.FileName.Substring(fpBack.FileName.LastIndexOf('.'))))
                            {
                                dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                            else
                            {
                                frontImageName = DateTime.UtcNow.Ticks.ToString().Trim() + "-front" + System.IO.Path.GetExtension(fpFront.FileName);
                                fpFront.PostedFile.SaveAs(Path.Combine(Server.MapPath("~/uploads/checkImages/"), frontImageName));

                                backImageName = DateTime.UtcNow.Ticks.ToString().Trim() + "-back" + System.IO.Path.GetExtension(fpBack.FileName);
                                fpFront.PostedFile.SaveAs(Path.Combine(Server.MapPath("~/uploads/checkImages/"), backImageName));
                            }
                        }
                        else
                        {
                            dvMessage.InnerHtml = "<strong>Plaese Upload Back Image.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                    }


                    string StripeCardId = string.Empty;
                    //Add New Client Card Start
                    if (drpCharge.SelectedValue == General.ChargeBy.NewCC.GetEnumDescription())
                    {
                        BizObjects.ClientPaymentMethod objClientPaymentMethod = new BizObjects.ClientPaymentMethod();
                        int ClientId = Convert.ToInt32(rblClient.SelectedValue.ToString());
                        DataTable dtClient = new DataTable();
                        objClientService = ServiceFactory.ClientService;
                        objClientService.GetClientById(ClientId, ref dtClient);

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
                        objClientPaymentMethod.ExpiryMonth = Convert.ToInt16(drpMonth.SelectedValue.ToString()); //Convert.ToInt16(txtMonth.Text.Trim());
                        objClientPaymentMethod.ExpiryYear = Convert.ToInt32(drpYear.SelectedValue.ToString());//Convert.ToInt32(txtYear.Text.Trim());
                        objClientPaymentMethod.IsDefaultPayment = false;
                        objClientPaymentMethod.AddedBy = ClientId;
                        objClientPaymentMethod.AddedByType = General.UserRoles.Client.GetEnumValue();
                        objClientPaymentMethod.AddedDate = DateTime.UtcNow;

                        try
                        {
                            var customerService = new StripeCustomerService();
                            var myCustomer = customerService.Get(dtClient.Rows[0]["StripeCustomerId"].ToString());

                            // setting up the card
                            var myCard = new StripeCardCreateOptions();

                            // setting up the card
                            myCard.SourceCard = new SourceCard()
                            {
                                Number = txtCardNumber.Text.Trim(),
                                ExpirationYear = drpYear.SelectedValue.ToString(), //txtYear.Text.Trim(),
                                ExpirationMonth = drpMonth.SelectedValue.ToString(),//txtMonth.Text.Trim(),
                                Name = txtCardName.Text.Trim(),
                                Cvc = txtCVV.Text.Trim()
                            };

                            var cardService = new StripeCardService();
                            StripeCard stripeCard = cardService.Create(dtClient.Rows[0]["StripeCustomerId"].ToString(), myCard);
                            if (string.IsNullOrEmpty(stripeCard.Id))
                            {
                                dvMessage.InnerHtml = "Invalid card.";
                                dvMessage.Attributes.Add("class", "error");
                                dvMessage.Visible = true;
                                return;
                            }
                            //objClientPaymentMethod.StripeCardId = stripeCard.Id;
                            StripeCardId = stripeCard.Id;
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
                            dvMessage.Attributes.Add("class", "error");
                            dvMessage.Visible = true;
                            return;
                        }
                        objClientPaymentMethodService.AddClientPaymentMethod(ref objClientPaymentMethod);
                    }

                    BizObjects.Orders objOrders = new BizObjects.Orders();
                    objClientService = ServiceFactory.ClientService;
                    objOrderService = ServiceFactory.OrderService;

                    DataTable dtClient1 = new DataTable();
                    string ClientAccountNumber = string.Empty;
                    objClientService.GetClientById(Convert.ToInt32(rblClient.SelectedValue.ToString()), ref dtClient1);
                    if (dtClient1.Rows.Count > 0)
                    {
                        ClientAccountNumber = dtClient1.Rows[0]["AccountNumber"].ToString();
                    }

                    DataTable dtOrder = new DataTable();
                    objOrderService.GetOrderByClientId(Convert.ToInt32(rblClient.SelectedValue.ToString()), ref dtOrder);

                    decimal Subtotal = 0;
                    foreach (DataRow dr in dtParts.Rows)
                    {
                        Subtotal = Subtotal + (Convert.ToInt32(dr["Quantity"].ToString()) * Convert.ToDecimal(dr["Price"].ToString()));
                    }

                    objOrders.OrderNumber = ClientAccountNumber + "-" + txtZip.Text.Trim() + "-O" + (dtOrder.Rows.Count + 1).ToString();
                    objOrders.OrderType = General.BillingTypes.FixedCost.GetEnumDescription();
                    objOrders.ClientId = Convert.ToInt32(rblClient.SelectedValue.ToString());
                    objOrders.OrderAmount = Subtotal;
                    objOrders.ChargeBy = drpCharge.SelectedItem.Text;
                    if (drpCharge.SelectedValue.ToString() == General.ChargeBy.Check.GetEnumDescription())
                    {
                        objOrders.ChequeNo = txtCheckNo.Text.Trim();
                        //objOrders.BankName = txtBank.Text.Trim();
                        //objOrders.RoutingNo = "";
                        objOrders.ChequeDate = Convert.ToDateTime(txtCheckDate.Value);
                        objOrders.AccountingNotes = txtAccNotes.Text.Trim();
                        objOrders.ChqueImageFront = frontImageName;
                        objOrders.ChequeImageBack = backImageName;
                    }
                    if (drpCharge.SelectedValue.ToString() == General.ChargeBy.NewCC.GetEnumDescription())
                    {
                        string cardStr = txtCardNumber.Text.Substring(txtCardNumber.Text.Trim().Length - 4);
                        objOrders.NameOnCard = txtCardName.Text.Trim();
                        if (rblVisa.Checked)
                            objOrders.CardType = "Visa";
                        else if (rblMaster.Checked)
                            objOrders.CardType = "MasterCard";
                        else if (rblDiscover.Checked)
                            objOrders.CardType = "Discover";
                        else
                            objOrders.CardType = "AmericanExpress";
                        objOrders.CardNumber = cardStr.PadLeft(16, '*');
                        objOrders.ExpirationMonth = Convert.ToInt32(drpMonth.SelectedValue.ToString());//Convert.ToInt32(txtMonth.Text.Trim());
                        objOrders.ExpirationYear = Convert.ToInt32(drpYear.SelectedValue.ToString());//Convert.ToInt32(txtYear.Text.Trim());
                    }
                    if (drpCharge.SelectedValue.ToString() == General.ChargeBy.CCOnFile.GetEnumDescription())
                    {

                        DataTable dtPaymentMethod = new DataTable();
                        objClientPaymentMethodService.GetClientPaymentMethodByClientId(Convert.ToInt32(rblClient.SelectedValue.ToString()), ref dtPaymentMethod);
                        if (dtPaymentMethod.Rows.Count > 0)
                        {
                            string filter = "IsDefaultPayment=True";
                            DataTable dtDefaultPayment = new DataTable();
                            DataView dv = new DataView(dtPaymentMethod, filter, "", DataViewRowState.CurrentRows);
                            dtDefaultPayment = dv.ToTable();
                            if (dtDefaultPayment.Rows.Count > 0)
                            {
                                objOrders.NameOnCard = dtDefaultPayment.Rows[0]["NameOnCard"].ToString();
                                objOrders.CardType = dtDefaultPayment.Rows[0]["CardType"].ToString();
                                objOrders.CardNumber = dtDefaultPayment.Rows[0]["CardNumber"].ToString();
                                objOrders.ExpirationMonth = Convert.ToInt32(dtDefaultPayment.Rows[0]["ExpiryMonth"].ToString());
                                objOrders.ExpirationYear = Convert.ToInt32(dtDefaultPayment.Rows[0]["ExpiryYear"].ToString());
                                StripeCardId = dtDefaultPayment.Rows[0]["StripeCardId"].ToString();
                            }
                        }
                    }

                    objOrders.CCEmail = txtCCEmail.Text.Trim();
                    objOrders.IsEmailToClient = chkEmailToClient.Checked;
                    objOrders.CustomerRecommendation = txtNotes.Text.Trim();
                    objOrders.ClientSignature = filenameOriginal;
                    objOrders.AddedBy = objLoginModel.Id;
                    objOrders.AddedByType = objLoginModel.RoleId;
                    objOrders.AddedDate = DateTime.UtcNow;
                    objOrders.IsDeleted = false;

                    int OrderId = 0;
                    OrderId = objOrderService.AddClientOrder(ref objOrders);

                    objOrderItemsService = ServiceFactory.OrderItemsService;

                    if (OrderId > 0)
                    {
                        foreach (DataRow dr in dtParts.Rows)
                        {
                            BizObjects.OrderItems objOrderItems = new BizObjects.OrderItems();
                            objOrderItems.OrderId = OrderId;
                            objOrderItems.PartId = Convert.ToInt32(dr["PartId"].ToString());
                            objOrderItems.Quantity = Convert.ToInt32(dr["Quantity"].ToString());

                            objOrderItemsService.AddOrderItems(ref objOrderItems);
                        }

                        string StripeCustomerId = string.Empty;

                        string PaymentStatus = string.Empty;
                        objClientService = ServiceFactory.ClientService;
                        DataTable dtClient2 = new DataTable();
                        objClientService.GetClientById(Convert.ToInt32(rblClient.SelectedValue.ToString()), ref dtClient2);
                        if (dtClient2.Rows.Count > 0)
                        {
                            try
                            {
                                StripeCustomerId = dtClient2.Rows[0]["StripeCustomerId"].ToString();
                                var StripeResponse = new Aircall.Common.StripeResponse();
                                if (drpCharge.SelectedValue.ToString() != General.ChargeBy.Check.GetEnumDescription())
                                    StripeResponse = General.StripeCharge(true, "", StripeCustomerId, StripeCardId, Convert.ToInt32(Subtotal * 100), "Charge For Admin Part Order", "");

                                if (StripeResponse.PaymentStatus != "Failed" && StripeResponse.ex == null || drpCharge.SelectedValue.ToString() == General.ChargeBy.Check.GetEnumDescription())
                                {
                                    int BillingId = 0;
                                    BizObjects.BillingHistory objBillingHistory = new BizObjects.BillingHistory();
                                    objBillingHistoryService = ServiceFactory.BillingHistoryService;
                                    objBillingHistory.ClientId = Convert.ToInt32(rblClient.SelectedValue);
                                    objBillingHistory.OrderId = OrderId;
                                    objBillingHistory.BillingType = General.BillingTypes.FixedCost.GetEnumDescription();
                                    objBillingHistory.OriginalAmount = Subtotal;
                                    objBillingHistory.PurchasedAmount = Subtotal;
                                    objBillingHistory.PartnerSalesCommisionAmount = 0;
                                    objBillingHistory.BillingAddress = txtAddress.Text.Trim();
                                    objBillingHistory.BillingCity = Convert.ToInt32(drpCity.SelectedValue.ToString());
                                    objBillingHistory.BillingState = Convert.ToInt32(drpState.SelectedValue.ToString());
                                    objBillingHistory.BillingZipcode = txtZip.Text.Trim();
                                    objBillingHistory.TransactionId = StripeResponse.TransactionId;
                                    // objBillingHistory.TransactionDate = DateTime.UtcNow; Code Commented on 19-07-2017
                                    objBillingHistory.TransactionDate = DateTime.UtcNow.ToLocalTime();
                                    objBillingHistory.AddedBy = objLoginModel.Id;
                                    //objBillingHistory.AddedDate = DateTime.UtcNow; Code Commented on 19-07-2017
                                    objBillingHistory.AddedDate = DateTime.UtcNow.ToLocalTime();
                                    objBillingHistory.IsPaid = true;
                                    objBillingHistory.failcode = "";
                                    objBillingHistory.faildesc = "Payment Success!";
                                    BillingId = objBillingHistoryService.AddBillingHistory(ref objBillingHistory);
                                    if (txtCCEmail.Text.Trim() == "")
                                    {
                                        txtCCEmail.Text = txtEmail.Text;
                                    }
                                    var request = (HttpWebRequest)WebRequest.Create(Application["APIURL"] + "api/v1/employee/ResendOrder1?" + "OrderId=" + OrderId + "&CCEmail=" + txtCCEmail.Text.Trim());

                                    request.Method = "GET";
                                    request.ContentType = "application/json";

                                    //using (var stream = request.GetRequestStream())
                                    //{
                                    //    stream.Write(data, 0, data.Length);
                                    //}

                                    try
                                    {
                                        var response = (HttpWebResponse)request.GetResponse();

                                        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                                    }
                                    catch (Exception)
                                    {

                                        throw;
                                    }

                                    if (BillingId > 0)
                                    {
                                        long NotificationId = 0;
                                        int BadgeCount = 0;
                                        string message = string.Empty;

                                        message = General.GetNotificationMessage("PartPurchasedSendToClient");
                                        message = message.Replace("{{EmployeeName}}", objLoginModel.FullName);
                                        BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
                                        IUserNotificationService objUserNotificationService;
                                        DataTable dtBadgeCount = new DataTable();

                                        objUserNotificationService = ServiceFactory.UserNotificationService;
                                        objUserNotification.UserId = Convert.ToInt32(rblClient.SelectedValue.ToString());
                                        objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                                        objUserNotification.Message = message;
                                        objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();
                                        objUserNotification.CommonId = BillingId;
                                        objUserNotification.MessageType = General.NotificationType.PartPurchased.GetEnumDescription();
                                        objUserNotification.AddedDate = DateTime.UtcNow;

                                        NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

                                        dtBadgeCount.Clear();

                                        objUserNotificationService.GetBadgeCount(Convert.ToInt32(rblClient.SelectedValue.ToString()), General.UserRoles.Client.GetEnumValue(), ref dtBadgeCount);
                                        BadgeCount = dtBadgeCount.Rows.Count;

                                        Notifications objNotifications = new Notifications { NId = NotificationId, NType = General.NotificationType.PartPurchased.GetEnumValue(), CommonId = BillingId };
                                        List<NotificationModel> notify = new List<NotificationModel>();
                                        notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                        notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                        notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                                        if (!string.IsNullOrEmpty(dtClient2.Rows[0]["DeviceType"].ToString()) &&
                                            !string.IsNullOrEmpty(dtClient2.Rows[0]["DeviceToken"].ToString()) &&
                                            dtClient2.Rows[0]["DeviceToken"].ToString().ToLower() != "no device token")
                                        {
                                            if (dtClient2.Rows[0]["DeviceType"].ToString().ToLower() == "android")
                                            {
                                                string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                                SendNotifications.SendAndroidNotification(dtClient2.Rows[0]["DeviceToken"].ToString(), message, CustomData, "client");
                                            }
                                            else if (dtClient2.Rows[0]["DeviceType"].ToString().ToLower() == "iphone")
                                            {
                                                SendNotifications.SendIphoneNotification(BadgeCount, dtClient2.Rows[0]["DeviceToken"].ToString(), message, notify, "client");
                                            }
                                        }
                                    }
                                    Session["msg"] = "add";
                                    Response.Redirect(Application["SiteAddress"] + "admin/Order_List.aspx");
                                }
                                else if (StripeResponse.PaymentStatus.ToString() == "Failed")
                                {
                                    objOrderService.DeleteOrder(OrderId);
                                    dvMessage.InnerHtml = "<strong>Payment Failed</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
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
                                objStripeErrorLog.Userid = Convert.ToInt32(rblClient.SelectedValue.ToString());
                                objStripeErrorLog.DateAdded = DateTime.UtcNow;

                                objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);
                                dvMessage.InnerHtml = "<strong>" + stex.StripeError.Message + "</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}