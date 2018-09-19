using Aircall.Common;
using ImageResizer;
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
    public partial class account_setting : System.Web.UI.Page
    {
        #region "Declaration"
        IClientService objClientService = ServiceFactory.ClientService;
        IStripeErrorLogService objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["ClientLoginCookie"] != null)
                {
                    BindClientProfile();
                }
                else
                    Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }
        }
        #endregion

        #region "Functions"
        private void BindClientProfile()
        {
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            DataTable dtClient = new DataTable();
            objClientService.GetClientById(ClientId, ref dtClient);
            if (dtClient.Rows.Count > 0)
            {
                txtFirstName.Text = dtClient.Rows[0]["FirstName"].ToString();
                txtLastName.Text = dtClient.Rows[0]["LastName"].ToString();
                txtEmail.Text = dtClient.Rows[0]["Email"].ToString();
                txtCompany.Text = dtClient.Rows[0]["Company"].ToString();
                hdnImage.Value = dtClient.Rows[0]["Image"].ToString();
                if (!string.IsNullOrEmpty(dtClient.Rows[0]["Image"].ToString()))
                    imgClient.Src = Application["SiteAddress"] + "uploads/profile/client/" + dtClient.Rows[0]["Image"].ToString();
                else
                    imgClient.Src = Application["SiteAddress"] + "client/images/place-holder-img@3x copy.png";
            }
        }
        #endregion

        #region "Events"
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    BizObjects.Client objClient = new BizObjects.Client();
                    if (fpImage.HasFile)
                    {
                        string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                        if (!AllowedFileExtensions.Contains(fpImage.FileName.Substring(fpImage.FileName.LastIndexOf('.'))))
                        {
                            dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                            dvMessage.Attributes.Add("class", "error");
                            dvMessage.Visible = true;
                            return;
                        }
                        else
                        {
                            string filenameOriginal = DateTime.Now.Ticks.ToString().Trim();
                            string fileName = fpImage.FileName;
                            string extension = System.IO.Path.GetExtension(fileName);
                            string extensionwithoutdot = extension.Remove(0, 1);

                            Instructions rsiphnWxH = new Instructions();
                            rsiphnWxH.Width = 300;
                            rsiphnWxH.Height = 300;
                            rsiphnWxH.Mode = FitMode.Stretch;
                            rsiphnWxH.Format = extensionwithoutdot;

                            ImageJob imjob = new ImageJob(fpImage.PostedFile.InputStream, Server.MapPath("~/uploads/profile/client/" + filenameOriginal + extension), rsiphnWxH);
                            imjob.CreateParentDirectory = true;
                            imjob.Build();
                            objClient.Image = filenameOriginal + extension;
                        }
                    }
                    else
                        objClient.Image = hdnImage.Value;

                    LoginModel login = Session["ClientLoginCookie"] as LoginModel;

                    objClient.Id = login.Id;// Convert.ToInt32(Request.Cookies["ClientLoginCookie"]["ClientId"].ToString());
                    objClient.FirstName = txtFirstName.Text.Trim();
                    objClient.LastName = txtLastName.Text.Trim();
                    objClient.UpdatedBy = login.Id;// Convert.ToInt32(Request.Cookies["ClientLoginCookie"]["ClientId"].ToString());
                    objClient.UpdatedByType = General.UserRoles.Client.GetEnumValue();
                    objClient.UpdatedDate = DateTime.Now;
                    objClient.Company = txtCompany.Text;

                    login.FullName = txtFirstName.Text.Trim() + " " + txtLastName.Text.Trim();
                    Session["ClientLoginCookie"] = login;
                    int ClientId = login.Id;// Convert.ToInt32(Request.Cookies["ClientLoginCookie"]["ClientId"].ToString());
                    try
                    {
                        DataTable dtClient = new DataTable();
                        objClientService.GetClientById(ClientId, ref dtClient);

                        var customerService = new StripeCustomerService();
                        var myCustomer = new StripeCustomerUpdateOptions();

                        myCustomer.Email = txtEmail.Text.Trim();
                        myCustomer.Description = objClient.FirstName + ' ' + objClient.LastName + " (" + txtEmail.Text.Trim() + ")";

                        StripeCustomer stripeCustomer = customerService.Update(dtClient.Rows[0]["StripeCustomerId"].ToString(), myCustomer);
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
                    objClientService.UpdateClientProfile(ref objClient);
                    Response.Redirect(Application["SiteAddress"].ToString() + "client/dashboard.aspx", false);
                }
                catch (Exception Ex)
                {
                    dvMessage.InnerHtml = Ex.Message.Trim();
                    dvMessage.Attributes.Add("class", "error");
                    dvMessage.Visible = true;
                }
            }
        }
        #endregion
    }
}