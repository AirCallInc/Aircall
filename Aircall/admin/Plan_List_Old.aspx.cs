using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Web.UI.HtmlControls;

namespace Aircall.admin
{
    public partial class Plan_List_Old : System.Web.UI.Page
    {
        IPlanService objPlanService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Plan updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Plan added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindPlans();
            }
        }

        private void BindPlans()
        {
            objPlanService = ServiceFactory.PlanService;
            DataTable dtPlans = new DataTable();
            objPlanService.GetAllPlan(ref dtPlans);
            if (dtPlans.Rows.Count > 0)
            {
                lstPlan.DataSource = dtPlans;
            }
            lstPlan.DataBind();
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
            objPlanService = ServiceFactory.PlanService;
            for (int i = 0; i <= lstPlan.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstPlan.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnPlanTypeId = (HiddenField)lstPlan.Items[i].FindControl("hdnPlanTypeId");
                    if (!string.IsNullOrEmpty(hdnPlanTypeId.Value))
                    {
                        objPlanService.SetStatus(true, Convert.ToInt32(hdnPlanTypeId.Value));
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
            BindPlans();
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            bool InActive = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objPlanService = ServiceFactory.PlanService;
            for (int i = 0; i <= lstPlan.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstPlan.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnPlanTypeId = (HiddenField)lstPlan.Items[i].FindControl("hdnPlanTypeId");
                    if (!string.IsNullOrEmpty(hdnPlanTypeId.Value))
                    {
                        objPlanService.SetStatus(false, Convert.ToInt32(hdnPlanTypeId.Value));
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
            BindPlans();
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            bool Delete = false;
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            objPlanService = ServiceFactory.PlanService;
            for (int i = 0; i <= lstPlan.Items.Count - 1; i++)
            {
                HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstPlan.Items[i].FindControl("chkcheck");
                if (chkUsers.Checked)
                {
                    HiddenField hdnPlanTypeId = (HiddenField)lstPlan.Items[i].FindControl("hdnPlanTypeId");
                    if (!string.IsNullOrEmpty(hdnPlanTypeId.Value))
                    {
                        objPlanService.DeletePlan(Convert.ToInt32(hdnPlanTypeId.Value));
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
            BindPlans();
        }
    }
}