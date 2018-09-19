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
    public partial class City_AddEdit : System.Web.UI.Page
    {
        IStateService objStateService;
        ICitiesService objCitiesService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillStateDropdown();

                if (!string.IsNullOrEmpty(Request.QueryString["CityId"]))
                {
                    BindCityByCityId();
                }
            }
        }

        private void BindCityByCityId()
        {
            btnSave.Text = "Update";
            //btnUpdate.Visible = true;

            int CityId = Convert.ToInt32(Request.QueryString["CityId"].ToString());
            objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCity = new DataTable();
            objCitiesService.GetByCityId(CityId, ref dtCity);
            if (dtCity.Rows.Count > 0)
            {
                drpState.SelectedValue = dtCity.Rows[0]["StateId"].ToString();
                txtCityName.Text = dtCity.Rows[0]["Name"].ToString();
                chkActive.Checked = Convert.ToBoolean(dtCity.Rows[0]["DisplayStatus"].ToString());
                //chkPendingInactive.Checked = Convert.ToBoolean(dtCity.Rows[0]["PendingInactive"].ToString());
            }
        }

        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtStates = new DataTable();
            if (!string.IsNullOrEmpty(Request.QueryString["CityId"]))
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    //if (Request.Cookies["LoginCookie"]["UserId"] != null)
                    //{
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;
                        objCitiesService = ServiceFactory.CitiesService;
                        DataTable dtCity = new DataTable();
                        int CityId = 0;

                        if (string.IsNullOrEmpty(Request.QueryString["CityId"]))
                            CityId = 0;
                        else
                            CityId = Convert.ToInt32(Request.QueryString["CityId"].ToString());

                        objCitiesService.GetCityByCityName(Convert.ToInt32(drpState.SelectedValue.ToString()), CityId,txtCityName.Text.Trim(), ref dtCity);
                        if (dtCity.Rows.Count > 0)
                        {
                            dvMessage.InnerHtml = "<strong>City name already exists.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        BizObjects.Cities objCity = new BizObjects.Cities();
                        objCity.StateId = Convert.ToInt32(drpState.SelectedValue.ToString());
                        objCity.Name = txtCityName.Text.Trim();
                        objCity.Status = chkActive.Checked;
                        //objCity.PendingInactive = chkPendingInactive.Checked;
                        objCity.AddedBy = objLoginModel.Id;
                        objCity.AddedByType = objLoginModel.RoleId;
                        objCity.AddedDate = DateTime.UtcNow;

                        if (!string.IsNullOrEmpty(Request.QueryString["CityId"]))
                        {
                            CityId = Convert.ToInt32(Request.QueryString["CityId"].ToString());
                            objCity.Id = CityId;
                            objCity.UpdatedBy = objLoginModel.Id;
                            objCity.UpdatedByType = objLoginModel.RoleId;
                            objCity.UpdatedDate = DateTime.UtcNow;

                            objCitiesService.UpdateCity(ref objCity);
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/City_List.aspx");
                        }
                        else
                        {
                            Session["msg"] = "add";
                            objCitiesService.AddCity(ref objCity);
                            Response.Redirect(Application["SiteAddress"] + "admin/City_List.aspx");
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