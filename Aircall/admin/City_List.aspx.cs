using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class City_List : System.Web.UI.Page
    {
        ICitiesService objCitiesService;
        IStateService objStateService;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>City updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>City added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                FillStateDropdown();
                BindCities();
            }
        }

        private void BindCities()
        {
            string State = "0";
            string City = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["State"]))
            {
                State = Request.QueryString["State"].ToString();
                drpState.SelectedValue = State;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["City"]))
            {
                City = Request.QueryString["City"].ToString();
                txtCity.Text = City;
            }

            objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCities = new DataTable();
            objCitiesService.GetAllCityFilter(int.Parse(State),City, false, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtCities);
            if (dtCities.Rows.Count > 0)
            {
                lstCity.DataSource = dtCities;
            }
            lstCity.DataBind();
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }

        protected void lnkActive_Click(object sender, EventArgs e)
        {
            bool Active = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objCitiesService = ServiceFactory.CitiesService;
            for (int i = 0; i <= lstCity.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstCity.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnCityId = (HiddenField)lstCity.Items[i].FindControl("hdnCityId");
                    if (!string.IsNullOrEmpty(hdnCityId.Value))
                    {
                        objCitiesService.SetStatus(true, Convert.ToInt32(hdnCityId.Value));
                        Active = true;
                    }
                }
            }
            if (Active)
            {
                dvMessage.InnerHtml = "<strong>Record updated successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindCities();
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            bool InActive = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objCitiesService = ServiceFactory.CitiesService;
            for (int i = 0; i <= lstCity.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstCity.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnCityId = (HiddenField)lstCity.Items[i].FindControl("hdnCityId");
                    if (!string.IsNullOrEmpty(hdnCityId.Value))
                    {
                        objCitiesService.SetStatus(false, Convert.ToInt32(hdnCityId.Value));
                        InActive = true;
                    }
                }
            }
            if (InActive)
            {
                dvMessage.InnerHtml = "<strong>Record updated successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindCities();
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            bool Delete = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;
            int UserId = objLoginModel.Id;
            int RoleId = objLoginModel.RoleId;

            BizObjects.Cities objCity = new BizObjects.Cities();
            objCitiesService = ServiceFactory.CitiesService;

            for (int i = 0; i <= lstCity.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstCity.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnCityId = (HiddenField)lstCity.Items[i].FindControl("hdnCityId");
                    if (!string.IsNullOrEmpty(hdnCityId.Value))
                    {
                        objCity.Id = Convert.ToInt32(hdnCityId.Value);
                        objCity.DeletedBy = UserId;
                        objCity.DeletedByType = RoleId;
                        objCity.DeletedDate = DateTime.UtcNow;
                        objCitiesService.DeleteCity(ref objCity);
                        Delete = true;
                    }
                }
            }
            if (Delete)
            {
                dvMessage.InnerHtml = "<strong>Record deleted successfully.</strong>";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            BindCities();
        }

        protected void dataPagerCity_PreRender(object sender, EventArgs e)
        {
            dataPagerCity.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindCities();
        }
        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtStates = new DataTable();
            objStateService.GetAllStates(true, false, ref dtStates);
            if (dtStates.Rows.Count > 0)
            {
                drpState.DataSource = dtStates;
                drpState.DataTextField = dtStates.Columns["Name"].ToString();
                drpState.DataValueField = dtStates.Columns["Id"].ToString();
            }
            drpState.DataBind();
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;
            if (drpState.SelectedItem.Value != "0")
            {
                Param = "?State=" + drpState.SelectedItem.Value;
            }
            if (!string.IsNullOrEmpty(txtCity.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&City=" + txtCity.Text.Trim();
                else
                    Param = "?City=" + txtCity.Text.Trim();
            }
            Response.Redirect(Application["SiteAddress"] + "admin/City_List.aspx" + Param);
        }
        protected SortDirection ListViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;
                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected string ListViewSortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                    ViewState["SortExpression"] = "";
                return (string)ViewState["SortExpression"];
            }
            set { ViewState["SortExpression"] = value; }
        }

        protected void lstCity_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstCity.FindControl(e.SortExpression) as LinkButton;
            HtmlTableCell th = lb.Parent as HtmlTableCell;
            HtmlTableRow tr = th.Parent as HtmlTableRow;
            List<HtmlTableCell> ths = new List<HtmlTableCell>();
            foreach (HtmlTableCell item in tr.Controls)
            {
                try
                {
                    if (item.ID.Contains("th"))
                    {
                        item.Attributes["class"] = "sorting";
                    }
                }
                catch (Exception ex)
                {
                }
            }

            ListViewSortExpression = e.SortExpression;
            if (ListViewSortDirection == SortDirection.Ascending)
            {
                ListViewSortDirection = SortDirection.Descending;
                th.Attributes["class"] = "sorting_desc";
            }
            else
            {
                ListViewSortDirection = SortDirection.Ascending;
                th.Attributes["class"] = "sorting_asc";
            }
        }

        protected void SortByServiceCase_Click(object sender, EventArgs e)
        {

        }
    }
}