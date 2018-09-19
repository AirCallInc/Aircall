using Aircall.Common;
using ImageResizer;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class Partner_AddEdit : System.Web.UI.Page
    {
        IStateService objStateService;
        ICitiesService objCitiesService;
        IPartnerService objPartnerService;
        IClientService objClientService;
        IZipCodeService objZipCodeService;
        IEmailTemplateService objEmailTemplateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillStateDropdown();
                drpCity.Items.Insert(0, new ListItem("Select City", "0"));
                if (!string.IsNullOrEmpty(Request.QueryString["PartnerId"]))
                {
                    if (Session["msg"] != null)
                    {
                        if (Session["msg"].ToString() == "add")
                        {
                            dvMessage.InnerHtml = "<strong>Client Added to Partner Affiliate.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-success");
                            dvMessage.Visible = true;
                        }
                        if (Session["msg"].ToString() == "removed")
                        {
                            dvMessage.InnerHtml = "<strong>Client removed from Partner Affiliate.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-success");
                            dvMessage.Visible = true;
                        } 
                    }
                    BindPartnerInfo();

                    BindAffiliateClients();
                }
                else
                    clientAdd.Visible = false;
            }
        }

        private void BindAffiliateClients()
        {
            int PartnerId = Convert.ToInt32(Request.QueryString["PartnerId"].ToString());
            objClientService = ServiceFactory.ClientService;
            DataTable dtClient = new DataTable();
            objClientService.GetAllClientByPartnerId(PartnerId, ref dtClient);
            if (dtClient.Rows.Count > 0)
            {
                lstClients.DataSource = dtClient;
            }
            lstClients.DataBind();
        }

        private void BindPartnerInfo()
        {

            int PartnerId = Convert.ToInt32(Request.QueryString["PartnerId"].ToString());
            DataTable dtPartner = new DataTable();
            objPartnerService = ServiceFactory.PartnerService;
            objPartnerService.GetPartnerById(PartnerId, ref dtPartner);
            if (dtPartner.Rows.Count > 0)
            {
                btnAdd.Text = "Update";
                rqfvPassword.Enabled = false;
                txtEmail.Enabled = false;
                txtUserName.Enabled = false;
                txtFName.Text = dtPartner.Rows[0]["FirstName"].ToString();
                txtLName.Text = dtPartner.Rows[0]["LastName"].ToString();
                txtUserName.Text = dtPartner.Rows[0]["UserName"].ToString();
                txtEmail.Text = dtPartner.Rows[0]["Email"].ToString();
                hdnPassword.Value = dtPartner.Rows[0]["Password"].ToString();
                txtCompany.Text = dtPartner.Rows[0]["CompanyName"].ToString();
                if (!string.IsNullOrEmpty(dtPartner.Rows[0]["Image"].ToString()))
                {
                    lnkImage.HRef = Application["SiteAddress"] + "uploads/profile/partner/" + dtPartner.Rows[0]["Image"].ToString();
                    lnkImage.Visible = true;
                }
                hdnImage.Value = dtPartner.Rows[0]["Image"].ToString();
                txtAddress.Text = dtPartner.Rows[0]["Address"].ToString();
                drpState.SelectedValue = dtPartner.Rows[0]["StateId"].ToString();
                FillCityDropdownByStateId(Convert.ToInt32(dtPartner.Rows[0]["StateId"].ToString()));
                drpCity.SelectedValue = dtPartner.Rows[0]["CitiesId"].ToString();
                txtZip.Text = dtPartner.Rows[0]["ZipCode"].ToString();
                txtMob.Text = dtPartner.Rows[0]["PhoneNumber"].ToString();
                txtAffiliate.Text = dtPartner.Rows[0]["AssignedAffiliateId"].ToString();
                txtCommission.Text = dtPartner.Rows[0]["SalesCommission"].ToString();
                chkActive.Checked = Convert.ToBoolean(dtPartner.Rows[0]["IsActive"].ToString());
            }
        }

        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtState = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["PartnerId"]))
                objStateService.GetAllStates(true, true,ref dtState);
            else
                objStateService.GetAllStates(true, false,ref dtState);
            if (dtState.Rows.Count > 0)
            {
                drpState.DataSource = dtState;
                drpState.DataTextField = dtState.Columns["Name"].ToString();
                drpState.DataValueField = dtState.Columns["Id"].ToString();
            }
            drpState.DataBind();
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
        }

        protected void drpState_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpState.SelectedValue != "0")
            {
                FillCityDropdownByStateId(Convert.ToInt32(drpState.SelectedValue.ToString()));
            }
            else
            {
                drpCity.DataSource = "";
                drpCity.DataBind();
                drpCity.Items.Insert(0, new ListItem("Select City", "0"));
            }
        }

        private void FillCityDropdownByStateId(int StateId)
        {
            objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCity = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["PartnerId"]))
                objCitiesService.GetAllCityByStateId(StateId, true,ref dtCity);
            else
                objCitiesService.GetAllCityByStateId(StateId, false,ref dtCity);
            if (dtCity.Rows.Count > 0)
            {
                drpCity.DataSource = dtCity;
                drpCity.DataTextField = dtCity.Columns["Name"].ToString();
                drpCity.DataValueField = dtCity.Columns["Id"].ToString();
            }
            else
                drpCity.DataSource = "";
            drpCity.DataBind();
            drpCity.Items.Insert(0, new ListItem("Select City", "0"));
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        objPartnerService = ServiceFactory.PartnerService;
                        BizObjects.Partner objPartner = new BizObjects.Partner();

                        //objZipCodeService = ServiceFactory.ZipCodeService;
                        //DataTable dtZipCode = new DataTable();
                        //objZipCodeService.CheckValidZipCode(Convert.ToInt32(drpState.SelectedValue), Convert.ToInt32(drpCity.SelectedValue), txtZip.Text.Trim(), ref dtZipCode);
                        //if (dtZipCode.Rows.Count == 0)
                        //{
                        //    dvMessage.InnerHtml = "<strong>Please enter a valid Zip Code.</strong>";
                        //    dvMessage.Attributes.Add("class", "alert alert-error");
                        //    dvMessage.Visible = true;
                        //    return;
                        //}
                        if (string.IsNullOrEmpty(Request.QueryString["PartnerId"]))
                        {
                            DataTable dtPartner = new DataTable();
                            objPartnerService.GetPartnerInfoByPartnerName(txtUserName.Text.Trim(), txtEmail.Text.Trim(), ref dtPartner);
                            if (dtPartner.Rows.Count > 0)
                            {
                                dvMessage.InnerHtml = "<strong>Username Or Email already exists.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }

                        if (fpImage.HasFile)
                        {
                            string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                            if (!AllowedFileExtensions.Contains(fpImage.FileName.Substring(fpImage.FileName.LastIndexOf('.'))))
                            {
                                dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                            else
                            {
                                string filenameOriginal = DateTime.UtcNow.Ticks.ToString().Trim();
                                string fileName = fpImage.FileName;
                                string extension = System.IO.Path.GetExtension(fileName);
                                string extensionwithoutdot = extension.Remove(0, 1);

                                Instructions rsiphnWxH = new Instructions();
                                rsiphnWxH.Width = 200;
                                rsiphnWxH.Height = 200;
                                rsiphnWxH.Mode = FitMode.Stretch;
                                rsiphnWxH.Format = extensionwithoutdot;

                                ImageJob imjob = new ImageJob(fpImage.PostedFile.InputStream, Server.MapPath("~/uploads/profile/partner/" + filenameOriginal + extension), rsiphnWxH);
                                imjob.CreateParentDirectory = true;
                                imjob.Build();
                                objPartner.Image = filenameOriginal + extension;
                            }
                        }
                        else
                            objPartner.Image = hdnImage.Value;

                        
                        objPartner.RoleId = (int)General.UserRoles.Partner;
                        objPartner.FirstName = txtFName.Text.Trim();
                        objPartner.LastName = txtLName.Text.Trim();
                        objPartner.UserName = txtUserName.Text.Trim();
                        objPartner.Email = txtEmail.Text.Trim();
                        if (string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
                        {
                            if (!string.IsNullOrEmpty(txtPassword.Text.Trim()))
                            {
                                using (MD5 md5Hash = MD5.Create())
                                {
                                    objPartner.Password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                                }
                            }
                            else
                            {
                                objPartner.Password = hdnPassword.Value;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(txtPassword.Text.Trim()))
                            {
                                using (MD5 md5Hash = MD5.Create())
                                {
                                    objPartner.Password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                                }
                            }
                            else
                            {
                                objPartner.Password = hdnPassword.Value;
                            }
                        }
                        objPartner.CompanyName = txtCompany.Text.Trim();
                        objPartner.Address = txtAddress.Text.Trim();
                        objPartner.StateId = Convert.ToInt32(drpState.SelectedValue.ToString());
                        objPartner.CitiesId = Convert.ToInt32(drpCity.SelectedValue.ToString());
                        objPartner.ZipCode = txtZip.Text.Trim();
                        objPartner.PhoneNumber = txtMob.Text.Trim();
                        objPartner.AssignedAffiliateId = txtAffiliate.Text.Trim();
                        objPartner.SalesCommission = Convert.ToDecimal(txtCommission.Text.Trim());
                        objPartner.IsActive = chkActive.Checked;
                        objPartner.AddedBy = objLoginModel.Id;
                        objPartner.AddedByType = objLoginModel.RoleId;
                        objPartner.AddedDate = DateTime.UtcNow;

                        int PartnerId = 0;
                        if (!string.IsNullOrEmpty(Request.QueryString["PartnerId"]))
                        {
                            PartnerId = Convert.ToInt32(Request.QueryString["PartnerId"].ToString());
                            objPartner.Id = PartnerId;
                            objPartner.UpdatedBy = objLoginModel.Id;
                            objPartner.UpdatedByType = objLoginModel.RoleId;
                            objPartner.UpdatedDate = DateTime.UtcNow;

                            objPartnerService.UpdatePartner(ref objPartner);

                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/Partner_List.aspx");
                        }
                        else
                        {
                            PartnerId = objPartnerService.AddPartner(ref objPartner);

                            DataTable dtEmailTemplate = new DataTable();
                            objEmailTemplateService = ServiceFactory.EmailTemplateService;
                            objEmailTemplateService.GetByName("NewUserPartner", ref dtEmailTemplate);
                            if (dtEmailTemplate.Rows.Count > 0)
                            {
                                string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                                string CCEmail = dtEmailTemplate.Rows[0]["CCEmails"].ToString();
                                EmailBody = EmailBody.Replace("{{FirstName}}", txtFName.Text.Trim());
                                EmailBody = EmailBody.Replace("{{LastName}}", txtLName.Text.Trim());
                                EmailBody = EmailBody.Replace("{{RegisterDate}}", DateTime.Now.ToString("MMMM dd, yyyy"));
                                EmailBody = EmailBody.Replace("{{Username}}", txtUserName.Text.Trim());
                                EmailBody = EmailBody.Replace("{{Email}}", txtEmail.Text.Trim());
                                EmailBody = EmailBody.Replace("{{Password}}", txtPassword.Text.Trim());
                                EmailBody = EmailBody.Replace("{{PhoneNumber}}", txtMob.Text.Trim());
                                EmailBody = EmailBody.Replace("{{Link}}", Application["SiteAddress"] + "partner/Login.aspx");
                                Email.SendEmail(dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString(), txtEmail.Text.ToString(), CCEmail, "", EmailBody);
                            }

                            //string Emailbody = "Partner Successfully added into system.<br/>Email: " + txtEmail.Text.Trim() + "<br/>Password: " + txtPassword.Text.Trim();
                            //Email.SendEmail("Partner Added into System", txtEmail.Text.Trim(), "", "", Emailbody);
                            //Email.Send("testlocalcoding@gmail.com", "this.admin", "Partner Added into System", txtEmail.Text.Trim(), Emailbody, "smtp.gmail.com", 587, true);
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/Partner_List.aspx");
                        }
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void lnkSearch_Click(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
            {
                DataTable dtClients = new DataTable();
                objClientService = ServiceFactory.ClientService;
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
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null &&
                                !string.IsNullOrEmpty(Request.QueryString["PartnerId"]))
                    {
                        if (rblClient.Items.Count == 0 || rblClient.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Client</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        BizObjects.Client objClient = new BizObjects.Client();
                        objClientService = ServiceFactory.ClientService;
                        objClient.Id = Convert.ToInt32(rblClient.SelectedValue);
                        objClient.AffiliateId = Convert.ToInt32(Request.QueryString["PartnerId"].ToString());
                        objClient.UpdatedBy = objLoginModel.Id;
                        objClient.UpdatedByType = objLoginModel.RoleId;
                        objClient.UpdatedDate = DateTime.UtcNow;

                        int rtn=objClientService.AssignAffiliate(ref objClient);

                        if (rtn == 0)
                        {
                            dvMessage.InnerHtml = "<strong>Client already assigned to another Partner.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                        }
                        else
                        {
                            Session["msg"] = "add";
                            Response.Redirect(Application["SiteAddress"] + "admin/Partner_AddEdit.aspx?PartnerId=" + Request.QueryString["PartnerId"].ToString() + "");
                        }
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
                    }
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.ToString().Trim();
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                }
            }
        }

        protected void lstClients_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["PartnerId"]))
            {
                if (e.CommandName == "RemoveAffiliate")
                {
                    LoginModel objLoginModel = new LoginModel();
                    objLoginModel = Session["LoginSession"] as LoginModel;

                    int ClientId = Convert.ToInt32(e.CommandArgument.ToString());
                    BizObjects.Client objClient = new BizObjects.Client();
                    objClientService = ServiceFactory.ClientService;
                    objClientService = ServiceFactory.ClientService;
                    objClient.Id = ClientId;
                    objClient.AffilateDeletedBy = objLoginModel.Id;
                    objClient.AffilateDeletedDate = DateTime.UtcNow;
                    objClientService.RemoveAffiliate(ref objClient);
                    Session["msg"] = "removed";
                    Response.Redirect(Application["SiteAddress"] + "admin/Partner_AddEdit.aspx?PartnerId=" + Request.QueryString["PartnerId"].ToString() + "");
                }
                
            }
        }
    }
}