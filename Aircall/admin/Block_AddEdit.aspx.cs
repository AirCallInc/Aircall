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
    public partial class Block_AddEdit : System.Web.UI.Page
    {
        IBlocksService objBlocksService = ServiceFactory.BlocksService;
        DataTable dtBlocks = new DataTable();
        BizObjects.Blocks objBlock = new BizObjects.Blocks();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!IsPostBack)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["BlockId"]))
                    {
                        BindBlockById();
                    }
                }
            }
        }

        private void BindBlockById()
        {
            int BlockId = Convert.ToInt32(Request.QueryString["BlockId"].ToString());

            objBlocksService.GetBlocksById(BlockId, ref dtBlocks);
            if (dtBlocks.Rows.Count > 0)
            {
                btnAdd.Text = "Update";

                txtBlockTitle.Text = dtBlocks.Rows[0]["BlockTitle"].ToString();
                CKEditor.Value = dtBlocks.Rows[0]["Description"].ToString();
                 ddlPostion.SelectedValue = dtBlocks.Rows[0]["Position"].ToString();
                //txtOrder.Text = dtBlocks.Rows[0]["Order"].ToString();
                chkActive.Checked = Convert.ToBoolean(dtBlocks.Rows[0]["Status"].ToString());
            }
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    //if (Request.Cookies["LoginCookie"] != null)
                    //{
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        if ((string.IsNullOrEmpty(CKEditor.Value.ToString()) && CKEditor.Value.ToString().Trim() != "<br />" && CKEditor.Value.ToString().Trim() != " "))
                        {
                            dvMessage.InnerHtml = "<strong>Description is required.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                        if (btnAdd.Text == "Add")
                        {
                            objBlock.BlockTitle = txtBlockTitle.Text.ToString().Trim();
                            objBlock.Description = CKEditor.Value.ToString().Trim();
                            objBlock.Position = ddlPostion.SelectedItem.Text.ToString();
                            //objBlock.order = Convert.ToInt32(txtOrder.Text.ToString().Trim());
                            if (chkActive.Checked == true)
                                objBlock.Status = true;
                            else
                                objBlock.Status = false;
                            objBlock.AddedBy = objLoginModel.Id;
                            objBlock.AddedByType = objLoginModel.RoleId;
                            objBlock.AddedDate = DateTime.UtcNow;
                            objBlocksService.AddNewBlocks(ref objBlock);
                            Response.Redirect("Block_List.aspx");
                        }
                        else
                        {
                            BizObjects.CMSPages objCMSPages = new BizObjects.CMSPages();


                            objBlock.Id = Convert.ToInt32(Request.QueryString["BlockId"].ToString());
                            objBlock.BlockTitle = txtBlockTitle.Text.ToString().Trim();
                            objBlock.Description = CKEditor.Value.ToString().Trim();
                            objBlock.Position = ddlPostion.SelectedItem.Text.ToString();
                            //objBlock.order = Convert.ToInt32(txtOrder.Text.ToString().Trim());
                            if (chkActive.Checked == true)
                                objBlock.Status = true;
                            else
                                objBlock.Status = false;
                            objBlock.UpdatedBy = objLoginModel.Id;
                            objBlock.UpdatedByType = objLoginModel.RoleId;
                            objBlock.UpdatedDate = DateTime.UtcNow;
                            objBlocksService.UpdateBlocks(ref objBlock);
                            Response.Redirect("Block_List.aspx");

                            //}
                        }
                    }
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
}