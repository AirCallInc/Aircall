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
    public partial class ZipCode_List : System.Web.UI.Page
    {
        IZipCodeService objZipCodeService;
        ICitiesService objCitiesService;
        IStateService objStateService;

        protected void Page_Load(object sender, EventArgs e)
        {
            dataPagerZip.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Zip Code updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Zip Code added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                FillStateDropdown();
                BindZipCodes();
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }

        private void BindZipCodes()
        {
            objZipCodeService = ServiceFactory.ZipCodeService;
            DataTable dtZipCodes = new DataTable();
            objZipCodeService.GetAllZipCodeByStateIdCityId(int.Parse(StateFilter), int.Parse(CityFilter), ZipCode, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtZipCodes);
            if (dtZipCodes.Rows.Count > 0)
            {
                lstZipCode.DataSource = dtZipCodes;
            }
            lstZipCode.DataBind();
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
        protected void lnkActive_Click(object sender, EventArgs e)
        {
            bool Active = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objZipCodeService = ServiceFactory.ZipCodeService;
            for (int i = 0; i <= lstZipCode.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstZipCode.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnZipId = (HiddenField)lstZipCode.Items[i].FindControl("hdnZipId");
                    if (!string.IsNullOrEmpty(hdnZipId.Value))
                    {
                        objZipCodeService.SetStatus(true, Convert.ToInt32(hdnZipId.Value));
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
            BindZipCodes();
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            bool InActive = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objZipCodeService = ServiceFactory.ZipCodeService;
            for (int i = 0; i <= lstZipCode.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstZipCode.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnZipId = (HiddenField)lstZipCode.Items[i].FindControl("hdnZipId");
                    if (!string.IsNullOrEmpty(hdnZipId.Value))
                    {
                        objZipCodeService.SetStatus(false, Convert.ToInt32(hdnZipId.Value));
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
            BindZipCodes();
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            bool Delete = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objZipCodeService = ServiceFactory.ZipCodeService;
            for (int i = 0; i <= lstZipCode.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstZipCode.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnZipId = (HiddenField)lstZipCode.Items[i].FindControl("hdnZipId");
                    if (!string.IsNullOrEmpty(hdnZipId.Value))
                    {
                        objZipCodeService.DeleteZipCode(Convert.ToInt32(hdnZipId.Value));
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
            BindZipCodes();
        }

        protected void dataPagerZip_PreRender(object sender, EventArgs e)
        {
            dataPagerZip.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindZipCodes();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            StateFilter = drpState.SelectedItem.Value;
            CityFilter = drpCity.SelectedItem.Value;
            ZipCode = txtZip.Text.Trim();
            BindZipCodes();
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

        protected string ZipCode
        {
            get
            {
                if (ViewState["ZipCode"] == null)
                    ViewState["ZipCode"] = "";
                return (string)ViewState["ZipCode"];
            }
            set { ViewState["ZipCode"] = value; }
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

        protected void lstZipCode_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstZipCode.FindControl(e.SortExpression) as LinkButton;
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

        protected void drpState_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCityFromState(int.Parse(drpState.SelectedItem.Value));
        }
    }
}