using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;


namespace DatabaseManager
{
    public static class Authentication
    {
        public const String ApplicationCookie = "DatabaseMgrAuthenticationType";
    }

    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Set cookie properties.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = Authentication.ApplicationCookie,
                LoginPath = new PathString("/Login"),
                Provider = new CookieAuthenticationProvider(),
                CookieName = "DatabaseMgr",
                CookieHttpOnly = true,
                ExpireTimeSpan = TimeSpan.FromHours(5208),
            });
        }
    }
}