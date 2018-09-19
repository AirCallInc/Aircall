using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class NewsDetails : System.Web.UI.Page
    {
        IBlocksService objBlocksService = ServiceFactory.BlocksService;
        INewsService objNewsService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMobileMenu();
                BindHeader();

                BindNewsDetails();
                BindFooter();
            }
        }

        private void BindNewsDetails()
        {
            string url = Page.RouteData.Values["NewsUrl"] as string;
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

            DataTable dtNews = new DataTable();
            objNewsService = ServiceFactory.NewsService;
            objNewsService.GetNewsByUrl(str, ref dtNews);
            if (dtNews.Rows.Count > 0)
            {
                Page.Title = dtNews.Rows[0]["MetaTitle"].ToString();
                Page.MetaKeywords = dtNews.Rows[0]["MetaKeywords"].ToString();
                Page.MetaDescription = dtNews.Rows[0]["MetaDescription"].ToString();
                ltrAdditionalMeta.Text = dtNews.Rows[0]["AdditionalMeta"].ToString();
                ltrMonthDay.Text = Convert.ToDateTime(dtNews.Rows[0]["PublishDate"].ToString()).ToString("MMM") + " " + dtNews.Rows[0]["Day"].ToString();
                ltrYear.Text = dtNews.Rows[0]["Year"].ToString();
                ltrNewsDetail.Text = dtNews.Rows[0]["Description"].ToString();
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
    }
}