using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.IO;
using Aircall.Common;

namespace Aircall.partner
{
    public partial class Profile : System.Web.UI.Page
    {
        IPartnerService objPartnerService;
        IStateService objStateService;
        ICitiesService objCitiesService;
        IZipCodeService objZipCodeService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["PartnerLoginCookie"] != null)
                {
                    if (Request.QueryString["msg"] == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Your Profile updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    FillStateDropdown();
                    BindPartnerInfo();
                }
                else
                {
                    Response.Redirect(Application["SiteAddress"] + "partner/Login.aspx");
                }
            }
        }

        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtState = new DataTable();
            objStateService.GetAllStates(true, true,ref dtState);
            if (dtState.Rows.Count > 0)
            {
                drpState.DataSource = dtState;
                drpState.DataTextField = dtState.Columns["Name"].ToString();
                drpState.DataValueField = dtState.Columns["Id"].ToString();
                drpState.DataBind();
            }
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
        }

        private void BindPartnerInfo()
        {
            string Username = (Session["PartnerLoginCookie"] as LoginModel).Username; //Request.Cookies["PartnerLoginCookie"]["Username"].ToString();
            objPartnerService = ServiceFactory.PartnerService;
            DataTable dtPartner = new DataTable();
            objPartnerService.GetPartnerInfoByPartnerName(Username, "",ref dtPartner);
            if (dtPartner.Rows.Count > 0)
            {
                txtFirstname.Text = dtPartner.Rows[0]["FirstName"].ToString();
                txtLastname.Text = dtPartner.Rows[0]["LastName"].ToString();
                txtUsername.Text = dtPartner.Rows[0]["UserName"].ToString();
                txtEmail.Text = dtPartner.Rows[0]["Email"].ToString();
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
                txtPhone.Text = dtPartner.Rows[0]["PhoneNumber"].ToString();
                txtAffiliate.Text = dtPartner.Rows[0]["AssignedAffiliateId"].ToString();
                txtAffiliate.Enabled = false;
                ltrSalesComm.Text = dtPartner.Rows[0]["SalesCommission"].ToString();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["PartnerLoginCookie"] != null)
                    {
                        string Username = (Session["PartnerLoginCookie"] as LoginModel).Username;//Request.Cookies["PartnerLoginCookie"]["Username"].ToString();
                        string image = string.Empty;

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

                        if (fImage.HasFile)
                        {
                            string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                            if (!AllowedFileExtensions.Contains(fImage.FileName.Substring(fImage.FileName.LastIndexOf('.'))))
                            {
                                dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                            else
                            {
                                string filePath = Path.Combine(Server.MapPath("~/uploads/profile/partner/"), fImage.FileName.Replace(' ', '_'));
                                fImage.SaveAs(filePath);
                                image = fImage.FileName.Replace(' ', '_');
                            }
                        }
                        else
                        {
                            image = hdnImage.Value;
                        }

                        BizObjects.Partner objPartner = new BizObjects.Partner();

                        objPartner.FirstName = txtFirstname.Text.Trim();
                        objPartner.LastName = txtLastname.Text.Trim();
                        objPartner.UserName = txtUsername.Text.Trim();
                        objPartner.Email = txtEmail.Text.Trim();
                        objPartner.Image = image;
                        objPartner.CompanyName = txtCompany.Text.Trim();
                        objPartner.Address = txtAddress.Text.Trim();
                        objPartner.StateId = Convert.ToInt32(drpState.SelectedValue.ToString());
                        objPartner.CitiesId = Convert.ToInt32(drpCity.SelectedValue.ToString());
                        objPartner.ZipCode = txtZip.Text.Trim();
                        objPartner.PhoneNumber = txtPhone.Text.Trim();
                        objPartner.UpdatedBy = (Session["PartnerLoginCookie"] as LoginModel).Id;
                        objPartner.UpdatedByType = (Session["PartnerLoginCookie"] as LoginModel).RoleId;
                        objPartner.UpdatedDate = DateTime.UtcNow;

                        objPartnerService = ServiceFactory.PartnerService;
                        objPartnerService.UpdatePartnerProfile(ref objPartner);
                        LoginModel login = Session["PartnerLoginCookie"] as LoginModel;
                        
                        login.FullName = txtFirstname.Text.Trim() + " " + txtLastname.Text.Trim();
                        login.Username = txtUsername.Text.Trim();
                        
                        if (string.IsNullOrEmpty(image))
                            login.Image = Application["SiteAddress"] + "partner/img/avatar1_small.jpg";
                        else
                            login.Image = Application["SiteAddress"] + "uploads/profile/partner/" + image;

                        Session["PartnerLoginCookie"] = login;

                        Response.Redirect(Application["SiteAddress"] + "partner/Profile.aspx?msg=edit");
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "partner/Login.aspx");
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
            objCitiesService.GetAllCityByStateId(StateId, true,ref dtCity);
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
    }
}