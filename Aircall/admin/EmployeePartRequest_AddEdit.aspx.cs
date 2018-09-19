using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using Aircall.Common;
using System.Data;

namespace Aircall.admin
{
    public partial class EmployeePartRequest_AddEdit : System.Web.UI.Page
    {
        IEmployeeService objEmployeeService;
        IPartsService objPartsService;
        IClientService objClientService;
        IClientAddressService objClientAddressService;
        IClientUnitService objClientUnitService;
        IEmployeePartRequestMasterService objEmployeePartRequestMasterService;
        IEmployeePartRequestService objEmployeePartRequestService;
        IUserNotificationService objUserNotificationService;
        IEmployeeWorkAreaService objEmployeeWorkAreaService;

        public static DataTable dtParts = new DataTable("Parts");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dtParts.Rows.Clear();
                dtParts.Columns.Clear();
                dtParts.Columns.Add("Id");
                dtParts.Columns.Add("UnitId");
                dtParts.Columns.Add("UnitName");
                dtParts.Columns.Add("PartId");
                dtParts.Columns.Add("PartName");
                dtParts.Columns.Add("PartSize");
                dtParts.Columns.Add("Description");
                dtParts.Columns.Add("Quantity");
                dtParts.Columns.Add("ArrangedQuantity");

                dtParts.AcceptChanges();
                FillStatusDropdown();
                FillPartsDropdown();
                if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
                {
                    BindEmployeePartRequest();
                }
            }
        }

        private void BindEmployeePartRequest()
        {
            btnAdd.Visible = false;
            btnUpdate.Visible = true;
            int RequestId = Convert.ToInt32(Request.QueryString["Id"].ToString());
            objEmployeePartRequestMasterService = ServiceFactory.EmployeePartRequestMasterService;
            DataTable dtEmployeeRequest = new DataTable();
            objEmployeePartRequestMasterService.GetEmployeePartRequestById(RequestId, ref dtEmployeeRequest);
            if (dtEmployeeRequest.Rows.Count > 0)
            {
                lnkSearch.Visible = false;
                txtEmpName.Text = dtEmployeeRequest.Rows[0]["EmployeeName"].ToString();
                int EmployeeId = Convert.ToInt32(dtEmployeeRequest.Rows[0]["EmployeeId"].ToString());
                //objEmployeeService = ServiceFactory.EmployeeService;
                //DataTable dtEmployee = new DataTable();
                //objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
                //if (dtEmployee.Rows.Count > 0)
                //{
                //    rblEmployee.DataSource = dtEmployee;
                //    rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
                //    rblEmployee.DataValueField = dtEmployee.Columns["Id"].ToString();
                //    rblEmployee.DataBind();
                //}
                //rblEmployee.SelectedValue = EmployeeId.ToString();

                lnkClientSearch.Visible = false;
                txtClient.Text = dtEmployeeRequest.Rows[0]["ClientName"].ToString();
                int ClientId = Convert.ToInt32(dtEmployeeRequest.Rows[0]["ClientId"].ToString());
                objClientService = ServiceFactory.ClientService;
                DataTable dtClient = new DataTable();
                objClientService.GetClientById(ClientId, ref dtClient);
                if (dtClient.Rows.Count > 0)
                {
                    rblClient.DataSource = dtClient;
                    rblClient.DataTextField = dtClient.Columns["ClientName"].ToString();
                    rblClient.DataValueField = dtClient.Columns["Id"].ToString();
                    rblClient.DataBind();
                }
                rblClient.SelectedValue = ClientId.ToString();
                rblClient.Enabled = false;
                BindAddressByClientId(ClientId);
                rblAddress.SelectedValue = dtEmployeeRequest.Rows[0]["ClientAddressId"].ToString();
                rblAddress.Enabled = false;
                BindEmployee(Convert.ToInt32(dtEmployeeRequest.Rows[0]["ClientAddressId"].ToString()));
                rblEmployee.SelectedValue = EmployeeId.ToString();
                BindClientUnitsByClientAndAddress(ClientId, Convert.ToInt32(rblAddress.SelectedValue));

                drpPartStatus.SelectedValue = dtEmployeeRequest.Rows[0]["Status"].ToString();

                if (dtEmployeeRequest.Rows[0]["Status"].ToString() == General.EmpPartRequestStatus.Completed.GetEnumDescription())
                    btnUpdate.Visible = false;

                txtEmpNotes.Text = dtEmployeeRequest.Rows[0]["EmpNotes"].ToString();

                if (dtEmployeeRequest.Rows[0]["Status"].ToString() == General.EmpPartRequestStatus.Discontinued.GetEnumDescription() ||
                    dtEmployeeRequest.Rows[0]["Status"].ToString() == General.EmpPartRequestStatus.Backordered.GetEnumDescription())
                {
                    dvNotes.Visible = true;
                    txtNotes.Text = dtEmployeeRequest.Rows[0]["Notes"].ToString();
                }

                objEmployeePartRequestService = ServiceFactory.EmployeePartRequestService;
                DataTable dtRequestParts = new DataTable();
                objEmployeePartRequestService.GetEmployeePartRequestByRequestId(RequestId, ref dtRequestParts);
                if (dtRequestParts.Rows.Count > 0)
                {
                    dtParts = dtRequestParts;
                }
                lstParts.DataSource = dtParts;
                lstParts.DataBind();
                if (dtEmployeeRequest.Rows[0]["Status"].ToString()==General.EmpPartRequestStatus.Completed.GetEnumDescription())
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertMessage", "$('.clsAction').hide();", true);
                }
            }
        }

        private void FillPartsDropdown()
        {
            objPartsService = ServiceFactory.PartsService;
            DataTable dtParts = new DataTable();
            objPartsService.GetAllParts(true, ref dtParts);
            if (dtParts.Rows.Count > 0)
            {
                drpParts.DataSource = dtParts;
                drpParts.DataTextField = dtParts.Columns["Name"].ToString();
                drpParts.DataValueField = dtParts.Columns["Id"].ToString();
            }
            drpParts.DataBind();
            drpParts.Items.Insert(0, new ListItem("Select Part", "0"));
        }

        private void FillStatusDropdown()
        {
            var values = DurationExtensions.GetValues<General.EmpPartRequestStatus>();
            List<string> data = new List<string>();
            foreach (var item in values)
            {
                General.EmpPartRequestStatus p = (General.EmpPartRequestStatus)item;
                data.Add(p.GetEnumDescription());
            }
            drpPartStatus.DataSource = data;
            drpPartStatus.DataBind();
            drpPartStatus.Items.Insert(0, new ListItem("Select Status", "0"));
        }

        protected void lnkSearch_Click(object sender, EventArgs e)
        {
            if (rblAddress.Items.Count != 0 || rblAddress.SelectedIndex != -1)
            {
                BindEmployee(Convert.ToInt32(rblAddress.SelectedItem.Value));
            }
        }

        private void BindEmployee(int AddressId)
        {

            objEmployeeWorkAreaService = ServiceFactory.EmployeeWorkAreaService;
            DataTable dtEmployee = new DataTable();
            objEmployeeWorkAreaService.GetEmployeeByClientAddressId(txtEmpName.Text.Trim(), AddressId, ref dtEmployee);
            if (dtEmployee.Rows.Count > 0)
            {
                rblEmployee.DataSource = dtEmployee;
                rblEmployee.DataTextField = dtEmployee.Columns["EmployeeName"].ToString();
                rblEmployee.DataValueField = dtEmployee.Columns["Id"].ToString();
                rblEmployee.DataBind();
            }
            else
            {
                rblEmployee.DataSource = "";
                rblEmployee.DataBind();
            }
        }

        protected void lnkClientSearch_Click(object sender, EventArgs e)
        {
            objClientService = ServiceFactory.ClientService;
            DataTable dtClients = new DataTable();
            objClientService.GetClientByName(txtClient.Text.Trim(), ref dtClients);
            if (dtClients.Rows.Count > 0)
            {
                rblClient.DataSource = dtClients;
                rblClient.DataTextField = dtClients.Columns["ClientName"].ToString();
                rblClient.DataValueField = dtClients.Columns["Id"].ToString();
            }
            else
                rblClient.DataSource = "";
            rblClient.DataBind();
        }

        protected void rblClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            rblAddress.DataSource = "";
            rblAddress.DataBind();

            rblClientUnits.DataSource = "";
            rblClientUnits.DataBind();

            rblEmployee.DataSource = "";
            rblEmployee.DataBind();

            BindAddressByClientId(Convert.ToInt32(rblClient.SelectedValue));
        }

        private void BindAddressByClientId(int ClientId)
        {
            objClientAddressService = ServiceFactory.ClientAddressService;
            DataTable dtClientAddress = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
                objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtClientAddress);
            else
                objClientAddressService.GetClientAddressesByClientId(ClientId, false, ref dtClientAddress);

            if (dtClientAddress.Rows.Count > 0)
            {
                rblAddress.DataSource = dtClientAddress;
                rblAddress.DataTextField = dtClientAddress.Columns["ClientAddress"].ToString();
                rblAddress.DataValueField = dtClientAddress.Columns["Id"].ToString();
            }
            rblAddress.DataBind();
        }

        protected void rblAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            rblClientUnits.DataSource = "";
            rblClientUnits.DataBind();

            BindClientUnitsByClientAndAddress(Convert.ToInt32(rblClient.SelectedValue.ToString()), Convert.ToInt32(rblAddress.SelectedValue.ToString()));
            BindEmployee(Convert.ToInt32(rblAddress.SelectedItem.Value));
        }

        private void BindClientUnitsByClientAndAddress(int ClientId, int AddressId)
        {
            objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dtClientUnits = new DataTable();
            objClientUnitService.GetClientUnitByClientAndAddressId(ClientId, AddressId, ref dtClientUnits);
            if (dtClientUnits.Rows.Count > 0)
            {
                rblClientUnits.DataSource = dtClientUnits;
                rblClientUnits.DataTextField = dtClientUnits.Columns["UnitName"].ToString();
                rblClientUnits.DataValueField = dtClientUnits.Columns["Id"].ToString();
            }
            rblClientUnits.DataBind();
        }

        protected void btnAddPart_Click(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            if (rblEmployee.Items.Count == 0 || rblEmployee.SelectedIndex == -1)
            {
                dvMessage.InnerHtml = "<strong>Please Select Employee.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return;
            }

            if (rblClient.Items.Count == 0 || rblClient.SelectedIndex == -1)
            {
                dvMessage.InnerHtml = "<strong>Please Select Client.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return;
            }

            if (rblAddress.Items.Count == 0 || rblAddress.SelectedIndex == -1)
            {
                dvMessage.InnerHtml = "<strong>Please Select Address.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return;
            }

            if (rblClientUnits.Items.Count == 0 || rblClientUnits.SelectedIndex == -1)
            {
                dvMessage.InnerHtml = "<strong>Please Select Unit.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
                return;
            }

            DataRow dr = dtParts.NewRow();
            dr["Id"] = dtParts.Rows.Count + 1;
            dr["UnitId"] = rblClientUnits.SelectedValue;
            dr["UnitName"] = rblClientUnits.SelectedItem.Text;

            if (drpParts.SelectedValue != "0")
            {
                objPartsService = ServiceFactory.PartsService;
                DataTable dtPart = new DataTable();
                objPartsService.GetPartById(Convert.ToInt32(drpParts.SelectedValue.ToString()), ref dtPart);
                if (dtPart.Rows.Count > 0)
                {
                    dr["PartId"] = drpParts.SelectedValue.ToString();
                    dr["PartName"] = dtPart.Rows[0]["Name"].ToString();
                    dr["PartSize"] = dtPart.Rows[0]["Size"].ToString();
                    dr["Description"] = dtPart.Rows[0]["Description"].ToString();
                }
            }
            else
            {
                dr["PartName"] = txtPartName.Text.Trim();
                dr["PartSize"] = txtPartSize.Text.Trim();
                dr["Description"] = txtDescription.Text.Trim();
            }
            dr["Quantity"] = txtQuantity.Text.Trim();
            dr["ArrangedQuantity"] = txtQuantity.Text.Trim();
            dtParts.Rows.Add(dr);

            lstParts.DataSource = dtParts;
            lstParts.DataBind();

            drpParts.SelectedValue = "0";
            txtPartName.Text = string.Empty;
            txtPartSize.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtQuantity.Text = string.Empty;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    dvMessage.InnerHtml = "";
                    dvMessage.Visible = false;
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        if (rblClient.Items.Count == 0 || rblClient.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Client.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        if (rblAddress.Items.Count == 0 || rblAddress.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Address.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        if (dtParts.Rows.Count == 0)
                        {
                            dvMessage.InnerHtml = "<strong>Please insert Parts.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        BizObjects.EmployeePartRequestMaster objEmployeePartRequestMaster = new BizObjects.EmployeePartRequestMaster();

                        int UserId = objLoginModel.Id;
                        int RoleId = objLoginModel.RoleId;

                        objEmployeePartRequestMasterService = ServiceFactory.EmployeePartRequestMasterService;
                        objEmployeePartRequestService = ServiceFactory.EmployeePartRequestService;

                        int EmployeePartReqId = 0;
                        objEmployeePartRequestMaster.Status = drpPartStatus.SelectedItem.Text;//General.EmpPartRequestStatus.NeedToOrder.GetEnumDescription();
                        objEmployeePartRequestMaster.AddedDate = DateTime.UtcNow;
                        objEmployeePartRequestMaster.EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue.ToString());
                        objEmployeePartRequestMaster.ClientId = Convert.ToInt32(rblClient.SelectedValue.ToString());
                        objEmployeePartRequestMaster.ClientAddressId = Convert.ToInt32(rblAddress.SelectedValue.ToString());
                        objEmployeePartRequestMaster.AddedBy = UserId;
                        objEmployeePartRequestMaster.AddedByType = RoleId;
                        objEmployeePartRequestMaster.EmpNotes = txtEmpNotes.Text.Trim();

                        if (drpPartStatus.SelectedItem.Text == General.EmpPartRequestStatus.Backordered.GetEnumDescription() ||
                            drpPartStatus.SelectedItem.Text == General.EmpPartRequestStatus.Discontinued.GetEnumDescription())
                        {
                            objEmployeePartRequestMaster.Notes = txtNotes.Text.Trim();
                        }

                        EmployeePartReqId = objEmployeePartRequestMasterService.AddEmployeePartRequest(ref objEmployeePartRequestMaster);

                        if (EmployeePartReqId != 0)
                        {
                            BizObjects.EmployeePartRequest objEmployeePartRequest = new BizObjects.EmployeePartRequest();

                            for (int i = 0; i < dtParts.Rows.Count; i++)
                            {
                                objEmployeePartRequest.EmployeePartRequestId = EmployeePartReqId;
                                objEmployeePartRequest.UnitId = Convert.ToInt32(dtParts.Rows[i]["UnitId"].ToString());
                                if (!string.IsNullOrEmpty(dtParts.Rows[i]["PartId"].ToString()))
                                {
                                    objEmployeePartRequest.PartId = Convert.ToInt32(dtParts.Rows[i]["PartId"].ToString());
                                    DataTable dtDbParts = new DataTable();
                                    objPartsService = ServiceFactory.PartsService;
                                    objPartsService.GetPartById(Convert.ToInt32(dtParts.Rows[i]["PartId"].ToString()), ref dtDbParts);
                                    if (dtDbParts.Rows.Count > 0)
                                    {
                                        objEmployeePartRequest.PartName = dtDbParts.Rows[0]["Name"].ToString();
                                        objEmployeePartRequest.PartSize = dtDbParts.Rows[0]["Size"].ToString();
                                        objEmployeePartRequest.Description = dtDbParts.Rows[0]["Description"].ToString();
                                    }
                                    else
                                    {
                                        objEmployeePartRequest.PartName = string.Empty;
                                        objEmployeePartRequest.PartSize = string.Empty;
                                        objEmployeePartRequest.Description = string.Empty;
                                    }
                                }

                                else
                                {
                                    objEmployeePartRequest.PartName = dtParts.Rows[i]["PartName"].ToString();
                                    objEmployeePartRequest.PartSize = dtParts.Rows[i]["PartSize"].ToString();
                                    objEmployeePartRequest.Description = dtParts.Rows[i]["Description"].ToString();
                                }

                                TextBox objQty = (TextBox)lstParts.Items[i].FindControl("txtQty");
                                if (!string.IsNullOrEmpty(objQty.Text))
                                {
                                    objEmployeePartRequest.RequestedQuantity = Convert.ToInt32(objQty.Text);
                                    objEmployeePartRequest.ArrangedQuantity = Convert.ToInt32(objQty.Text);
                                }
                                objEmployeePartRequestService.AddEmployeePartRequest(ref objEmployeePartRequest);
                            }

                            if (drpPartStatus.SelectedItem.Text == General.EmpPartRequestStatus.NeedToOrder.GetEnumDescription())
                            {
                                //check instock and insert new service
                                DataTable dtService = new DataTable();
                                objEmployeePartRequestMasterService.CheckInstockAndScheduleService(EmployeePartReqId, UserId, RoleId, DateTime.UtcNow, ref dtService);
                                if (dtService.Rows.Count > 0)
                                {
                                    long ServiceId = Convert.ToInt64(dtService.Rows[0]["ServiceId"].ToString());
                                    DateTime ScheduleDate = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString());

                                    if (ServiceId != 0)
                                    {
                                        string ServiceCaseNumber = dtService.Rows[0]["ServiceCaseNumber"].ToString();
                                        string strAddress = dtService.Rows[0]["Address"].ToString();
                                        //deducted from In-Stock Quantity and added to Reserved Quantity
                                        objPartsService = ServiceFactory.PartsService;
                                        for (int i = 0; i < dtParts.Rows.Count; i++)
                                        {
                                            int PartId = Convert.ToInt32(dtParts.Rows[i]["PartId"].ToString());
                                            int Quantity = Convert.ToInt32(dtParts.Rows[i]["Quantity"].ToString());
                                            objPartsService.UpdateInStock(PartId, Quantity);
                                        }

                                        string message = string.Empty;
                                        string ClientDeviceType = string.Empty;
                                        string ClientDeviceToken = string.Empty;
                                        string EmployeeDeviceType = string.Empty;
                                        string EmployeeDeviceToken = string.Empty;
                                        objClientService = ServiceFactory.ClientService;
                                        DataTable dtClient = new DataTable();
                                        objClientService.GetClientById(Convert.ToInt32(rblClient.SelectedValue), ref dtClient);
                                        if (dtClient.Rows.Count > 0)
                                        {
                                            ClientDeviceType = dtClient.Rows[0]["DeviceType"].ToString();
                                            ClientDeviceToken = dtClient.Rows[0]["DeviceToken"].ToString();
                                        }

                                        objEmployeeService = ServiceFactory.EmployeeService;
                                        DataTable dtEmployee = new DataTable();
                                        objEmployeeService.GetEmployeeById(Convert.ToInt32(rblEmployee.SelectedValue), ref dtEmployee);
                                        if (dtEmployee.Rows.Count > 0)
                                        {
                                            EmployeeDeviceType = dtEmployee.Rows[0]["DeviceType"].ToString();
                                            EmployeeDeviceToken = dtEmployee.Rows[0]["DeviceToken"].ToString();
                                        }
                                        message = General.GetNotificationMessage("PartRequestServiceScheduleSendToClient"); //"Service " + ServiceCaseNumber + " for your requested parts has been scheduled on " + ScheduleDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                        message = message.Replace("{{Address}}", strAddress);
                                        message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                        NotifyUser(Convert.ToInt32(rblClient.SelectedValue), "client", General.NotificationType.PeriodicServiceReminder.GetEnumDescription(), General.NotificationType.PeriodicServiceReminder.GetEnumValue(), ServiceId, message, ClientDeviceType, ClientDeviceToken);

                                        message = General.GetNotificationMessage("EmployeeSchedule"); //"System has scheduled a service for you on " + ScheduleDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                        message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                        NotifyUser(Convert.ToInt32(rblEmployee.SelectedValue), "employee", General.NotificationType.ServiceScheduled.GetEnumDescription(), General.NotificationType.ServiceScheduled.GetEnumValue(), ServiceId, message, EmployeeDeviceType, EmployeeDeviceToken);
                                    }
                                }
                            }
                        }
                        Session["msg"] = "add";
                        Response.Redirect(Application["SiteAddress"] + "/admin/EmployeePartRequest_List.aspx");
                    }
                    else
                        Response.Redirect(Application["SiteAddress"] + "/admin/Login.aspx");
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        if (rblClient.Items.Count == 0 || rblClient.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Client.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        if (rblAddress.Items.Count == 0 || rblAddress.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Address.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        BizObjects.EmployeePartRequestMaster objEmployeePartRequestMaster = new BizObjects.EmployeePartRequestMaster();

                        objEmployeePartRequestMasterService = ServiceFactory.EmployeePartRequestMasterService;
                        objEmployeePartRequestService = ServiceFactory.EmployeePartRequestService;

                        int EmployeePartReqId = 0;
                        if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
                        {
                            EmployeePartReqId = Convert.ToInt32(Request.QueryString["Id"].ToString());
                            objEmployeePartRequestMaster.Id = EmployeePartReqId;
                            objEmployeePartRequestMaster.Status = drpPartStatus.SelectedValue.ToString();
                            objEmployeePartRequestMaster.EmployeeId = Convert.ToInt32(rblEmployee.SelectedValue.ToString());
                            objEmployeePartRequestMaster.ClientId = Convert.ToInt32(rblClient.SelectedValue.ToString());
                            objEmployeePartRequestMaster.ClientAddressId = Convert.ToInt32(rblAddress.SelectedValue.ToString());
                            objEmployeePartRequestMaster.UpdatedBy = objLoginModel.Id;
                            objEmployeePartRequestMaster.UpdatedByType = objLoginModel.RoleId;
                            objEmployeePartRequestMaster.UpdatedDate = DateTime.UtcNow;
                            objEmployeePartRequestMaster.EmpNotes = txtEmpNotes.Text.Trim();

                            if (drpPartStatus.SelectedItem.Text == General.EmpPartRequestStatus.Backordered.GetEnumDescription() ||
                            drpPartStatus.SelectedItem.Text == General.EmpPartRequestStatus.Discontinued.GetEnumDescription())
                            {
                                objEmployeePartRequestMaster.Notes = txtNotes.Text.Trim();
                            }

                            objEmployeePartRequestMasterService.UpdateEmployeePartRequest(ref objEmployeePartRequestMaster);

                            objEmployeePartRequestService.DeleteEmployeePartRequestByRequestId(EmployeePartReqId);


                            BizObjects.EmployeePartRequest objEmployeePartRequest = new BizObjects.EmployeePartRequest();

                            for (int i = 0; i < dtParts.Rows.Count; i++)
                            {
                                objEmployeePartRequest.EmployeePartRequestId = EmployeePartReqId;
                                objEmployeePartRequest.UnitId = Convert.ToInt32(dtParts.Rows[i]["UnitId"].ToString());
                                if (!string.IsNullOrEmpty(dtParts.Rows[i]["PartId"].ToString()))
                                {
                                    objEmployeePartRequest.PartId = Convert.ToInt32(dtParts.Rows[i]["PartId"].ToString());
                                    DataTable dtDbParts = new DataTable();
                                    objPartsService = ServiceFactory.PartsService;
                                    objPartsService.GetPartById(Convert.ToInt32(dtParts.Rows[i]["PartId"].ToString()), ref dtDbParts);
                                    if (dtDbParts.Rows.Count > 0)
                                    {
                                        objEmployeePartRequest.PartName = dtDbParts.Rows[0]["Name"].ToString();
                                        objEmployeePartRequest.PartSize = dtDbParts.Rows[0]["Size"].ToString();
                                        objEmployeePartRequest.Description = dtDbParts.Rows[0]["Description"].ToString();
                                    }
                                    else
                                    {
                                        objEmployeePartRequest.PartName = string.Empty;
                                        objEmployeePartRequest.PartSize = string.Empty;
                                        objEmployeePartRequest.Description = string.Empty;
                                    }
                                }
                                else
                                {
                                    objEmployeePartRequest.PartId = 0;
                                    objEmployeePartRequest.PartName = dtParts.Rows[i]["PartName"].ToString();
                                    objEmployeePartRequest.PartSize = dtParts.Rows[i]["PartSize"].ToString();
                                    objEmployeePartRequest.Description = dtParts.Rows[i]["Description"].ToString();
                                }

                                TextBox objQty = (TextBox)lstParts.Items[i].FindControl("txtQty");
                                if (!string.IsNullOrEmpty(objQty.Text))
                                    objEmployeePartRequest.RequestedQuantity = Convert.ToInt32(objQty.Text);

                                TextBox objArrQty = (TextBox)lstParts.Items[i].FindControl("txtArrQty");
                                if (!string.IsNullOrEmpty(objArrQty.Text))
                                    objEmployeePartRequest.ArrangedQuantity = Convert.ToInt32(objArrQty.Text);

                                objEmployeePartRequestService.AddEmployeePartRequest(ref objEmployeePartRequest);
                            }
                        }

                        string ClientDeviceType = string.Empty;
                        string ClientDeviceToken = string.Empty;
                        string EmployeeDeviceType = string.Empty;
                        string EmployeeDeviceToken = string.Empty;
                        objClientService = ServiceFactory.ClientService;
                        DataTable dtClient = new DataTable();
                        objClientService.GetClientById(Convert.ToInt32(rblClient.SelectedValue), ref dtClient);
                        if (dtClient.Rows.Count > 0)
                        {
                            ClientDeviceType = dtClient.Rows[0]["DeviceType"].ToString();
                            ClientDeviceToken = dtClient.Rows[0]["DeviceToken"].ToString();
                        }

                        objEmployeeService = ServiceFactory.EmployeeService;
                        DataTable dtEmployee = new DataTable();
                        objEmployeeService.GetEmployeeById(Convert.ToInt32(rblEmployee.SelectedValue), ref dtEmployee);
                        if (dtEmployee.Rows.Count > 0)
                        {
                            EmployeeDeviceType = dtEmployee.Rows[0]["DeviceType"].ToString();
                            EmployeeDeviceToken = dtEmployee.Rows[0]["DeviceToken"].ToString();
                        }

                        switch (drpPartStatus.SelectedValue)
                        {
                            case "Discontinued":
                                string message = General.GetNotificationMessage("PartDiscontinuedSendToClientAndEmp"); //"Requested part is discontinued.";
                                NotifyUser(Convert.ToInt32(rblClient.SelectedValue), "client", General.NotificationType.FriendlyReminder.GetEnumDescription(), General.NotificationType.FriendlyReminder.GetEnumValue(), 0, message, ClientDeviceType, ClientDeviceToken);
                                NotifyUser(Convert.ToInt32(rblEmployee.SelectedValue), "employee", General.NotificationType.FriendlyReminder.GetEnumDescription(), General.NotificationType.FriendlyReminder.GetEnumValue(), 0, message, EmployeeDeviceType, EmployeeDeviceToken);
                                break;

                            case "Backordered":
                                message = General.GetNotificationMessage("PartBackorderedSendToClientAndEmp"); //"Requested part is Backordered. Requested Parts are in temporary halt.";
                                NotifyUser(Convert.ToInt32(rblClient.SelectedValue), "client", General.NotificationType.FriendlyReminder.GetEnumDescription(), General.NotificationType.FriendlyReminder.GetEnumValue(), 0, message, ClientDeviceType, ClientDeviceToken);
                                NotifyUser(Convert.ToInt32(rblEmployee.SelectedValue), "employee", General.NotificationType.FriendlyReminder.GetEnumDescription(), General.NotificationType.FriendlyReminder.GetEnumValue(), 0, message, EmployeeDeviceType, EmployeeDeviceToken);
                                break;

                            case "Cancelled":
                                message = General.GetNotificationMessage("PartCancelledSendToEmp"); //"Requested Part is Cancelled by admin.";
                                NotifyUser(Convert.ToInt32(rblEmployee.SelectedValue), "employee", General.NotificationType.FriendlyReminder.GetEnumDescription(), General.NotificationType.FriendlyReminder.GetEnumValue(), 0, message, EmployeeDeviceType, EmployeeDeviceToken);
                                break;

                            default:
                                break;

                        }

                        //Completed
                        if (drpPartStatus.SelectedValue == General.EmpPartRequestStatus.Completed.GetEnumDescription())
                        {
                            int UserId = objLoginModel.Id;
                            int RoleId = objLoginModel.RoleId;

                            DataTable dtService = new DataTable();
                            objEmployeePartRequestMasterService.ArrangePartAndScheduleService(EmployeePartReqId, UserId, RoleId, DateTime.UtcNow, ref dtService);
                            if (dtService.Rows.Count > 0)
                            {
                                long ServiceId = Convert.ToInt64(dtService.Rows[0]["ServiceId"].ToString());


                                if (ServiceId != 0 && dtService.Rows[0]["ScheduleDate"].ToString() != "")
                                {
                                    string ServiceCaseNumber = dtService.Rows[0]["ServiceCaseNumber"].ToString();
                                    DateTime ScheduleDate = Convert.ToDateTime(dtService.Rows[0]["ScheduleDate"].ToString());
                                    string message = string.Empty;
                                    string strAddress = dtService.Rows[0]["Address"].ToString();
                                    //string ClientDeviceType = string.Empty;
                                    //string ClientDeviceToken = string.Empty;
                                    //string EmployeeDeviceType = string.Empty;
                                    //string EmployeeDeviceToken = string.Empty;
                                    //objClientService = ServiceFactory.ClientService;
                                    //DataTable dtClient = new DataTable();
                                    //objClientService.GetClientById(Convert.ToInt32(rblClient.SelectedValue), ref dtClient);
                                    //if (dtClient.Rows.Count > 0)
                                    //{
                                    //    ClientDeviceType = dtClient.Rows[0]["DeviceType"].ToString();
                                    //    ClientDeviceToken = dtClient.Rows[0]["DeviceToken"].ToString();
                                    //}

                                    //objEmployeeService = ServiceFactory.EmployeeService;
                                    //DataTable dtEmployee = new DataTable();
                                    //objEmployeeService.GetEmployeeById(Convert.ToInt32(rblEmployee.SelectedValue), ref dtEmployee);
                                    //if (dtEmployee.Rows.Count > 0)
                                    //{
                                    //    EmployeeDeviceType = dtEmployee.Rows[0]["DeviceType"].ToString();
                                    //    EmployeeDeviceToken = dtEmployee.Rows[0]["DeviceToken"].ToString();
                                    //}
                                    message = General.GetNotificationMessage("PartRequestServiceScheduleSendToClient"); //"Service " + ServiceCaseNumber + " for your requested parts has been scheduled on " + ScheduleDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                    message = message.Replace("{{Address}}", strAddress);
                                    message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                    NotifyUser(Convert.ToInt32(rblClient.SelectedValue), "client", General.NotificationType.PeriodicServiceReminder.GetEnumDescription(), General.NotificationType.PeriodicServiceReminder.GetEnumValue(), ServiceId, message, ClientDeviceType, ClientDeviceToken);

                                    message = General.GetNotificationMessage("EmployeeSchedule"); //"System has scheduled a service for you on " + ScheduleDate.ToLocalTime().ToString("MMMM dd, yyyy") + ".";
                                    message = message.Replace("{{ScheduleDate}}", ScheduleDate.ToString("MMMM dd, yyyy"));
                                    NotifyUser(Convert.ToInt32(rblEmployee.SelectedValue), "employee", General.NotificationType.ServiceScheduled.GetEnumDescription(), General.NotificationType.ServiceScheduled.GetEnumValue(), ServiceId, message, EmployeeDeviceType, EmployeeDeviceToken);
                                }
                            }
                        }
                        Session["msg"] = "edit";
                        Response.Redirect(Application["SiteAddress"] + "admin/EmployeePartRequest_List.aspx");
                    }
                    else
                        Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        private void NotifyUser(int UserId, string Role, string MessageType, int NoType, long ServiceId, string message, string DeviceType, string DeviceToken)
        {
            long NotificationId = 0;
            DataTable dtBadgeCount = new DataTable();
            int BadgeCount = 0;
            int NType = 0;
            BizObjects.UserNotification objUserNotification = new BizObjects.UserNotification();
            objUserNotificationService = ServiceFactory.UserNotificationService;
            objUserNotification.UserId = UserId;
            if (Role.ToLower() == "client")
            {
                objUserNotification.UserTypeId = General.UserRoles.Client.GetEnumValue();
                //objUserNotification.MessageType = General.NotificationType.PeriodicServiceReminder.GetEnumDescription();
                //NType = General.NotificationType.PeriodicServiceReminder.GetEnumValue();
            }
            else
            {
                objUserNotification.UserTypeId = General.UserRoles.Employee.GetEnumValue();
                //objUserNotification.MessageType = General.NotificationType.ServiceScheduled.GetEnumDescription();
                //NType = General.NotificationType.ServiceScheduled.GetEnumValue();
            }
            objUserNotification.MessageType = MessageType;
            NType = NoType;

            objUserNotification.Message = message;
            if (ServiceId != 0)
                objUserNotification.CommonId = ServiceId;

            objUserNotification.Status = General.NotificationStatus.UnRead.GetEnumDescription();

            objUserNotification.AddedDate = DateTime.UtcNow;

            NotificationId = objUserNotificationService.AddUserNotification(ref objUserNotification);

            objUserNotificationService.GetBadgeCount(UserId, objUserNotification.UserTypeId, ref dtBadgeCount);
            BadgeCount = dtBadgeCount.Rows.Count;

            Notifications objNotifications = new Notifications { NId = NotificationId, NType = NType, CommonId = objUserNotification.CommonId };
            List<NotificationModel> notify = new List<NotificationModel>();
            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

            if (Role.ToLower() == "client")
            {
                if (DeviceType.ToLower() == "android")
                {
                    string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                    SendNotifications.SendAndroidNotification(DeviceToken, message, CustomData, "client");
                }
                else if (DeviceType.ToLower() == "iphone")
                {
                    SendNotifications.SendIphoneNotification(BadgeCount, DeviceToken, message, notify, "client");
                }
            }
            else
            {
                if (DeviceType.ToLower() == "iphone")
                {
                    SendNotifications.SendIphoneNotification(BadgeCount, DeviceToken, message, notify, "employee");
                }
            }
        }

        protected void drpPartStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpPartStatus.SelectedItem.Text == General.EmpPartRequestStatus.Backordered.GetEnumDescription() ||
                drpPartStatus.SelectedItem.Text == General.EmpPartRequestStatus.Discontinued.GetEnumDescription())
                dvNotes.Visible = true;
            else
                dvNotes.Visible = false;
        }

        protected void lstParts_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemovePart" && e.CommandArgument.ToString() != "0")
            {
                int Id = Convert.ToInt32(e.CommandArgument.ToString());
                DataRow dr = dtParts.Select("Id='" + Id + "'").FirstOrDefault();
                if (dr!=null)
                {
                    dtParts.Rows.Remove(dr);
                }
                //dtParts.Rows.RemoveAt(Id - 1);
            }
            DataTable dtNewParts = new DataTable();
            dtNewParts = dtParts.Copy();
            dtNewParts.AcceptChanges();
            dtParts.Rows.Clear();
            dtParts.AcceptChanges();

            foreach (DataRow dr in dtNewParts.Rows)
            {

                DataRow dr1 = dtParts.NewRow();
                dr1["Id"] = dtParts.Rows.Count + 1;
                dr1["UnitId"] = dr["UnitId"].ToString();
                dr1["UnitName"] = dr["UnitName"].ToString();
                if (!string.IsNullOrEmpty(dr["PartId"].ToString()))
                    dr1["PartId"] = dr["PartId"].ToString();
                dr1["PartName"] = dr["PartName"].ToString();
                dr1["PartSize"] = dr["PartSize"].ToString();
                dr1["Description"] = dr["Description"].ToString();
                dr1["Quantity"] = dr["Quantity"].ToString();
                dr1["ArrangedQuantity"] = dr["ArrangedQuantity"].ToString();
                dtParts.Rows.Add(dr1);
            }
            lstParts.DataSource = dtParts;
            lstParts.DataBind();
        }
    }
}