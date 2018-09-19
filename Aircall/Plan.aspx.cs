using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class Plan : System.Web.UI.Page
    {
        #region Declaration
        ICMSPagesService objCMSService = ServiceFactory.CMSPagesService;
        IBlocksService objSectionService = ServiceFactory.BlocksServices;
        DataTable dtCMSService = new DataTable();
        DataTable dtCMSSectionService = new DataTable();
        #endregion

        public string innerHTML = String.Empty;
        public string innerBanner = String.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {            
                string url = Page.RouteData.Values["Plan"] as string;
                string str = "";
                if (url != null)
                {
                    url = url.Replace("/", "");
                    str = url.Split('.')[0].ToString();

                }
                else
                {
                    str = "index";
                }
                InnerHTML(str);
            }
        }
        private void InnerHTML(string str)
        {
            string iBanner = String.Empty;

            string iMBanner = String.Empty;
            string iFooter = String.Empty;
            string iContent = String.Empty;
            DataTable dtSection = new DataTable();
            DataTable dtMSection = new DataTable();
            DataTable dtFSection = new DataTable();
            objCMSService.GetPageContent(str, ref dtCMSService);

            if (dtCMSService.Rows.Count > 0)
            {
                foreach (DataRow item in dtCMSService.Rows)
                {
                    iContent = iContent + item["Description"].ToString();

                }
                BanngerImg.Attributes.Add("style", "background-image:url('" + Application["SiteAddress"] + "uploads/BannerImg/" + dtCMSService.Rows[0]["BannerImage"].ToString() + "')");

                ltBannerText.Text = dtCMSService.Rows[0]["PageTitle"].ToString();
            }

            if (dtCMSService.Rows.Count > 0)
            {
                Page.Title = dtCMSService.Rows[0]["PageTitle"].ToString();
                objSectionService.GetBlockByCMSId(Convert.ToInt32(dtCMSService.Rows[0]["Id"].ToString()), ref dtSection);
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
                objSectionService.GetBlockByCMSId(Convert.ToInt32(dtCMSService.Rows[0]["Id"].ToString()), ref dtMSection);
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
                objSectionService.GetBlockByCMSId(Convert.ToInt32(dtCMSService.Rows[0]["Id"].ToString()), ref dtFSection);
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

                ltBanner.Text = iBanner;
                ltMiddle.Text = iMBanner;
                ltContent.Text = iContent;
                ltBottom.Text = iFooter;


            }
        }
    }
}