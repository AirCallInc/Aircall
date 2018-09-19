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
    public partial class Area_List : System.Web.UI.Page
    {
        IAreasService objAreasService;
        ICitiesService objCitiesService;
        IStateService objStateService;
        protected void Page_Load(object sender, EventArgs e)
        {
            dataPagerArea.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Area updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Area added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindAreasList();
                FillStateDropdown();
            }
        }
        private void FillStateDropdown()
        {
            objStateService = ServiceFactory.StateService;
            DataTable dtStates = new DataTable();
            objStateService.GetAllStates(true, true,ref dtStates);
            if (dtStates.Rows.Count > 0)
            {
                drpState.DataSource = dtStates;
                drpState.DataTextField = dtStates.Columns["Name"].ToString();
                drpState.DataValueField = dtStates.Columns["Id"].ToString();
            }
            drpState.DataBind();
            drpState.Items.Insert(0, new ListItem("Select State", "0"));
            BindCityFromState(0);
        }
        private void BindAreasList()
        {
            objAreasService = ServiceFactory.AreasService;
            DataTable dtAreas = new DataTable();
            objAreasService.SearchByAreaNameStateCityZip(AreaNameFilter, int.Parse(StateFilter), int.Parse(CityFilter), ZipFilter, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtAreas);
            if (dtAreas.Rows.Count > 0)
            {
                lstArea.DataSource = dtAreas;
            }
            lstArea.DataBind();
        }

        private void BindCityFromState(int StateId)
        {
            objCitiesService = ServiceFactory.CitiesService;
            DataTable dtCities = new DataTable();
            objCitiesService.GetAllCityByStateId(StateId, true,ref dtCities);
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

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Scheduled Service for selected areas will be moved to Pending. Are you sure want to delete selected record?','Please select atleast one record')");
        }

        protected void lnkActive_Click(object sender, EventArgs e)
        {
            bool Active = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            objAreasService = ServiceFactory.AreasService;

            int UserId = objLoginModel.Id;
            int RoleId = objLoginModel.RoleId;

            for (int i = 0; i <= lstArea.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstArea.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnAreaId = (HiddenField)lstArea.Items[i].FindControl("hdnAreaId");
                    if (!string.IsNullOrEmpty(hdnAreaId.Value))
                    {
                        objAreasService.SetStatus(true, Convert.ToInt32(hdnAreaId.Value),UserId,RoleId);
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
            BindAreasList();
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            bool InActive = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;

            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            objAreasService = ServiceFactory.AreasService;

            int UserId = objLoginModel.Id;
            int RoleId = objLoginModel.RoleId;

            for (int i = 0; i <= lstArea.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstArea.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnAreaId = (HiddenField)lstArea.Items[i].FindControl("hdnAreaId");
                    if (!string.IsNullOrEmpty(hdnAreaId.Value))
                    {
                        objAreasService.SetStatus(false, Convert.ToInt32(hdnAreaId.Value),UserId,RoleId);
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
            BindAreasList();
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            bool Delete = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            LoginModel objLoginModel = new LoginModel();
            objLoginModel = Session["LoginSession"] as LoginModel;

            objAreasService = ServiceFactory.AreasService;

            int UserId = objLoginModel.Id;
            int RoleId = objLoginModel.RoleId;

            for (int i = 0; i <= lstArea.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstArea.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnAreaId = (HiddenField)lstArea.Items[i].FindControl("hdnAreaId");
                    if (!string.IsNullOrEmpty(hdnAreaId.Value))
                    {
                        objAreasService.DeleteArea(Convert.ToInt32(hdnAreaId.Value),UserId,RoleId);
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
            BindAreasList();
        }

        protected void dataPagerArea_PreRender(object sender, EventArgs e)
        {
            dataPagerArea.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindAreasList();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            StateFilter = drpState.SelectedItem.Value;
            CityFilter = drpCity.SelectedItem.Value;
            AreaNameFilter = txtAreaName.Text;
            ZipFilter = txtZip.Text;
            BindAreasList();
        }

        protected void lstArea_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstArea.FindControl(e.SortExpression) as LinkButton;
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

        protected string StateFilter
        {
            get
            {
                if (ViewState["StateFilter"] == null)
                    ViewState["StateFilter"] = "0";
                return (string)ViewState["StateFilter"];
            }
            set { ViewState["StateFilter"] = value; }
        }

        protected string ZipFilter
        {
            get
            {
                if (ViewState["ZipFilter"] == null)
                    ViewState["ZipFilter"] = "";
                return (string)ViewState["ZipFilter"];
            }
            set { ViewState["ZipFilter"] = value; }
        }

        protected string AreaNameFilter
        {
            get
            {
                if (ViewState["AreaNameFilter"] == null)
                    ViewState["AreaNameFilter"] = "";
                return (string)ViewState["AreaNameFilter"];
            }
            set { ViewState["AreaNameFilter"] = value; }
        }

        protected string CityFilter
        {
            get
            {
                if (ViewState["CityFilter"] == null)
                    ViewState["CityFilter"] = "0";
                return (string)ViewState["CityFilter"];
            }
            set { ViewState["CityFilter"] = value; }
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

        protected void SortByServiceCase_Click(object sender, EventArgs e)
        {

        }

        protected void drpState_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCityFromState(int.Parse(drpState.SelectedItem.Value));
        }
    }
}