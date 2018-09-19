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
    public partial class sitesettingEdit : System.Web.UI.Page
    {
        ISiteSettingService objSiteSettingService = ServiceFactory.SiteSettingService;

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dtResult = new DataTable();
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    objSiteSettingService.GetSiteSettingById(int.Parse(Request.QueryString["id"]), ref dtResult);

                    if (dtResult.Rows.Count > 0)
                    {
                        txtName.Value = dtResult.Rows[0]["DisplayName"].ToString();
                        if (Request.QueryString["id"].ToString() == "54")
                        {
                            ltrSalesAgreement.Text = dtResult.Rows[0]["Value"].ToString();
                            dvFile.Visible = true;
                            dvNonFile.Visible = false;
                            RequiredFieldValidator1.Enabled = false;
                        }
                        else
                        {
                            txtValue.Value = dtResult.Rows[0]["Value"].ToString();
                            dvFile.Visible = false;
                            dvNonFile.Visible = true;
                            RequiredFieldValidator1.Enabled = true;
                        }
                        hdnValue.Value = dtResult.Rows[0]["Value"].ToString();
                    }
                }
                else
                {
                    Response.Redirect("/admin/sitesetting_list.aspx");
                }
            }
        }

        protected void btnUpate_Click(object sender, EventArgs e)
        {
            dvMessage.InnerHtml = "";
            dvMessage.Visible = false;
            string SiteSettingVal=string.Empty;
            int SiteId = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    SiteId = Convert.ToInt32(Request.QueryString["id"].ToString());
                    if (SiteId.ToString() == "54")
                    {
                        if (fpdSalesAgreement.HasFile)
                        {
                            string[] AllowedFileExtensions = new string[] { ".pdf" };
                            if (!AllowedFileExtensions.Contains(fpdSalesAgreement.FileName.Substring(fpdSalesAgreement.FileName.LastIndexOf('.'))))
                            {
                                dvMessage.InnerHtml = "<strong>Please select file of type: " + string.Join(", ", AllowedFileExtensions) + "</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                return;
                            }
                            string FileName = URL(fpdSalesAgreement.FileName);
                            string filePath = Path.Combine(Server.MapPath("~/uploads/Policies/"), FileName);
                            fpdSalesAgreement.SaveAs(filePath);
                            SiteSettingVal = FileName;
                        }
                        else
                            SiteSettingVal = hdnValue.Value;
                    }
                    else
                        SiteSettingVal = txtValue.Value;

                    if (SiteId == 21 || SiteId == 22)
                    {
                        if (Convert.ToInt32(SiteSettingVal) <= 0)
                        {
                            dvMessage.InnerHtml = "<strong>Value must be greater than 1.</strong>";
                            dvMessage.Attributes.Add("class", "alert alert-error");
                            dvMessage.Visible = true;
                            return;
                        }
                    }

                    objSiteSettingService.UpdateSiteSettingById(SiteId, SiteSettingVal);
                    Session["upd"] = "1";
                    Response.Redirect("/admin/sitesetting_list.aspx");
                }
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>" + Ex.Message.ToString() + "</strong>";
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        public string URL(string url)
        {
            string FileName = url.Trim();
            FileName = FileName.Replace(" ", "-");
            //FileName = FileName.Replace(".", "-");
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
    }
}