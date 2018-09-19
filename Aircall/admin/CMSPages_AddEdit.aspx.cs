using Aircall.Common;
using ImageResizer;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Aircall.admin
{
    public partial class CMSPages_AddEdit : System.Web.UI.Page
    {
        ICMSPagesService objAddNewCMSPage = ServiceFactory.CMSPagesService;
        DataTable dtCMSPage = new DataTable();
        DataTable dtParentPage = new DataTable();
        DataTable dtBlockList = new DataTable();
        DataTable dtBlockById = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //BindParentDetails();
                BindListofBlock();
                if (!string.IsNullOrEmpty(Request.QueryString["CMSPageId"]))
                {
                    BindCMSPageById();
                }

            }

        }

        public void BindListofBlock()
        {
            objAddNewCMSPage.GetAllBlockList(ref dtBlockList);
            if (dtBlockList.Rows.Count > 0)
            {
                cblBlocks.DataSource = dtBlockList;
                cblBlocks.DataTextField = "BlockTitle";
                cblBlocks.DataValueField = "Id";
                cblBlocks.DataBind();
            }
        }

        //public void BindParentDetails()
        //{
        //    objAddNewCMSPage.GetParentList(ref dtParentPage);
        //    if (dtParentPage.Rows.Count > 0)
        //    {
        //        ddlParent.DataSource = dtParentPage;
        //        ddlParent.DataTextField = dtParentPage.Columns["MenuTitle"].ToString();
        //        ddlParent.DataValueField = dtParentPage.Columns["Id"].ToString();
        //    }
        //    ddlParent.DataBind();
        //    ddlParent.Items.Insert(0, new ListItem("Select Parent Page", "0"));
        //}
        private void BindCMSPageById()
        {
            int CMSPageId = Convert.ToInt32(Request.QueryString["CMSPageId"].ToString());


            objAddNewCMSPage.GetCMSPageById(CMSPageId, ref dtCMSPage);
            if (dtCMSPage.Rows.Count > 0)
            {
                btnAdd.Text = "Save";
                // parent list

                //objAddNewCMSPage.GetParentList(ref dtParentPage);
                //if (dtParentPage.Rows.Count > 0)
                //{
                //    ddlParent.DataSource = dtParentPage;
                //    ddlParent.DataTextField = dtParentPage.Columns["MenuTitle"].ToString();
                //    ddlParent.DataValueField = dtParentPage.Columns["Id"].ToString();
                //}
                //ddlParent.DataBind();
                //ddlParent.Items.Insert(0, new ListItem("Select Parent Page", "0"));

                // Block list


                objAddNewCMSPage.GetBlockCMSId(CMSPageId, ref dtBlockById);

                for (int j = 0; j < dtBlockById.Rows.Count; j++)
                {
                    for (int i = 0; i < cblBlocks.Items.Count; i++)
                    {
                        if ((string)cblBlocks.Items[i].Value.ToString() == dtBlockById.Rows[j]["Id"].ToString())
                        {
                            cblBlocks.Items[i].Selected = true;
                        }
                    }
                }

                txtPageTitle.Text = dtCMSPage.Rows[0]["PageTitle"].ToString();
                txtMenuTitle.Text = dtCMSPage.Rows[0]["MenuTitle"].ToString();
                txtURL.Text = dtCMSPage.Rows[0]["URL"].ToString();
                CKEditor.Value = dtCMSPage.Rows[0]["Description"].ToString();
                if (!string.IsNullOrEmpty(dtCMSPage.Rows[0]["BannerImage"].ToString()))
                {
                    lnkBanner.HRef = Application["SiteAddress"] + "/uploads/BannerImg/" + dtCMSPage.Rows[0]["BannerImage"].ToString();
                    lnkBanner.Visible = true;
                }
                hdnBanner.Value = dtCMSPage.Rows[0]["BannerImage"].ToString();
                txtMTitle.Text = dtCMSPage.Rows[0]["MetaTitle"].ToString();
                txtMKeywords.Text = dtCMSPage.Rows[0]["MetaKeywords"].ToString();
                txtMDes.Text = dtCMSPage.Rows[0]["MetaDescription"].ToString();
                txtAMeta.Text = dtCMSPage.Rows[0]["AdditionalMeta"].ToString();
                chkActive.Checked = Convert.ToBoolean(dtCMSPage.Rows[0]["Status"].ToString());
            }
        }
        public string URL(string url)
        {
            string FileName = url.Trim();
            FileName = FileName.Replace(" ", "-");
            FileName = FileName.Replace(".", "-");
            FileName = FileName.Replace(",", "-");
            FileName = FileName.Replace("_", "-");
            FileName = FileName.Replace("'", "-");
            FileName = FileName.Replace("\"", "-");
            FileName = FileName.Replace("!", "-");
            FileName = FileName.Replace("@", "-");
            FileName = FileName.Replace("#", "-");
            FileName = FileName.Replace("$", "-");
            FileName = FileName.Replace("%", "-");
            FileName = FileName.Replace("^", "-");
            FileName = FileName.Replace("&", "-");
            FileName = FileName.Replace("*", "-");
            FileName = FileName.Replace("(", "-");
            FileName = FileName.Replace(")", "-");
            FileName = FileName.Replace("|", "-");
            FileName = FileName.Replace("\\", "-");
            FileName = FileName.Replace("?", "-");
            FileName = FileName.Replace("{", "-");
            FileName = FileName.Replace("}", "-");
            FileName = FileName.Replace("[", "-");
            FileName = FileName.Replace("]", "-");
            FileName = FileName.Replace("~", "-");
            FileName = FileName.Replace("`", "-");
            FileName = FileName.Replace(";", "-");
            FileName = FileName.Replace(":", "-");
            FileName = FileName.Replace("+", "-");
            FileName = FileName.Replace("=", "-");
            FileName = FileName.Replace("--", "-");
            FileName = FileName.Replace("----", "-");
            if (FileName.EndsWith("-"))
            {
                FileName = FileName.Remove(FileName.Length - 1, 1);
            }
            return FileName;
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    if (Session["LoginSession"] != null)
                    {
                        LoginModel objLoginModel = new LoginModel();
                        objLoginModel = Session["LoginSession"] as LoginModel;

                        if (btnAdd.Text == "Add")
                        {
                            BizObjects.CMSPages objCMSPages = new BizObjects.CMSPages();

                          
                            objCMSPages.PageTitle = txtPageTitle.Text.ToString().Trim();
                            objCMSPages.MenuTitle = txtMenuTitle.Text.ToString().Trim();
                            objCMSPages.URL = URL(txtURL.Text.ToString().Trim());
                            objCMSPages.Description = CKEditor.Value.ToString().Trim();
                            if (fpBanner.HasFile)
                            {
                                //string filename = DateTime.Now.Ticks.ToString() + Path.GetExtension(fpBanner.FileName);
                                //var uploadpath = Server.MapPath("~/uploads/ReportServiceImg/") + filename;
                                //fpBanner.SaveAs(uploadpath);
                                //objCMSPages.BannerImage = filename;

                                string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                                if (!AllowedFileExtensions.Contains(fpBanner.FileName.Substring(fpBanner.FileName.LastIndexOf('.'))))
                                {
                                    dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                                else
                                {
                                    string filenameOriginal = DateTime.UtcNow.Ticks.ToString().Trim();
                                    string fileName = fpBanner.FileName;
                                    string extension = System.IO.Path.GetExtension(fileName);
                                    string extensionwithoutdot = extension.Remove(0, 1);

                                    Instructions rsiphnWxH = new Instructions();
                                    rsiphnWxH.Width = 1500;
                                    rsiphnWxH.Height = 330;
                                    rsiphnWxH.Mode = FitMode.Stretch;
                                    rsiphnWxH.Format = extensionwithoutdot;

                                    ImageJob imjob = new ImageJob(fpBanner.PostedFile.InputStream, Server.MapPath("~/uploads/BannerImg/" + filenameOriginal + extension), rsiphnWxH);
                                    imjob.CreateParentDirectory = true;
                                    imjob.Build();
                                    objCMSPages.BannerImage = filenameOriginal + extension;
                                    //objCMSPages.BannerImage = "";
                                }
                            }
                            else
                            {
                                objCMSPages.BannerImage = hdnBanner.Value;
                            }


                            objCMSPages.MetaTitle = txtMTitle.Text.ToString().Trim();
                            objCMSPages.MetaKeywords = txtMKeywords.Text.ToString().Trim();
                            objCMSPages.MetaDescription = txtMDes.Text.ToString().Trim();
                            objCMSPages.AdditionalMeta = txtAMeta.Text.ToString().Trim();
                            if (chkActive.Checked == true)
                            {
                                objCMSPages.Status = true;
                            }
                            else
                            {
                                objCMSPages.Status = false;
                            }
                            objCMSPages.AddedBy = objLoginModel.Id;
                            objCMSPages.AddedByType = objLoginModel.RoleId;
                            objCMSPages.AddedDate = DateTime.UtcNow;
                            int CMSPageId = objAddNewCMSPage.AddNewCMSPage(ref objCMSPages);
                            if (CMSPageId > 0)
                            {
                                for (int i = 0; i < cblBlocks.Items.Count; i++)
                                {
                                    if (cblBlocks.Items[i].Selected)
                                    {
                                        int BlockId = Convert.ToInt32(cblBlocks.Items[i].Value);
                                        objAddNewCMSPage.AddBlockListById(CMSPageId, BlockId);
                                    }

                                }
                            }
                            Response.Redirect("CMSPages_List.aspx");
                        }
                        else
                        {
                            BizObjects.CMSPages objCMSPages = new BizObjects.CMSPages();
                            objCMSPages.Id = Convert.ToInt32(Request.QueryString["CMSPageId"].ToString());
                            int CMSPageId = Convert.ToInt32(Request.QueryString["CMSPageId"].ToString());

                            objAddNewCMSPage.GetBlockCMSId(CMSPageId, ref dtBlockById);

                            // block List Update....
                            objAddNewCMSPage.DeletePageBlock(CMSPageId);
                            for (int i = 0; i < cblBlocks.Items.Count; i++)
                            {
                                if (cblBlocks.Items[i].Selected)
                                {
                                    int BlockId = Convert.ToInt32(cblBlocks.Items[i].Value);

                                    objAddNewCMSPage.UpdateBlockListById(CMSPageId,BlockId);
                                }
                               
                            }                       

                            objCMSPages.PageTitle = txtPageTitle.Text.ToString().Trim();
                            objCMSPages.MenuTitle = txtMenuTitle.Text.ToString().Trim();
                            objCMSPages.URL = URL(txtURL.Text.ToString().Trim());
                            objCMSPages.Description = CKEditor.Value.ToString().Trim();
                            if (fpBanner.HasFile)
                            {
                                //string filename = DateTime.Now.Ticks.ToString() + Path.GetExtension(fpBanner.FileName);
                                //var uploadpath = Server.MapPath("~/uploads/ReportServiceImg/") + filename;
                                //fpBanner.SaveAs(uploadpath);
                                //objCMSPages.BannerImage = filename;

                                string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };
                                if (!AllowedFileExtensions.Contains(fpBanner.FileName.Substring(fpBanner.FileName.LastIndexOf('.'))))
                                {
                                    dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                    dvMessage.Attributes.Add("class", "alert alert-error");
                                    dvMessage.Visible = true;
                                    return;
                                }
                                else
                                {
                                    string filenameOriginal = DateTime.UtcNow.Ticks.ToString().Trim();
                                    string fileName = fpBanner.FileName;
                                    string extension = System.IO.Path.GetExtension(fileName);
                                    string extensionwithoutdot = extension.Remove(0, 1);

                                    Instructions rsiphnWxH = new Instructions();
                                    rsiphnWxH.Width = 1500;
                                    rsiphnWxH.Height = 330;
                                    rsiphnWxH.Mode = FitMode.Stretch;
                                    rsiphnWxH.Format = extensionwithoutdot;

                                    ImageJob imjob = new ImageJob(fpBanner.PostedFile.InputStream, Server.MapPath("~/uploads/BannerImg/" + filenameOriginal + extension), rsiphnWxH);
                                    imjob.CreateParentDirectory = true;
                                    imjob.Build();
                                    objCMSPages.BannerImage = filenameOriginal + extension;
                                    //objCMSPages.BannerImage = "";
                                }
                            }
                            else
                            {
                                objCMSPages.BannerImage = hdnBanner.Value;
                            }
                            objCMSPages.MetaTitle = txtMTitle.Text.ToString().Trim();
                            objCMSPages.MetaKeywords = txtMKeywords.Text.ToString().Trim();
                            objCMSPages.MetaDescription = txtMDes.Text.ToString().Trim();
                            objCMSPages.AdditionalMeta = txtAMeta.Text.ToString().Trim();
                            if (chkActive.Checked == true)
                            {
                                objCMSPages.Status = true;
                            }
                            else
                            {
                                objCMSPages.Status = false;
                            }
                            objCMSPages.UpdatedBy = objLoginModel.Id;
                            objCMSPages.UpdatedByType = objLoginModel.RoleId;
                            objCMSPages.UpdatedDate = DateTime.UtcNow;
                            objAddNewCMSPage.UpdateCMSPage(ref objCMSPages);
                            Response.Redirect("CMSPages_List.aspx");
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