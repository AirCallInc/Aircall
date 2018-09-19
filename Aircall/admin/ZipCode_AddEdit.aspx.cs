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
    public partial class ZipCode_AddEdit : System.Web.UI.Page
    {
        IStateService objStateService;
        ICitiesService objCitiesService;
        IZipCodeService objZipCodeService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillStateDropdown();

                if (!string.IsNullOrEmpty(Request.QueryString["ZipId"]))
                {
                    BindZipCodeById();
                }
            }
        }

        private void BindZipCodeById()
        {
            btnSave.Text = "Update";
            //btnUpdate.Visible = true;

            int ZipId = Convert.ToInt32(Request.QueryString["ZipId"].ToString());
            DataTable dtZipCode = new DataTable();
            objZipCodeService = ServiceFactory.ZipCodeService;
            objZipCodeService.GetById(ZipId, ref dtZipCode);
            if (dtZipCode.Rows.Count > 0)
            {
                drpState.SelectedValue = dtZipCode.Rows[0]["StateId"].ToString();
                BindCityFromState(Convert.ToInt32(dtZipCode.Rows[0]["StateId"].ToString()));
                drpCity.SelectedValue = dtZipCode.Rows[0]["CitiesId"].ToString();
                hdnZip.Value = txtZip.Text = dtZipCode.Rows[0]["ZipCode"].ToString();
                chkActive.Checked = Convert.ToBoolean(dtZipCode.Rows[0]["DisplayStatus"].ToString());
                //chkPendingInactive.Checked = Convert.ToBoolean(dtZipCode.Rows[0]["PendingInactive"].ToString());
            }
        }

        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtStates = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["ZipId"]))
                objStateService.GetAllStates(true, true,ref dtStates);
            else
                objStateService.GetAllStates(true, false,ref dtStates);
            if (dtStates.Rows.Count > 0)
            {
                drpState.DataSource = dtStates;
                drpState.DataTextField = dtStates.Columns["Name"].ToString();
                drpState.DataValueField = dtStates.Columns["Id"].ToString();
            }
            drpState.DataBind();
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
        }

        protected void drpState_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpState.SelectedValue != "0")
            {
                BindCityFromState(Convert.ToInt32(drpState.SelectedValue.ToString()));
            }
        }

        private void BindCityFromState(int StateId)
        {
            objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCities = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["ZipId"]))
                objCitiesService.GetAllCityByStateId(StateId, true,ref dtCities);
            else
                objCitiesService.GetAllCityByStateId(StateId, false,ref dtCities);

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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        objZipCodeService = ServiceFactory.ZipCodeService;
                        DataTable dtZipCodes = new DataTable();
                        
                        if (string.IsNullOrEmpty(Request.QueryString["ZipId"]))
                        {
                            objZipCodeService.GetByZipCode(Convert.ToInt32(drpState.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()), txtZip.Text.Trim(), ref dtZipCodes);
                            if (dtZipCodes.Rows.Count > 0)
                            {
                                dvMessage.InnerHtml = "<strong>Zip Code already exists.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }
                        else
                        {
                            if (hdnZip.Value != txtZip.Text)
                            {
                                objZipCodeService.GetByZipCode(Convert.ToInt32(drpState.SelectedValue.ToString()), Convert.ToInt32(drpCity.SelectedValue.ToString()), txtZip.Text.Trim(), ref dtZipCodes);
                                if (dtZipCodes.Rows.Count > 0)
                                {
                                    dvMessage.InnerHtml = "<strong>Zip Code already exists.</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                            }
                        }

                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        BizObjects.ZipCodes objZipCode = new BizObjects.ZipCodes();
                        objZipCode.StateId = Convert.ToInt32(drpState.SelectedValue.ToString());
                        objZipCode.CitiesId = Convert.ToInt32(drpCity.SelectedValue.ToString());
                        objZipCode.ZipCode = txtZip.Text.Trim();
                        objZipCode.Status = chkActive.Checked;
                        //objZipCode.PendingInactive = chkPendingInactive.Checked;
                        objZipCode.AddedBy = objLoginModel.Id;
                        objZipCode.AddedByType = objLoginModel.RoleId;
                        objZipCode.AddedDate = DateTime.UtcNow;

                        if (!string.IsNullOrEmpty(Request.QueryString["ZipId"]))
                        {
                            int ZipId = Convert.ToInt32(Request.QueryString["ZipId"].ToString());
                            objZipCode.Id = ZipId;
                            objZipCode.UpdatedBy = objLoginModel.Id;
                            objZipCode.UpdatedByType = objLoginModel.RoleId;
                            objZipCode.UpdatedDate = DateTime.UtcNow;

                            objZipCodeService.UpdateZipCode(ref objZipCode);
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/ZipCode_List.aspx");
                        }
                        else
                        {
                            objZipCodeService.AddZipCode(ref objZipCode);
                            Session["msg"] = "add";
                            Response.Redirect(Application["SiteAddress"] + "admin/ZipCode_List.aspx");
                        }
                    }
                    else
                    {
                        Response.Redirect(Application["SiteAddress"] + "/admin/Login.aspx");
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