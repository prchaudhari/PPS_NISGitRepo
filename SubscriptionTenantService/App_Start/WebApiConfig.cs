using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SubscriptionTenantService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Filters.Add(new HandleExceptionFilter());
            // Web API configuration and services
            config.DependencyResolver = new IoCContainer(Container.GetUnityContainer());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
