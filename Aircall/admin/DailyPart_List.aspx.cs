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
    public partial class DailyPart_List : System.Web.UI.Page
    {
        IDailyPartListMasterService objDailyPartListMasterService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDailyPartList();
            }
        }

        private void BindDailyPartList()
        {
            DataTable dtPartList = new DataTable();
            objDailyPartListMasterService = ServiceFactory.DailyPartListMasterService;
            objDailyPartListMasterService.GetAllDailyPartList(ref dtPartList);
            if (dtPartList.Rows.Count > 0)
            {
                lstDailyPart.DataSource = dtPartList;
            }
            lstDailyPart.DataBind();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        BizObjects.DailyPartListMaster objDailyPartListMaster = new BizObjects.DailyPartListMaster();
                        objDailyPartListMasterService = ServiceFactory.DailyPartListMasterService;
                        for (int i = 0; i <= lstDailyPart.Items.Count - 1; i++)
                        {
                            HiddenField hdnPartId = (HiddenField)lstDailyPart.Items[i].FindControl("hdnPartId");
                            HtmlInputCheckBox chkService = (HtmlInputCheckBox)lstDailyPart.Items[i].FindControl("chkcheckService");
                            HtmlInputCheckBox chkRepair = (HtmlInputCheckBox)lstDailyPart.Items[i].FindControl("chkcheckRepair");
                            objDailyPartListMaster.Id = Convert.ToInt32(hdnPartId.Value);
                            objDailyPartListMaster.IsIncludeInService = chkService.Checked;
                            objDailyPartListMaster.IsIncludeInRepair = chkRepair.Checked;
                            objDailyPartListMaster.UpdatedBy = objLoginModel.Id;
                            objDailyPartListMaster.UpdatedByType = objLoginModel.RoleId;
                            objDailyPartListMaster.UpdatedDate = DateTime.UtcNow;
                            objDailyPartListMasterService.UpdateDailyPartList(ref objDailyPartListMaster);

                            dvMessage.InnerHtml = "<strong>Daily Part List Updated Successfully.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-success");
                            dvMessage.Visible = true;
                        }
                        BindDailyPartList();
                    }
                    else
                        Response.Redirect(Application["SiteAddress"] + "/admin/Login.aspx");
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