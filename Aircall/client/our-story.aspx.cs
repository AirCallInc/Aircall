using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class our_story : System.Web.UI.Page
    {
        IBlocksService objBlocksService = ServiceFactory.BlocksService;
        ICMSPagesService objCMSService = ServiceFactory.CMSPagesService;
        DataTable dtCMSService = new DataTable();
        DataTable dtCMSSectionService = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string url = "our-story.aspx";
                string str = "";
                if (url != null)
                {
                    url = url.Replace("/", "");
                    str = url.Split('.')[0].ToString();

                }
                else
                {
                    str = "404";
                }
                InnerHTML(str);
            }
        }

        private void InnerHTML(string url)
        {
            string iBanner = String.Empty;

            string iMBanner = String.Empty;
            string iFooter = String.Empty;
            string iContent = String.Empty;
            DataTable dtSection = new DataTable();
            DataTable dtMSection = new DataTable();
            DataTable dtFSection = new DataTable();
            objCMSService.GetPageContent(url, ref dtCMSService);

            if (dtCMSService.Rows.Count > 0)
            {
                foreach (DataRow item in dtCMSService.Rows)
                {
                    iContent = iContent + item["Description"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(dtCMSService.Rows[0]["BannerImage"].ToString()))
                {
                    BanngerImg.Attributes.Add("style", "background-image:url('" + Application["SiteAddress"] + "uploads/BannerImg/" + dtCMSService.Rows[0]["BannerImage"].ToString() + "')");
                }
                else
                {
                    BanngerImg.Visible = false;
                }

                ltBannerText.Text = dtCMSService.Rows[0]["PageTitle"].ToString();
            }

            if (dtCMSService.Rows.Count > 0)
            {
                Page.Title = dtCMSService.Rows[0]["MetaTitle"].ToString();
                Page.MetaDescription = dtCMSService.Rows[0]["MetaDescription"].ToString();
                Page.MetaKeywords = dtCMSService.Rows[0]["MetaKeywords"].ToString();

                objBlocksService.GetBlockByCMSId(Convert.ToInt32(dtCMSService.Rows[0]["Id"].ToString()), ref dtSection);
                if (dtSection.Rows.Count > 0)
                {
                    string filter = "Position='Top'";
                    DataView dv = new DataView(dtSection, filter, "Order", DataViewRowState.CurrentRows);
                    dtSection = dv.ToTable();
                    if (dtSection.Rows.Count > 0)
                    {
                        foreach (DataRow item in dtSection.Rows)
                        {
                            iBanner = iBanner + item["Description"].ToString();
                        }
                    }
                }
                objBlocksService.GetBlockByCMSId(Convert.ToInt32(dtCMSService.Rows[0]["Id"].ToString()), ref dtMSection);
                if (dtMSection.Rows.Count > 0)
                {
                    string filter = "Position='Middle'";
                    DataView dv = new DataView(dtMSection, filter, "Order", DataViewRowState.CurrentRows);
                    dtMSection = dv.ToTable();
                    if (dtMSection.Rows.Count > 0)
                    {
                        foreach (DataRow item in dtMSection.Rows)
                        {
                            iMBanner = iMBanner + item["Description"].ToString();
                        }
                    }
                }
                objBlocksService.GetBlockByCMSId(Convert.ToInt32(dtCMSService.Rows[0]["Id"].ToString()), ref dtFSection);
                if (dtFSection.Rows.Count > 0)
                {
                    string filter = "Position='Bottom'";
                    DataView dv = new DataView(dtFSection, filter, "Order", DataViewRowState.CurrentRows);
                    dtFSection = dv.ToTable();
                    if (dtFSection.Rows.Count > 0)
                    {
                        foreach (DataRow item in dtFSection.Rows)
                        {
                            iFooter = iFooter + item["Description"].ToString();

                        }
                    }

                }
                //ltBanner.Text = iBanner;
                ltMiddle.Text = iMBanner;
                ltContent.Text = iContent;
                ltBottom.Text = iFooter;
            }
        }
    }
}