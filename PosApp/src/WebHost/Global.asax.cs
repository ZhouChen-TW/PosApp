using System;
using System.Web.Http;
using PosApp;

namespace WebHost
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            var webApiConfig = GlobalConfiguration.Configuration;
            new Bootstrap().Initialize(webApiConfig);
        }
    }
}