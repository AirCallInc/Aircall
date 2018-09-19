using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;

namespace Aircall.admin
{
    public partial class Part_AddEdit : System.Web.UI.Page
    {
        IDailyPartListMasterService objDailyPartListMasterService;
        IPartsService objPartsService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillPartTypeDropdown();
                if (!string.IsNullOrEmpty(Request.QueryString["PartId"]))
                {
                    BindPartByPartId();
                }

            }
        }

        private void BindPartByPartId()
        {
            btnSave.Text = "Update";

            int PartId = Convert.ToInt32(Request.QueryString["PartId"].ToString());
            DataTable dtPart = new DataTable();
            objPartsService = ServiceFactory.PartsService;
            objPartsService.GetPartById(PartId, ref dtPart);
            if (dtPart.Rows.Count > 0)
            {
                if (dtPart.Rows[0]["InventoryType"].ToString().ToLower() == "inventory")
                    rblInventory.Checked = true;
                else
                    rblNonInventory.Checked = true;
                drpPartType.SelectedValue = dtPart.Rows[0]["DailyPartListMasterId"].ToString();
                txtPartname.Text = dtPart.Rows[0]["Name"].ToString();
                txtSize.Text = dtPart.Rows[0]["Size"].ToString();
                txtDescription.Text = dtPart.Rows[0]["Description"].ToString();
                //txtInbound.Text = dtPart.Rows[0]["InboundQuantity"].ToString();
                //Set 0 as per Michael call on 25Jan2017
                txtReceive.Text = "0";//dtPart.Rows[0]["ReceivedQuantity"].ToString();
                hdnReceice.Value = dtPart.Rows[0]["ReceivedQuantity"].ToString();
                txtAcquired.Text = dtPart.Rows[0]["TotalAcquiredQuantity"].ToString();
                txtInStock.Text = dtPart.Rows[0]["InStockQuantity"].ToString();
                txtReserved.Text = dtPart.Rows[0]["ReservedQuantity"].ToString();
                txtPurchased.Text = dtPart.Rows[0]["PurchasedPrice"].ToString();
                txtSelling.Text = dtPart.Rows[0]["SellingPrice"].ToString();
                txtMinReorder.Text = dtPart.Rows[0]["MinReorderQuantity"].ToString();
                txtReOrder.Text = dtPart.Rows[0]["ReorderQuantity"].ToString();
                chkActive.Checked = Convert.ToBoolean(dtPart.Rows[0]["Status"].ToString());
                chkIsDefault.Checked = Convert.ToBoolean(dtPart.Rows[0]["IsDefault"].ToString());
                txtAcquired.Enabled = false;
                txtInStock.Enabled = false;
                txtReserved.Enabled = false;

            }
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        BizObjects.Parts objParts = new BizObjects.Parts();
                        objPartsService = ServiceFactory.PartsService;
                        int PartId = 0;

                        int InstockQuantity = (string.IsNullOrEmpty(txtInStock.Text.Trim()) ? 0 : Convert.ToInt32(txtInStock.Text.Trim()));
                        int ReorderQuantity = (string.IsNullOrEmpty(txtReOrder.Text.Trim()) ? 0 : Convert.ToInt32(txtReOrder.Text.Trim()));
                        if (ReorderQuantity>InstockQuantity)
                        {
                            dvMessage.InnerHtml = "<strong>In-Stock Quantity must be greater than Re-Order Quantity.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        //if part type is Refrigerant then Part size always required
                        if (drpPartType.SelectedValue=="4" && string.IsNullOrEmpty(txtSize.Text.Trim()))
                        {
                            dvMessage.InnerHtml = "<strong>Part Size is required for Refrigerant Type in lbs.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        if (rblInventory.Checked)
                            objParts.InventoryType = General.InventoryType.Inventory.GetEnumDescription();
                        else
                            objParts.InventoryType = General.InventoryType.NonInventory.GetEnumDescription();

                        if (!string.IsNullOrEmpty(Request.QueryString["PartId"]))
                            PartId = Convert.ToInt32(Request.QueryString["PartId"].ToString());
                        else
                            PartId = 0;

                        DataTable dtParts = new DataTable();
                        objPartsService.CheckPart(PartId, txtPartname.Text.Trim(), ref dtParts);
                        if (dtParts.Rows.Count > 0)
                        {
                            dvMessage.InnerHtml = "<strong>Part already Exists.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }

                        if (drpPartType.SelectedValue != "0")
                            objParts.DailyPartListMasterId = Convert.ToInt32(drpPartType.SelectedValue.ToString());
                        objParts.Name = txtPartname.Text.Trim();
                        objParts.Size = txtSize.Text.Trim();
                        objParts.Description = txtDescription.Text.Trim();

                        //if (!string.IsNullOrEmpty(txtInbound.Text.Trim()))
                        //Commented on 25Jan2017 
                        objParts.InboundQuantity = 0;//Convert.ToInt32(txtInbound.Text.Trim()); 
                        if (!string.IsNullOrEmpty(txtReceive.Text.Trim()))
                            objParts.ReceivedQuantity = Convert.ToInt32(txtReceive.Text.Trim());

                        //if (Convert.ToInt32(hdnReceice.Value) != Convert.ToInt32(txtReceive.Text.Trim()))
                        if (Convert.ToInt32(txtReceive.Text.Trim())!=0)
                        {
                            objParts.TotalAcquiredQuantity = string.IsNullOrEmpty(txtAcquired.Text.Trim()) ?0 : Convert.ToInt32(txtAcquired.Text.Trim()) + Convert.ToInt32(txtReceive.Text.Trim());
                            objParts.InStockQuantity = (string.IsNullOrEmpty(txtInStock.Text.Trim())?0:Convert.ToInt32(txtInStock.Text.Trim())) + (string.IsNullOrEmpty(txtReceive.Text.Trim())?0:Convert.ToInt32(txtReceive.Text.Trim()));
                        }
                        else
                        {
                            objParts.TotalAcquiredQuantity = string.IsNullOrEmpty(txtAcquired.Text.Trim()) ? 0 : Convert.ToInt32(txtAcquired.Text.Trim());
                            objParts.InStockQuantity = string.IsNullOrEmpty(txtInStock.Text.Trim()) ? 0 : Convert.ToInt32(txtInStock.Text.Trim());
                        }
                        if (!string.IsNullOrEmpty(txtReserved.Text.Trim()))
                            objParts.ReservedQuantity = string.IsNullOrEmpty(txtReserved.Text.Trim()) ? 0: Convert.ToInt32(txtReserved.Text.Trim());
                        objParts.PurchasedPrice = Convert.ToDecimal(txtPurchased.Text.Trim());
                        objParts.SellingPrice = Convert.ToDecimal(txtSelling.Text.Trim());
                        objParts.MinReorderQuantity = Convert.ToInt32(txtMinReorder.Text.Trim());
                        objParts.ReorderQuantity = Convert.ToInt32(txtReOrder.Text.Trim());
                        objParts.Status = chkActive.Checked;
                        objParts.IsDefault = chkIsDefault.Checked;
                        objParts.AddedBy = objLoginModel.Id;
                        objParts.AddedByType = objLoginModel.RoleId;
                        objParts.AddedDate = DateTime.UtcNow;

                        if (!string.IsNullOrEmpty(Request.QueryString["PartId"]))
                        {
                            PartId = Convert.ToInt32(Request.QueryString["PartId"].ToString());
                            objParts.Id = PartId;
                            objParts.UpdatedBy = objLoginModel.Id;
                            objParts.UpdatedByType = objLoginModel.RoleId;
                            objParts.UpdatedDate = DateTime.UtcNow;

                            objPartsService.UpdatePart(ref objParts);
                            Session["msg"] = "edit";
                            Response.Redirect(Application["SiteAddress"] + "admin/Part_List.aspx");
                        }
                        else
                        {
                            Session["msg"] = "add";
                            objPartsService.AddPart(ref objParts);
                            Response.Redirect(Application["SiteAddress"] + "admin/Part_List.aspx");
                        }
                    }
                    else
                        Response.Redirect(Application["SiteAddress"] + "admin/Login.aspx");
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