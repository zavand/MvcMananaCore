using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Zavand.MvcMananaCore
{
    public static class BaseExtensions
    {
        public static IHtmlContent ActionLink<T>(this IHtmlHelper helper, string linkText, IBaseRoute currentRoute, T r, object htmlAttributes = null, Action<IBaseRoute> postRouting = null, object extraParams = null, bool skipFollowContext=false) where T : IBaseRoute
        {
            return helper.ActionLink(linkText, currentRoute, null, r, htmlAttributes, postRouting, extraParams, skipFollowContext);
        }

        public static IHtmlContent ActionLink<T>(this IHtmlHelper helper, string linkText, IBaseRoute currentRoute, IUrlHelper urlHelper, T r, object htmlAttributes = null, Action<IBaseRoute> postRouting = null, object extraParams = null, bool skipFollowContext = false)
            where T : IBaseRoute
        {
            if (urlHelper == null)
                urlHelper = new UrlHelper(helper.ViewContext);
            return ActionLink(urlHelper,linkText,currentRoute,r,htmlAttributes,postRouting,extraParams,skipFollowContext);
        }

        public static IHtmlContent ActionLinkClone<T>(this IHtmlHelper helper, string linkText, T currentRoute, IUrlHelper urlHelper, Action<T> action, object htmlAttributes = null)
            where T : IBaseRoute
        {
            var r = currentRoute.Clone(action);
            return ActionLink(urlHelper, linkText, currentRoute, r, htmlAttributes, skipFollowContext: true);
        }

        public static IHtmlContent ActionLink<T>(IUrlHelper urlHelper, string linkText, IBaseRoute currentRoute, T r, object htmlAttributes = null, Action<IBaseRoute> postRouting = null, object extraParams = null, bool skipFollowContext = false) where T : IBaseRoute
        {
            var url = urlHelper.RouteUrl(currentRoute, r, extraParams, postRouting, skipFollowContext);
            return new HtmlString(GetAnchor(url, linkText, htmlAttributes));
        }

        public static string GetAnchor(string url, string linkText, object htmlAttributes = null)
        {
            return String.Format("<a href=\"{0}\"{2}>{1}</a>", url, linkText, GetAttributesString(htmlAttributes));
        }

        public static IHtmlContent ActionLinkCloneGeneral(this IHtmlHelper helper, string linkText, IBaseRoute currentRoute, Action<IBaseRoute> action, object htmlAttributes = null)
        {
            var r = currentRoute.Clone(action);
            return ActionLink(
                helper,
                linkText,
                currentRoute,
                r,
                htmlAttributes,
                skipFollowContext:true
            );
        }

        public static IHtmlContent ActionLinkClone<T>(this IHtmlHelper helper, string linkText, T currentRoute, Action<T> action, object htmlAttributes = null) where T : IBaseRoute
        {
            var r = currentRoute.Clone(action);
            return ActionLink(
                helper,
                linkText,
                currentRoute,
                r,
                htmlAttributes,
                skipFollowContext:true
            );
        }

        static string GetAttributesString(object htmlAttributes)
        {
            var attributes = "";
            if (htmlAttributes != null)
            {
                RouteValueDictionary d;
                if (htmlAttributes is IDictionary<string, object>)
                    d = new RouteValueDictionary((IDictionary<string, object>)htmlAttributes);
                else
                    d = new RouteValueDictionary(htmlAttributes);
                foreach (var k in d)
                {
                    attributes += String.Format(" {0}=\"{1}\"", k.Key, k.Value);
                }
            }
            return attributes;
        }

        public static void MapRoute<T>(this IRouteBuilder routes, string[] locales = null) where T : IBaseRoute, new()
        {
            var r = new T();

            var isLocalizationSupported = locales != null && locales.Any();
            if (isLocalizationSupported)
            {
                if (!String.IsNullOrEmpty(r.Area))
                {
                    routes.MapAreaRoute(r.GetNameLocalized(), r.Area, r.GetUrlLocalized(), r.GetDefaults(), r.GetConstraintsLocalized(locales));
                }
                else
                {
                    routes.MapRoute(r.GetNameLocalized(), r.GetUrlLocalized(), r.GetDefaults(), r.GetConstraintsLocalized(locales));
                }
            }
            if (!String.IsNullOrEmpty(r.Area))
            {
                routes.MapAreaRoute(r.GetName(), r.Area, r.GetUrl(), r.GetDefaults(), r.GetConstraints());
            }
            else
            {
                routes.MapRoute(r.GetName(), r.GetUrl(), r.GetDefaults(), r.GetConstraints());
            }
        }

        public static String RouteUrlClone<T>(this IUrlHelper u, T currentRoute, Action<T> postRouting = null) where T : IBaseRoute
        {
            var r = currentRoute.Clone(postRouting);
            return u.RouteUrl(currentRoute, r);
        }

        public static String RouteUrl<T>(this IUrlHelper u, IBaseRoute currentRoute, T r, object extraParams = null, Action<IBaseRoute> postRouting = null, bool skipFollowContext=false) where T : IBaseRoute
        {
            if (!skipFollowContext && currentRoute != null)
                r.FollowContext(currentRoute);

            var finalRoute = GetFinalRoute(r,currentRoute);

            postRouting?.Invoke(finalRoute);

            var domain = finalRoute.GetDomain();

            // ------------------------
            // Route local url
            // ------------------------
            if (String.IsNullOrEmpty(domain))
            {
                var rd = new RouteValueDictionary(finalRoute);
                if (String.IsNullOrEmpty(finalRoute.Area))
                {
                    rd.Remove("Area");
                }
                if (String.IsNullOrEmpty(finalRoute.Locale))
                {
                    rd.Remove("Locale");
                }

                if (extraParams != null)
                {
                    var extraParamsDictionary = new RouteValueDictionary(extraParams);
                    foreach (var k in extraParamsDictionary.Keys)
                    {
                        if (rd.ContainsKey(k))
                            rd[k] = extraParamsDictionary[k];
                        else
                            rd.Add(k, extraParamsDictionary[k]);
                    }
                }

                var routeName = String.IsNullOrEmpty(finalRoute.Locale) ? finalRoute.GetName() : finalRoute.GetNameLocalized();

                var url= u.RouteUrl(
                    routeName,
                    rd
                    );

                var queryParams = finalRoute.GetQueryString();
                if (!String.IsNullOrEmpty(queryParams))
                {
                    if (url.Contains('?'))
                        url += "&" + queryParams; //HttpUtility.UrlEncode(queryParams);
                    else
                        url += "?" + queryParams;//HttpUtility.UrlEncode(queryParams);
                }

                if (!String.IsNullOrEmpty(finalRoute.GetAnchor()))
                    url += $"#{HttpUtility.UrlEncode(finalRoute.GetAnchor())}";

                return url;
            }

            // ------------------------
            // Route external domain
            // ------------------------
            var protocol = r.GetProtocol();
            var port = r.GetPort();

            // Determine required protocol
//            if (protocol == BaseRoute.UrlProtocol.Inherited)
//            {
//                // Try to get protocol from current context
//                try
//                {
//                    if (u.RequestContext.HttpContext.Request.Url != null &&
//                        !Enum.TryParse(u.RequestContext.HttpContext.Request.Url.Scheme, true, out protocol))
//                    {
//                        throw new Exception(); // Protocol can't be parsed. Internal exception.
//                    }
//                }
//                catch // In case if anything of following is null: u.RequestContext.HttpContext.Request.Url
//                {
//                    protocol = BaseRoute.UrlProtocol.Http;
//                }
//            }
            var protocolUrl = protocol.ToString().ToLower();

            // Determine required port
            var porturl = "";
            if (protocol == BaseRoute.UrlProtocol.Http && port == BaseRoute.DefaultHttpPort)
                port = 0;
            if (protocol == BaseRoute.UrlProtocol.Https && port == BaseRoute.DefaultHttpsPort)
                port = 0;

            if (port != 0)
                porturl = String.Format(":{0}", port);

            var path = r.GetUrl();
            if (!String.IsNullOrEmpty(path))
                path = "/" + path;

            return String.Format("{0}://{1}{2}{3}", protocolUrl, domain, porturl, path);
        }

        public static MvcForm BeginForm(this IHtmlHelper h, IBaseRoute currentRoute, IBaseRoute newRoute = null, FormMethod formMethod = FormMethod.Post, object htmlAttributes = null, Action<IBaseRoute> postRouting = null)
        {
//            var uh = new UrlHelper(h.ViewContext);
            if (newRoute == null)
                newRoute = currentRoute.Clone();
            //            var url = uh.RouteUrl(currentRoute, newRoute);

            newRoute.FollowContext(currentRoute);
            var finalRoute = GetFinalRoute(newRoute,currentRoute);

            postRouting?.Invoke(finalRoute);

            var f = h.BeginRouteForm(String.IsNullOrEmpty(finalRoute.Locale) ? finalRoute.GetName() : finalRoute.GetNameLocalized(), finalRoute, FormMethod.Post, null, htmlAttributes);
            return f;
//            var start = String.Format("<form action=\"{0}\" method=\"{1}\" enctype=\"multipart/form-data\"{2}>", url, formMethod.ToString().ToUpper(), GetAttributesString(htmlAttributes));
//            var end = "</form>";
//
//            return new MvcForm(h.ViewContext, htmlEncoder);
        }

//        public static MvcForm BeginFormFileUpload(this IHtmlHelper h, IBaseRoute currectRoute)
//        {
//            return h.BeginForm(currectRoute.Action, currectRoute.Controller, currectRoute, FormMethod.Post, null, new { enctype = "multipart/form-data" });
//        }

        public static MvcForm BeginFormFileUpload(this IHtmlHelper h, IBaseRoute currentRoute, IBaseRoute newRoute = null, FormMethod formMethod = FormMethod.Post, object htmlAttributes = null)
        {
            var rv=new RouteValueDictionary(htmlAttributes);
            if (!rv.ContainsKey("enctype"))
            {
                rv.Add("enctype", "multipart/form-data");
            }
            return h.BeginForm(currentRoute, newRoute, htmlAttributes: rv);
        }

//        public static MvcHtmlString FileUploadFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, System.Linq.Expressions.Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
//        {
//            return htmlHelper.TextBoxFor(expression, new RouteValueDictionary(htmlAttributes) { { "type", "file" } });
//        }

        public static void MapControllerRoute<TRoute>(this IEndpointRouteBuilder endpoints, string[] locales = null) where TRoute:IBaseRoute, new()
        {
            var isLocalizationSupported = locales != null && locales.Any();

            var r = new TRoute();

            var routeLocales = r.GetAllRouteLocales();

            // Make sure route is registered even it doesn't have any route locale
            if (routeLocales == null || !routeLocales.Any())
                routeLocales = new[] {""};

            var registeredUrls = new List<string>();
            var registeredNames = new List<string>();
            foreach (var routeLocale in routeLocales)
            {
                r.SetRouteLocale(routeLocale);

                if (isLocalizationSupported)
                {
                    registeredUrls.Add(r.GetUrlLocalized());
                    registeredNames.Add(r.GetNameLocalized());
                    if (!String.IsNullOrEmpty(r.Area))
                    {
                        endpoints.MapAreaControllerRoute(r.GetNameLocalized(), r.Area, r.GetUrlLocalized(), r.GetDefaults(), r.GetConstraintsLocalized(locales));
                    }
                    else
                    {
                        endpoints.MapControllerRoute(r.GetNameLocalized(), r.GetUrlLocalized(), r.GetDefaults(), r.GetConstraintsLocalized(locales));
                    }
                }

                registeredUrls.Add(r.GetUrl());
                registeredNames.Add(r.GetName());
                if (!String.IsNullOrEmpty(r.Area))
                {
                    endpoints.MapAreaControllerRoute(r.GetName(), r.Area, r.GetUrl(), r.GetDefaults(), r.GetConstraints());
                }
                else
                {
                    endpoints.MapControllerRoute(r.GetName(), r.GetUrl(), r.GetDefaults(), r.GetConstraints());
                }
            }
        }

        public static string GetPathByRoute<TRoute>(this LinkGenerator linkGenerator, TRoute r, IBaseRoute currentRoute = null) where TRoute:IBaseRoute
        {
            var finalRoute = GetFinalRoute(r,currentRoute);
            var url = linkGenerator.GetPathByRouteValues(String.IsNullOrEmpty(finalRoute.Locale) ? finalRoute.GetName() : finalRoute.GetNameLocalized(), finalRoute);
            return url;
        }

        public static IBaseRoute GetFinalRoute<TRoute>(TRoute r, IBaseRoute currentRoute = null) where TRoute:IBaseRoute
        {
            IBaseRoute finalRoute = r;

            var currentRouteLocale = currentRoute?.GetRouteLocale();
            var newRouteLocale = finalRoute.GetRouteLocale();

            if (String.IsNullOrEmpty(currentRouteLocale))
            {
                var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
                while (!ci.IsNeutralCulture)
                {
                    ci = ci.Parent;
                }

                currentRouteLocale = ci.Name;
            }

            if (newRouteLocale != currentRouteLocale && !String.IsNullOrEmpty(currentRouteLocale))
            {
                finalRoute = finalRoute.CreateLocalizedRoute(currentRouteLocale);
            }

            return finalRoute;
        }

        public static IBaseRoute GetCurrentRoute(this HttpContext context)
        {
            var ep = context.GetEndpoint();
            var rd = context.GetRouteData();
            var cad = ep.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
            var routeName = ep.Metadata.OfType<RouteNameMetadata>().FirstOrDefault();

            var currentRoute = cad.Parameters
                    .Where(m => typeof(IBaseRoute).IsAssignableFrom(m.ParameterType))
                    .Select(m=>(IBaseRoute)Activator.CreateInstance(m.ParameterType))
                    .FirstOrDefault()
                ;

            if(currentRoute!=null)
            {
                var t = currentRoute.GetType();
                var pp = t.GetProperties()
                    .Where(m => m.CanWrite)
                    .ToArray();

                // Update current route with values from route data
                foreach (var value in rd.Values)
                {
                    ReflectionExtensions.SetValue(pp, value.Key,currentRoute,value.Value.ToString());
                }

                // Update current route with values from query string
                foreach (var k in context.Request.Query.Keys)
                {
                    ReflectionExtensions.SetValue(pp, k,currentRoute,context.Request.Query[k].Select(m=>m));
                }
            }

            return currentRoute;
        }

        public static int GetLastPage<TRoute>(this IPageableModel<TRoute> m)
            where TRoute : IBaseRoute
        {
            if (m.PageSize == 0)
                return 0;

            return (int) m.Total / m.PageSize + ((int) m.Total % m.PageSize == 0 ? 0 : 1);
        }
    }
}
