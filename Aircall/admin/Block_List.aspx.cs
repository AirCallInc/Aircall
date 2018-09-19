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
    public partial class Block_List : System.Web.UI.Page
    {
        IBlocksService objBlocksService = ServiceFactory.BlocksService;
        DataTable dtBlocks = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindListOfBlocks();
            }
        }

        private void BindListOfBlocks()
        {
            objBlocksService.GetAllBlocks(ref dtBlocks);
            if (dtBlocks.Rows.Count > 0)
            {
                lstBlocks.DataSource = dtBlocks;
            }
            lstBlocks.DataBind();
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Are you sure want to delete selected record?','Please select atleast one record')");
        }

        protected void lnkActive_Click(object sender, EventArgs e)
        {
            try
            {
                bool Active = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;

                for (int i = 0; i <= lstBlocks.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkBlock = (HtmlInputCheckBox)lstBlocks.Items[i].FindControl("chkcheck");
                    if (chkBlock.Checked)
                    {
                        HiddenField hdnBlockId = (HiddenField)lstBlocks.Items[i].FindControl("hdnBlockId");
                        if (!string.IsNullOrEmpty(hdnBlockId.Value))
                        {
                            objBlocksService.SetStatus(true, Convert.ToInt32(hdnBlockId.Value));
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
                BindListOfBlocks();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            try
            {
                bool InActive = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;

                for (int i = 0; i <= lstBlocks.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkBlock = (HtmlInputCheckBox)lstBlocks.Items[i].FindControl("chkcheck");

                    if (chkBlock.Checked)
                    {
                        HiddenField hdnBlockId = (HiddenField)lstBlocks.Items[i].FindControl("hdnBlockId");
                        if (!string.IsNullOrEmpty(hdnBlockId.Value))
                        {
                            objBlocksService.SetStatus(false, Convert.ToInt32(hdnBlockId.Value));
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
                BindListOfBlocks();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool Deleted = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;

                for (int i = 0; i <= lstBlocks.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkBlock = (HtmlInputCheckBox)lstBlocks.Items[i].FindControl("chkcheck");
                    if (chkBlock.Checked)
                    {
                        HiddenField hdnBlockId = (HiddenField)lstBlocks.Items[i].FindControl("hdnBlockId");
                        if (!string.IsNullOrEmpty(hdnBlockId.Value))
                        {
                            objBlocksService.DeleteBlock(Convert.ToInt32(hdnBlockId.Value));
                            Deleted = true;
                        }
                    }
                }
                if (Deleted)
                {
                    dvMessage.InnerHtml = "<strong>Record deleted successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                BindListOfBlocks();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + Ex.Message.Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }
    }
}