using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Web.UI.HtmlControls;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class Part_List : System.Web.UI.Page
    {
        IDailyPartListMasterService objDailyPartListMasterService;
        IPartsService objPartsService;


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
                FillInventoryDropdown();
                FillPartTypeDropdown();
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Part updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Part added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindParts();
            }
        }
        private void FillInventoryDropdown()
        {
            var values = DurationExtensions.GetValues<General.InventoryType>();
            List<string> data = new List<string>();
            foreach (var item in values)
            {
                General.InventoryType p = (General.InventoryType)item;
                data.Add(p.GetEnumDescription());
            }
            drpInventory.DataSource = data;
            drpInventory.DataBind();
        }
        private void FillPartTypeDropdown()
        {
            DataTable dtParts = new DataTable();
            objDailyPartListMasterService = ServiceFactory.DailyPartListMasterService;
            objDailyPartListMasterService.GetAllDailyPartList(ref dtParts);
            if (dtParts.Rows.Count > 0)
            {
                drpPartType.DataSource = dtParts;
                drpPartType.DataTextField = dtParts.Columns["Name"].ToString();
                drpPartType.DataValueField = dtParts.Columns["Id"].ToString();
            }
            drpPartType.DataBind();
            drpPartType.Items.Insert(0, new ListItem("Select Part Type", "0"));
        }
        private void BindParts()
        {
            DataTable dtParts = new DataTable();
            objPartsService = ServiceFactory.PartsService;

            string PartType = "0";
            string Partname = string.Empty;
            string Inventory = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["Name"]))
            {
                Partname = Request.QueryString["Name"].ToString();
                txtPartname.Text = Partname;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["PartType"]))
            {
                PartType = Request.QueryString["PartType"].ToString();
                drpPartType.SelectedValue = PartType;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Inventory"]))
            {
                Inventory = Request.QueryString["Inventory"].ToString();
                drpInventory.SelectedValue = Inventory;
            }

            objPartsService.GetAllPartsByFilter(int.Parse(PartType),Partname,Inventory ,ref dtParts);
            if (dtParts.Rows.Count > 0)
            {
                lstParts.DataSource = dtParts;
            }
            lstParts.DataBind();
        }

        protected void lnkActive_Click(object sender, EventArgs e)
        {
            bool Active = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objPartsService = ServiceFactory.PartsService;
            for (int i = 0; i <= lstParts.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkPart = (HtmlInputCheckBox)lstParts.Items[i].FindControl("chkcheck");
                if (chkPart.Checked)
                {
                    HiddenField hdnPartId = (HiddenField)lstParts.Items[i].FindControl("hdnPartId");
                    if (!string.IsNullOrEmpty(hdnPartId.Value))
                    {
                        objPartsService.SetStatus(true, Convert.ToInt32(hdnPartId.Value));
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
            BindParts();
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            bool InActive = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objPartsService = ServiceFactory.PartsService;
            for (int i = 0; i <= lstParts.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkPart = (HtmlInputCheckBox)lstParts.Items[i].FindControl("chkcheck");
                if (chkPart.Checked)
                {
                    HiddenField hdnPartId = (HiddenField)lstParts.Items[i].FindControl("hdnPartId");
                    if (!string.IsNullOrEmpty(hdnPartId.Value))
                    {
                        objPartsService.SetStatus(false, Convert.ToInt32(hdnPartId.Value));
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
            BindParts();
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

            BizObjects.Parts objParts = new BizObjects.Parts();
            objPartsService = ServiceFactory.PartsService;
            for (int i = 0; i <= lstParts.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkPart = (HtmlInputCheckBox)lstParts.Items[i].FindControl("chkcheck");
                if (chkPart.Checked)
                {
                    HiddenField hdnPartId = (HiddenField)lstParts.Items[i].FindControl("hdnPartId");
                    if (!string.IsNullOrEmpty(hdnPartId.Value))
                    {
                        objParts.Id = Convert.ToInt32(hdnPartId.Value);
                        objParts.DeletedBy = UserId;
                        objParts.DeletedByType = RoleId;
                        objParts.DeletedDate = DateTime.UtcNow;
                        objPartsService.DeletePart(ref objParts);
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
            BindParts();
        }

        protected void dataPagerParts_PreRender(object sender, EventArgs e)
        {
            dataPagerParts.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindParts();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;
            if (drpPartType.SelectedItem.Value != "0")
            {
                Param = "?PartType=" + drpPartType.SelectedItem.Value;
            }
            if (!string.IsNullOrEmpty(txtPartname.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&Name=" + txtPartname.Text.Trim();
                else
                    Param = "?Name=" + txtPartname.Text.Trim();
            }
                if (!string.IsNullOrEmpty(Param))
                    Param = Param + "&Inventory=" + drpInventory.SelectedValue;
                else
                    Param = "?Inventory=" + drpInventory.SelectedValue;
            Response.Redirect(Application["SiteAddress"] + "admin/Part_List.aspx" + Param);
        }
    }
}