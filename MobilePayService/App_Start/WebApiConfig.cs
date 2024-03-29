﻿using MobilePayService.Authentication;
using MobilePayService.RestAPI;
using System.Web.Http;

namespace MobilePayService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Filters.Add(new BasicAuthenticationAttribute());
            config.MessageHandlers.Add(new WrappingHandler());
        }
    }
}
