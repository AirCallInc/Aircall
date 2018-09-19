using api.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace api.Providers
{
    public class ApplicationRefreshTokenProvider : AuthenticationTokenProvider
    {
        public override void Create(AuthenticationTokenCreateContext context)
        {
            // Expiration time in seconds
            int expire = Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString());
            context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(expire));
            context.SetToken(context.SerializeTicket());
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }
    }

    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }


        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = "*";

            Aircall_DBEntities1 db = new Aircall_DBEntities1();
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var user = db.Clients.Where(x => x.Email == context.UserName && x.Password == context.Password);
            var employee = db.Employees.Where(x => x.Email == context.UserName && x.Password == context.Password);
            if (user == null && employee == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            
            //identity.AddClaim(new Claim("role", "user"));

            //ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");
            //oAuthIdentity.AddClaim(new Claim("newClaim", "refreshToken"));
            //oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));

            var ticket = new AuthenticationTicket(identity, null);
            context.Validated(ticket);

        }


    }
}