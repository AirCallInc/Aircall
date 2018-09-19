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
    public partial class Area_AddEdit : System.Web.UI.Page
    {
        IStateService objStateService;
        ICitiesService objCitiesService;
        IZipCodeService objZipCodeService;
        IAreasService objAreasService;
        IWorkAreaService objWorkAreaService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dvChkAll.Visible = false;
                FillStateDropdown();

                if (!string.IsNullOrEmpty(Request.QueryString["AreaId"]))
                {
                    BindAreaById();
                }
            }
        }

        private void BindAreaById()
        {
            btnSave.Text = "Update";
            //btnUpdate.Visible = true;

            int AreaId = Convert.ToInt32(Request.QueryString["AreaId"].ToString());
            DataTable dtArea = new DataTable();
            objAreasService = ServiceFactory.AreasService;
            objAreasService.GetAreaById(AreaId, false,ref dtArea);
            if (dtArea.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dtArea.Rows[0]["Status"].ToString()))
                    btnSave.Visible = true;
                else
                    btnSave.Visible = false;

                txtAreaName.Text = dtArea.Rows[0]["Name"].ToString();

                drpState.SelectedValue = dtArea.Rows[0]["StateId"].ToString();
                BindCityFromState(Convert.ToInt32(dtArea.Rows[0]["StateId"].ToString()));

                drpCity.SelectedValue = dtArea.Rows[0]["CitiesId"].ToString();
                BindZipCodeFromCity(Convert.ToInt32(dtArea.Rows[0]["CitiesId"].ToString()));

                chkActive.Checked = Convert.ToBoolean(dtArea.Rows[0]["Status"].ToString());

                objWorkAreaService = ServiceFactory.WorkAreaService;
                DataTable dtWorkAreas = new DataTable();
                objWorkAreaService.GetAllWorkAreaByAreaId(AreaId, ref dtWorkAreas);

                if (dtWorkAreas.Rows.Count > 0)
                {
                    for (int i = 0; i < chkZipCodes.Items.Count; i++)
                    {
                        for (int j = 0; j < dtWorkAreas.Rows.Count; j++)
                        {
                            if (chkZipCodes.Items[i].Value.Contains(dtWorkAreas.Rows[j]["ZipCodeId"].ToString()))
                                chkZipCodes.Items[i].Selected = true;
                        }
                    }
                }
            }
        }

        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtStates = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["AreaId"]))
                objStateService.GetAllStates(true, true, ref dtStates);
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
            chkAll.Checked = false;
            dvChkAll.Visible = false;
            if (drpState.SelectedValue != "0")
            {
                BindCityFromState(Convert.ToInt32(drpState.SelectedValue.ToString()));
            }
            else
            {
                drpCity.DataSource = "";
                drpCity.DataBind();
                drpCity.Items.Insert(0, new ListItem("Select City", "0"));

                chkZipCodes.DataSource = "";
                chkZipCodes.DataBind();
            }
        }

        private void BindCityFromState(int StateId)
        {
            objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCities = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["AreaId"]))
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

        protected void drpCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkAll.Checked = false;
            if (drpCity.SelectedValue != "0")
            {
                BindZipCodeFromCity(Convert.ToInt32(drpCity.SelectedValue.ToString()));
            }
            else
            {
                chkZipCodes.DataSource = "";
                chkZipCodes.DataBind();
            }
        }

        private void BindZipCodeFromCity(int CityId)
        {
            objZipCodeService = ServiceFactory.ZipCodeService;
            DataTable dtZipCodes = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["AreaId"]))
                objZipCodeService.GetAllZipCodeByCityId(CityId, true,ref dtZipCodes);
            else
                objZipCodeService.GetAllZipCodeByCityId(CityId, false,ref dtZipCodes);
            if (dtZipCodes.Rows.Count > 0)
            {
                chkZipCodes.DataSource = dtZipCodes;
                chkZipCodes.DataValueField = dtZipCodes.Columns["Id"].ToString();
                chkZipCodes.DataTextField = dtZipCodes.Columns["ZipCode"].ToString();
                chkZipCodes.DataBind();
                //chkZipCodes.Items.Insert(0, new ListItem("Select All", "0"));
                dvChkAll.Visible = true;
            }
            else
            {
                dvChkAll.Visible = false;
                chkZipCodes.DataSource = "";
                chkZipCodes.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    //if (Request.Cookies["LoginCookie"]["UserId"] != null)
                    //{
                    if (Session["LoginSession"]!=null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;
                        objAreasService = ServiceFactory.AreasService;
                        objWorkAreaService = ServiceFactory.WorkAreaService;
                        DataTable dtAreas = new DataTable();
                        BizObjects.Areas objAreas = new BizObjects.Areas();

                        if (string.IsNullOrEmpty(Request.QueryString["AreaId"]))
                        {
                            objAreasService.GetAreaByName(txtAreaName.Text.Trim(), ref dtAreas);
                            if (dtAreas.Rows.Count > 0)
                            {
                                dvMessage.InnerHtml = "<strong>Area name already exists.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                        }

                        if (chkZipCodes.Items.Count == 0 || chkZipCodes.SelectedIndex == -1)
                        {
                            dvMessage.InnerHtml = "<strong>Please Select Zip code.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        objAreas.Name = txtAreaName.Text.Trim();
                        objAreas.StateId = Convert.ToInt32(drpState.SelectedValue.ToString());
                        objAreas.CitiesId = Convert.ToInt32(drpCity.SelectedValue.ToString());
                        objAreas.Status = chkActive.Checked;
                        objAreas.AddedBy = objLoginModel.Id;
                        objAreas.AddedByType = objLoginModel.RoleId;
                        objAreas.AddedDate = DateTime.UtcNow;

                        int AreaId = 0;
                        if (!string.IsNullOrEmpty(Request.QueryString["AreaId"]))
                        {
                            AreaId = Convert.ToInt32(Request.QueryString["AreaId"].ToString());
                            objAreas.Id = AreaId;
                            objAreas.UpdatedBy = objLoginModel.Id;
                            objAreas.UpdatedByType = objLoginModel.RoleId;
                            objAreas.UpdatedDate = DateTime.UtcNow;
                            objAreasService.UpdateArea(ref objAreas);
                        }
                        else
                        {
                            AreaId = objAreasService.AddArea(ref objAreas);
                        }

                        BizObjects.WorkAreas objWorkArea = new BizObjects.WorkAreas();
                        

                        //bool SelectAll = false;
                        //if (chkZipCodes.Items.Count > 0 && chkZipCodes.Items[0].Selected)
                        //    SelectAll = true;

                        for (int i = 0; i < chkZipCodes.Items.Count; i++)
                        {
                            //if (SelectAll)
                            //{
                            //    objWorkArea.AreaId = AreaId;
                            //    objWorkArea.ZipCodeId = Convert.ToInt32(chkZipCodes.Items[i].Value);
                            //    objWorkAreaService.AddWorkAreaa(ref objWorkArea);
                            //}
                            //else
                            //{
                                if (chkZipCodes.Items[i].Selected)
                                {
                                    objWorkArea.AreaId = AreaId;
                                    objWorkArea.ZipCodeId = Convert.ToInt32(chkZipCodes.Items[i].Value);
                                    objWorkAreaService.AddWorkAreaa(ref objWorkArea);
                                }
                            //}
                        }

                        if (string.IsNullOrEmpty(Request.QueryString["AreaId"]))
                        {
                            Session["msg"] = "add";
                            Response.Redirect(Application["SiteAddress"] + "admin/Area_List.aspx");
                        }
                        else
                        {
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/Area_List.aspx");
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

        protected void chkZipCodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkAll.Checked = false;
            //if (chkZipCodes.SelectedItem.Value == "0")
            //{
            //    for (int i = 1; i < chkZipCodes.Items.Count; i++)
            //    {
            //        chkZipCodes.Items[i].Selected = chkZipCodes.Items[0].Selected;
            //    }
            //}
        }

        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < chkZipCodes.Items.Count; i++)
            {
                chkZipCodes.Items[i].Selected = chkAll.Checked;
            }
        }
    }
}