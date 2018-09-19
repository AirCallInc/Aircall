using System;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Web.UI.HtmlControls;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class UnitType_List : System.Web.UI.Page
    {
        IUnitsService objUnitsService;

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Unit has been updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Unit has been added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindUnits();
            }
        }

        private void BindUnits()
        {
            DataTable dtUnits = new DataTable();
            int AddedBy = 0;
            string ModelNo = string.Empty;
            string SerialNo = string.Empty;
            string MfgBrand = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["AddedBy"]))
            {
                AddedBy = Convert.ToInt32(Request.QueryString["AddedBy"].ToString());
                drpAddedBy.SelectedValue = AddedBy.ToString();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["MNo"]))
            {
                ModelNo = Request.QueryString["MNo"].ToString();
                txtModel.Text = ModelNo;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SNo"]))
            {
                SerialNo = Request.QueryString["SNo"].ToString();
                txtSerial.Text = SerialNo;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Mfg"]))
            {
                MfgBrand = Request.QueryString["Mfg"].ToString();
                txtMfg.Text = MfgBrand;
            }

            objUnitsService = ServiceFactory.UnitsService;
            objUnitsService.GetAllUnits(false, AddedBy, ModelNo,SerialNo,MfgBrand, ref dtUnits);
            if (dtUnits.Rows.Count > 0)
            {
                lstUnitType.DataSource = dtUnits;
            }
            lstUnitType.DataBind();
        }

        protected void lnkActive_Click(object sender, EventArgs e)
        {
            bool Active = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objUnitsService = ServiceFactory.UnitsService;
            for (int i = 0; i <= lstUnitType.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkPart = (HtmlInputCheckBox)lstUnitType.Items[i].FindControl("chkcheck");
                if (chkPart.Checked)
                {
                    HiddenField hdnUnitId = (HiddenField)lstUnitType.Items[i].FindControl("hdnUnitId");
                    if (!string.IsNullOrEmpty(hdnUnitId.Value))
                    {
                        objUnitsService.SetStatus(true, Convert.ToInt32(hdnUnitId.Value));
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
            BindUnits();
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            bool InActive = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objUnitsService = ServiceFactory.UnitsService;
            for (int i = 0; i <= lstUnitType.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkPart = (HtmlInputCheckBox)lstUnitType.Items[i].FindControl("chkcheck");
                if (chkPart.Checked)
                {
                    HiddenField hdnUnitId = (HiddenField)lstUnitType.Items[i].FindControl("hdnUnitId");
                    if (!string.IsNullOrEmpty(hdnUnitId.Value))
                    {
                        objUnitsService.SetStatus(false, Convert.ToInt32(hdnUnitId.Value));
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
            BindUnits();
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

            BizObjects.Units objUnits = new BizObjects.Units();
            objUnitsService = ServiceFactory.UnitsService;
            for (int i = 0; i <= lstUnitType.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkPart = (HtmlInputCheckBox)lstUnitType.Items[i].FindControl("chkcheck");
                if (chkPart.Checked)
                {
                    HiddenField hdnUnitId = (HiddenField)lstUnitType.Items[i].FindControl("hdnUnitId");
                    if (!string.IsNullOrEmpty(hdnUnitId.Value))
                    {
                        objUnits.Id=Convert.ToInt32(hdnUnitId.Value);
                        objUnits.DeletedBy=UserId;
                        objUnits.DeletedByType=RoleId;
                        objUnits.DeletedDate=DateTime.UtcNow;
                        objUnitsService.DeleteUnit(ref objUnits);
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
            BindUnits();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (drpAddedBy.SelectedValue!="0")
                Param = "?AddedBy=" + drpAddedBy.SelectedValue;
            if (!string.IsNullOrEmpty(txtModel.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&MNo=" + txtModel.Text.Trim();
                else
                    Param = "?MNo=" + txtModel.Text.Trim();
            }
            if (!string.IsNullOrEmpty(txtSerial.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&SNo=" + txtSerial.Text.Trim();
                else
                    Param = "?SNo=" + txtSerial.Text.Trim();
            }
            if (!string.IsNullOrEmpty(txtMfg.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&Mfg=" + txtMfg.Text.Trim();
                else
                    Param = "?Mfg=" + txtMfg.Text.Trim();
            }
            Response.Redirect(Application["SiteAddress"] + "admin/UnitType_List.aspx" + Param);
        }

        protected void dataPagerUnitType_PreRender(object sender, EventArgs e)
        {
            dataPagerUnitType.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindUnits();
        }
    }
}