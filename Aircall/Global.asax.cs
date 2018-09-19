using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Aircall
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
            Application["SiteAddress"] = ConfigurationManager.AppSettings["SiteAddress"].ToString().Trim();
            Application["APIURL"] = ConfigurationManager.AppSettings["APIURL"].ToString().Trim();
            Application["PageSize"] = ConfigurationManager.AppSettings["PageSize"].ToString().Trim();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //HttpContext ctx = HttpContext.Current;
            //Exception ex = ctx.Server.GetLastError();

            //if (ex is HttpRequestValidationException)
            //{
            //    ctx.Server.ClearError();
            //    if (ctx.Request.Path.ToLower().Contains("/admin/"))
            //    {
            //        ctx.Response.Redirect("/admin/NotFound.aspx");
            //    }
            //    else if (ctx.Request.Path.ToLower().Contains("/client/"))
            //    {
            //        ctx.Response.Redirect("/client/NotFound.aspx");
            //    }
            //    else
            //    {
            //        ctx.Response.Redirect("/404.aspx");
            //    }
            //}
            //else
            //{
            //    ctx.Server.ClearError();
            //    if (ctx.Request.Path.ToLower().Contains("/admin/"))
            //    {
            //        ctx.Response.Redirect("/admin/NotFound.aspx");
            //    }
            //    else if (ctx.Request.Path.ToLower().Contains("/client/"))
            //    {
            //        ctx.Response.Redirect("/client/NotFound.aspx");
            //    }
            //    else
            //    {
            //        ctx.Response.Redirect("/404.aspx");
            //    }
            //}
        }

        void RegisterRoutes(RouteCollection routes)
        {

            routes.Ignore("{resource}.axd/{*pathInfo}");
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "admin" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "assets" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "Certificate" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "client" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "Common" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "controls" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "Certificate" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "css" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "fonts" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "images" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "js" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "NotificationErrorLog" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "partner" });
            routes.Ignore("{folder}/{*pathInfo}", new { folder = "uploads" });
            routes.Ignore("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.MapPageRoute("CMSPlan", "{Plan}", "~/PackagedPlan.aspx");
            routes.MapPageRoute("NewsDetail", "News/{NewsUrl}", "~/NewsDetails.aspx");
        }
    }
}