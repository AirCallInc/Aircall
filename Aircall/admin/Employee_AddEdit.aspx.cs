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
    public partial class Employee_AddEdit : System.Web.UI.Page
    {
        IStateService objStateService;
        ICitiesService objCitiesService;
        IEmployeeService objEmployeeService;
        IPlanService objPlanService;
        IEmployeePlanTypeService objEmployeePlanTypeService;
        IZipCodeService objZipCodeService;
        IEmailTemplateService objEmailTemplateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillStateDropdown();
                FillPlanTypeListbox();

                if (!string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
                {
                    BindEmployeeById();
                }
            }
        }

        private void FillPlanTypeListbox()
        {
            objPlanService = ServiceFactory.PlanService;
            DataTable dtPlanType = new DataTable();
            objPlanService.GetAllPlanType(ref dtPlanType);
            if (dtPlanType.Rows.Count > 0)
            {
                lstPlanType.DataSource = dtPlanType;
                lstPlanType.DataTextField = dtPlanType.Columns["Name"].ToString();
                lstPlanType.DataValueField = dtPlanType.Columns["Id"].ToString();
            }
            lstPlanType.DataBind();
        }

        private void BindEmployeeById()
        {
            int EmployeeId = Convert.ToInt32(Request.QueryString["EmployeeId"]);
            objEmployeeService = ServiceFactory.EmployeeService;
            objEmployeePlanTypeService = ServiceFactory.EmployeePlanTypeService;
            DataTable dtEmployee = new DataTable();
            objEmployeeService.GetEmployeeById(EmployeeId, ref dtEmployee);
            if (dtEmployee.Rows.Count > 0)
            {
                btnAdd.Text = "Update";

                txtFName.Text = dtEmployee.Rows[0]["FirstName"].ToString();
                txtLName.Text = dtEmployee.Rows[0]["LastName"].ToString();
                txtEmail.Text = dtEmployee.Rows[0]["Email"].ToString();
                //txtEmail.Enabled = false;
                rqfvPassword.Enabled = false;
                hdnPassword.Value = dtEmployee.Rows[0]["Password"].ToString();
                txtAddress.Text = dtEmployee.Rows[0]["Address"].ToString();
                drpState.SelectedValue = dtEmployee.Rows[0]["StateId"].ToString();
                //FillCityDropdownByStateId(Convert.ToInt32(dtEmployee.Rows[0]["StateId"].ToString()));
                txtCity.Text = dtEmployee.Rows[0]["City"].ToString();
                //drpCity.SelectedValue = dtEmployee.Rows[0]["CitiesId"].ToString();
                txtZip.Text = dtEmployee.Rows[0]["ZipCode"].ToString();
                txtPhone.Text = dtEmployee.Rows[0]["PhoneNumber"].ToString();
                txtMob.Text = dtEmployee.Rows[0]["MobileNumber"].ToString();
                txtStart.Value = dtEmployee.Rows[0]["WorkStartTime"].ToString();
                txtEnd.Value = dtEmployee.Rows[0]["WorkEndTime"].ToString();
                if (!string.IsNullOrEmpty(dtEmployee.Rows[0]["Image"].ToString()))
                {
                    lnkImage.HRef = Application["SiteAddress"] + "uploads/profile/employee/" + dtEmployee.Rows[0]["Image"].ToString();
                    lnkImage.Visible = true;
                }
                hdnImage.Value = dtEmployee.Rows[0]["Image"].ToString();
                chkIsSales.Checked = Convert.ToBoolean(dtEmployee.Rows[0]["IsSalesPerson"].ToString());
                chkActive.Checked = Convert.ToBoolean(dtEmployee.Rows[0]["IsActive"].ToString());
            }
            DataTable dtPlanType= new DataTable();
            objEmployeePlanTypeService.GetAllPlanTypeByEmployeeId(EmployeeId, ref dtPlanType);
            if (dtPlanType.Rows.Count>0)
            {
                for (int i = 0; i < dtPlanType.Rows.Count; i++)
                {
                    foreach (ListItem item in lstPlanType.Items)
                    {
                        if (item.Value==dtPlanType.Rows[i]["PlanTypeId"].ToString())
                            item.Selected = true;
                    }
                }
            }
        }

        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtState = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
                objStateService.GetAllStates(true, true,ref dtState);
            else
                objStateService.GetAllStates(true, false, ref dtState);
            if (dtState.Rows.Count > 0)
            {
                drpState.DataSource = dtState;
                drpState.DataTextField = dtState.Columns["Name"].ToString();
                drpState.DataValueField = dtState.Columns["Id"].ToString();
            }
            drpState.DataBind();
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
        }

        //protected void drpState_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (drpState.SelectedValue != "0")
        //    {
        //        FillCityDropdownByStateId(Convert.ToInt32(drpState.SelectedValue.ToString()));
        //    }
        //    else
        //    {
        //        drpCity.DataSource = "";
        //        drpCity.DataBind();
        //        drpCity.Items.Insert(0, new ListItem("Select City", "0"));
        //    }
        //}

        //private void FillCityDropdownByStateId(int StateId)
        //{
        //    objCitiesService = ServiceFactory.CitiesService;
        //    DataTable dtCity = new DataTable();
        //    if (!string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
        //        objCitiesService.GetAllCityByStateId(StateId, true,ref dtCity);
        //    else
        //        objCitiesService.GetAllCityByStateId(StateId, false,ref dtCity);
        //    if (dtCity.Rows.Count > 0)
        //    {
        //        drpCity.DataSource = dtCity;
        //        drpCity.DataTextField = dtCity.Columns["Name"].ToString();
        //        drpCity.DataValueField = dtCity.Columns["Id"].ToString();
        //    }
        //    else
        //        drpCity.DataSource = "";
        //    drpCity.DataBind();
        //    drpCity.Items.Insert(0, new ListItem("Select City", "0"));
        //}

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

                        int EmployeeId = 0;
                        BizObjects.Employee objEmployee = new BizObjects.Employee();
                        BizObjects.EmployeePlanType objEmployeePlanType = new BizObjects.EmployeePlanType();

                        objEmployeeService = ServiceFactory.EmployeeService;
                        objEmployeePlanTypeService = ServiceFactory.EmployeePlanTypeService;

                        DateTime StartTime = new DateTime();
                        DateTime EndTime = new DateTime();
                        string Start = string.Empty;
                        string End = string.Empty;

                        Start = DateTime.Now.Date.ToShortDateString() + " " + txtStart.Value.Trim();
                        StartTime = Convert.ToDateTime(Start);
                        End = DateTime.Now.Date.ToShortDateString() + " " + txtEnd.Value.Trim();
                        EndTime = Convert.ToDateTime(End);

                        if (StartTime == EndTime)
                        {
                            dvMessage.InnerHtml = "<strong>Employee Start Time is same as End Time.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        if ((txtStart.Value.Trim().Contains("PM") && txtEnd.Value.Trim().Contains("PM"))||
                            (txtStart.Value.Trim().Contains("AM") && txtEnd.Value.Trim().Contains("AM")))
                        {
                            if (StartTime > EndTime)
                            {
                                dvMessage.InnerHtml = "<strong>Work End Time must be greater than Start Time.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }

                        //Validation of Zipcode Start
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
                        //Validation of Zipcode End


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

                                ImageJob imjob = new ImageJob(fpImage.PostedFile.InputStream, Server.MapPath("~/uploads/profile/employee/" + filenameOriginal + extension), rsiphnWxH);
                                imjob.CreateParentDirectory = true;
                                imjob.Build();
                                objEmployee.Image = filenameOriginal + extension;
                            }
                        }
                        else
                        {
                            objEmployee.Image = hdnImage.Value;
                        }

                        if (string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
                        {
                            DataTable dtEmployee = new DataTable();
                            objEmployeeService.GetEmployeeByEmail(txtEmail.Text.Trim(), ref dtEmployee);
                            if (dtEmployee.Rows.Count > 0)
                            {
                                dvMessage.InnerHtml = "<strong>Email addresss already exist.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }
                        else
                        {
                            DataTable dtEmployee = new DataTable();
                            objEmployeeService.GetEmployeeByEmail(txtEmail.Text.Trim(), ref dtEmployee);
                            if (dtEmployee.Rows.Count > 0 && dtEmployee.Rows[0]["Id"].ToString() != Request.QueryString["EmployeeId"].ToString())
                            {
                                dvMessage.InnerHtml = "<strong>Email addresss already exist.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }

                        }

                        objEmployee.RoleId = (int)General.UserRoles.Employee;
                        objEmployee.FirstName = txtFName.Text.Trim();
                        objEmployee.LastName = txtLName.Text.Trim();
                        objEmployee.UserName = txtEmail.Text.Trim();
                        objEmployee.Email = txtEmail.Text.Trim();
                        if (string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
                        {
                            using (MD5 md5Hash = MD5.Create())
                            {
                                objEmployee.Password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(txtPassword.Text.Trim()))
                            {
                                using (MD5 md5Hash = MD5.Create())
                                {
                                    objEmployee.Password = Md5Encrypt.GetMd5Hash(md5Hash, txtPassword.Text.Trim());
                                }
                            }
                            else
                            {
                                objEmployee.Password = hdnPassword.Value;
                            }
                        }
                        objEmployee.Address = txtAddress.Text.Trim();
                        objEmployee.StateId = Convert.ToInt32(drpState.SelectedValue.ToString());
                        objEmployee.City = txtCity.Text.Trim();
                        //objEmployee.CitiesId = Convert.ToInt32(drpCity.SelectedValue.ToString());
                        objEmployee.ZipCode = txtZip.Text.Trim();
                        objEmployee.MobileNumber = txtMob.Text.Trim();
                        objEmployee.PhoneNumber = txtPhone.Text.Trim();
                        objEmployee.WorkStartTime = txtStart.Value.Trim();
                        objEmployee.WorkEndTime = txtEnd.Value.Trim();
                        objEmployee.IsSalesPerson = chkIsSales.Checked;
                        objEmployee.IsActive = chkActive.Checked;
                        objEmployee.AddedBy = objLoginModel.Id ;
                        objEmployee.AddedByType = objLoginModel.RoleId;
                        objEmployee.AddedDate = DateTime.UtcNow;


                        if (!string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
                        {
                            EmployeeId = Convert.ToInt32(Request.QueryString["EmployeeId"].ToString());
                            objEmployee.Id = Convert.ToInt32(Request.QueryString["EmployeeId"].ToString());
                            objEmployee.UpdatedBy = objLoginModel.Id;
                            objEmployee.UpdatedByType = objLoginModel.RoleId;
                            objEmployee.UpdatedDate = DateTime.UtcNow;

                            objEmployeeService.UpdateEmployee(ref objEmployee);
                        }
                        else
                        {
                            EmployeeId = objEmployeeService.AddEmployee(ref objEmployee);

                            DataTable dtEmailTemplate = new DataTable();
                            objEmailTemplateService = ServiceFactory.EmailTemplateService;
                            objEmailTemplateService.GetByName("NewUserEmployee", ref dtEmailTemplate);
                            if (dtEmailTemplate.Rows.Count > 0)
                            {
                                string EmailBody = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                                string CCEmail = dtEmailTemplate.Rows[0]["CCEmails"].ToString();
                                EmailBody = EmailBody.Replace("{{FirstName}}", txtFName.Text.Trim());
                                EmailBody = EmailBody.Replace("{{LastName}}", txtLName.Text.Trim());
                                EmailBody = EmailBody.Replace("{{RegisterDate}}", DateTime.Now.ToString("MMMM dd, yyyy"));
                                EmailBody = EmailBody.Replace("{{Email}}", txtEmail.Text.Trim());
                                EmailBody = EmailBody.Replace("{{Password}}", txtPassword.Text.Trim());
                                EmailBody = EmailBody.Replace("{{PhoneNumber}}", txtMob.Text.Trim());
                                Email.SendEmail(dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString(), txtEmail.Text.ToString(), CCEmail, "", EmailBody);
                            }
                        }

                        foreach (ListItem item in lstPlanType.Items)
                        {
                            if (item.Selected)
                            {
                                objEmployeePlanType.EmployeeId = EmployeeId;
                                objEmployeePlanType.PlanTypeId = Convert.ToInt32(item.Value);
                                objEmployeePlanTypeService.AddEmployeePlanType(ref objEmployeePlanType);
                            }
                        }

                        if (!string.IsNullOrEmpty(Request.QueryString["EmployeeId"]))
                        {
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/Employee_List.aspx");
                        }
                        else
                        {
                            Session["msg"] = "add";
                            Response.Redirect(Application["SiteAddress"] + "admin/Employee_List.aspx");
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
    }
}