using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SON_eStore.Models;
using System.Net;
namespace SON_eStore
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           // GlobalConfiguration.Configure(WebApiConfig.Register);
            var context = new ApplicationDbContext();
            SeedRolesAndUser.Seed(context);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                Response.ClearContent();
                Server.TransferRequest("/estore/Login");
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }
    }
}
