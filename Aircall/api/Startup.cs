using api.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.IdentityModel;
using System.Web.Http.Dispatcher;
using api.ActionFilters;
using System.Web.Http.Controllers;

[assembly: OwinStartup(typeof(api.Startup))]
namespace api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigureOAuthTokenGeneration(app);

            //ConfigureOAuthTokenConsumption(app);

            ConfigureWebApi(httpConfig);
            //WebApiConfig.Register(httpConfig);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(httpConfig);

            //httpConfig.Services.Replace(typeof(IHttpControllerSelector), new HttpNotFoundAwareDefaultHttpControllerSelector(httpConfig));

            //httpConfig.Services.Replace(typeof(IHttpActionSelector), new HybridActionSelector());

        }
        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString())),
                Provider = new CustomOAuthProvider(),
                RefreshTokenProvider = new ApplicationRefreshTokenProvider(),
                AccessTokenFormat = new CustomJwtFormat(ConfigurationManager.AppSettings["APIURL"].ToString())
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {

            var issuer = ConfigurationManager.AppSettings["APIURL"].ToString();
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Filters.Add(new LoggingFilterAttribute());
            //config.Filters.Add(new GlobalExceptionAttribute());
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            //jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();            
        }
    }
}