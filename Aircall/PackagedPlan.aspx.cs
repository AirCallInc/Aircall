using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using System.Text;
using System.IO;
using System.Web.UI.HtmlControls;
using Aircall.Common;

namespace Aircall
{
    public partial class PackagedPlan : System.Web.UI.Page
    {
        IBlocksService objBlocksService=ServiceFactory.BlocksService;
        ICMSPagesService objCMSService=ServiceFactory.CMSPagesService;
        DataTable dtCMSService = new DataTable();
        DataTable dtCMSSectionService = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMobileMenu();
                BindHeader();

                string url = Page.RouteData.Values["Plan"] as string;
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
                objCMSService.GetPageContent(str, ref dtCMSService);
                if (dtCMSService.Rows.Count > 0)
                {
                    InnerHTML(str);
                }
                else
                {
                    str = "404";
                    InnerHTML(str);
                }

                BindFooter();
            }
        }


        private void BindMobileMenu()
        {
            objBlocksService = ServiceFactory.BlocksService;
            DataTable dtBlock = new DataTable();
            objBlocksService.GetBlockContentByBlockName("Mobile Header", ref dtBlock);
            if (dtBlock.Rows.Count > 0)
            {

                if (Session["ClientLoginCookie"] != null)
                {
                    controls.mobileheadernew login = LoadControl("~/controls/mobileheadernew.ascx") as controls.mobileheadernew;
                    StringBuilder myStringBuilder = new StringBuilder();
                    TextWriter myTextWriter = new StringWriter(myStringBuilder);
                    HtmlTextWriter myWriter = new HtmlTextWriter(myTextWriter);
                    HtmlAnchor link = (HtmlAnchor)login.FindControl("lnkUsername");
                    Label ltrCnt = (Label)login.FindControl("ltrCnt");
                    if (link != null)
                    {
                        link.InnerText = (Session["ClientLoginCookie"] as LoginModel).FullName;
                    }
                    login.RenderControl(myWriter);
                    string html = myTextWriter.ToString();

                    dtBlock.Rows[0]["Description"] = dtBlock.Rows[0]["Description"].ToString().Replace("{{UserArea}}", html);
                }
                else
                {
                    dtBlock.Rows[0]["Description"] = dtBlock.Rows[0]["Description"].ToString().Replace("{{UserArea}}", "Navigation");
                }
                ltrMobileMenu.Text = dtBlock.Rows[0]["Description"].ToString();
            }
        }

        private void BindHeader()
        {
            objBlocksService = ServiceFactory.BlocksService;
            DataTable dtBlock = new DataTable();
            objBlocksService.GetBlockContentByBlockName("Header", ref dtBlock);
            if (dtBlock.Rows.Count > 0)
            {
                if (Session["ClientLoginCookie"] != null)
                {
                    IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
                    IClientAddressService objClientAddressService = ServiceFactory.ClientAddressService;

                    controls.AfterLogin login = LoadControl("~/controls/AfterLogin.ascx") as controls.AfterLogin;
                    StringBuilder myStringBuilder = new StringBuilder();
                    TextWriter myTextWriter = new StringWriter(myStringBuilder);
                    HtmlTextWriter myWriter = new HtmlTextWriter(myTextWriter);
                    HtmlAnchor link = (HtmlAnchor)login.FindControl("lnkUsername");
                    Label ltrCnt = (Label)login.FindControl("ltrCnt");
                    if (link != null)
                    {
                        link.InnerText = (Session["ClientLoginCookie"] as LoginModel).FullName;
                    }
                    if (ltrCnt != null)
                    {
                        int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;

                        DataTable dtAddress = new DataTable();
                        var AddressId = 0;
                        objClientAddressService.GetClientAddressesByClientId(ClientId, true,ref dtAddress);
                        if (dtAddress.Rows.Count > 0)
                        {
                            if (dtAddress.Rows.Count == 1)
                            {
                                AddressId = int.Parse(dtAddress.Rows[0]["Id"].ToString());
                            }
                            else
                            {
                                var rows = dtAddress.Select(" IsDefaultAddress = true ");
                                AddressId = int.Parse(rows[0]["Id"].ToString());
                            }
                            DataTable dtService = new DataTable();
                            objUserNotificationService.GetAllNotificationByUserId(ClientId, ref dtService);
                            if (dtService.Rows.Count > 0)
                            {
                                var rows = dtService.Select(" Status ='UnRead'");
                                if (rows.Length > 0)
                                {
                                    ltrCnt.Text = rows.Length.ToString(); ;
                                }
                            }
                        }
                    }
                    login.RenderControl(myWriter);
                    string html = myTextWriter.ToString();

                    dtBlock.Rows[0]["Description"] = dtBlock.Rows[0]["Description"].ToString().Replace("{{UserArea}}", html);
                }
                else
                {
                    controls.BeforeSignUp login = LoadControl("~/controls/BeforeSignUp.ascx") as controls.BeforeSignUp;
                    StringBuilder myStringBuilder = new StringBuilder();
                    TextWriter myTextWriter = new StringWriter(myStringBuilder);
                    HtmlTextWriter myWriter = new HtmlTextWriter(myTextWriter);
                    login.RenderControl(myWriter);
                    string html = myTextWriter.ToString();
                    dtBlock.Rows[0]["Description"] = dtBlock.Rows[0]["Description"].ToString().Replace("{{UserArea}}", html);
                }
                ltrHeader.Text = dtBlock.Rows[0]["Description"].ToString();
            }
        }

        private void BindFooter()
        {
            objBlocksService = ServiceFactory.BlocksService;
            DataTable dtBlock = new DataTable();
            objBlocksService.GetBlockContentByBlockName("Footer", ref dtBlock);
            if (dtBlock.Rows.Count > 0)
            {
                ltrFooter.Text = dtBlock.Rows[0]["Description"].ToString();
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
            else
            {
                InnerHTML("404");
            }

            if (dtCMSService.Rows.Count > 0)
            {
                Page.Title = dtCMSService.Rows[0]["MetaTitle"].ToString();
                Page.MetaDescription = dtCMSService.Rows[0]["MetaDescription"].ToString();
                Page.MetaKeywords = dtCMSService.Rows[0]["MetaKeywords"].ToString();
                ltrAdditionalMeta.Text = dtCMSService.Rows[0]["AdditionalMeta"].ToString();

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