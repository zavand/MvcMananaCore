using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Zavand.MvcMananaCore
{
    public static class BaseExtensions
    {
        public static IHtmlContent ActionLink<T>(this IHtmlHelper helper, string linkText, IBaseRoute currentRoute, T r, object htmlAttributes = null, object extraParams = null) where T : IBaseRoute
        {
            var uh = new UrlHelper(helper.ViewContext);
            var url = uh.RouteUrl(currentRoute, r, extraParams);

            return new HtmlString(String.Format("<a href=\"{0}\"{2}>{1}</a>", url, linkText, GetAttributesString(htmlAttributes)));
        }
//        public static IHtmlContent ActionLink<T>(this IHtmlHelper helper, IUrlHelper uh, string linkText, IBaseRoute currentRoute, T r, object htmlAttributes = null, object extraParams = null) where T : IBaseRoute
//        {
//            var url = uh.RouteUrl(currentRoute, r, extraParams);
//
//            return new HtmlString(String.Format("<a href=\"{0}\"{2}>{1}</a>", url, linkText, GetAttributesString(htmlAttributes)));
//        }

//        public static IHtmlContent ActionLinkCloneGeneral(this IHtmlHelper helper, IUrlHelper uh, string linkText, IBaseRoute currentRoute, Action<IBaseRoute> action, object htmlAttributes = null)
//        {
//            var r = currentRoute.Clone(action);
//            return ActionLink(
//                helper,
//                uh,
//                linkText,
//                null, // No need to follow context as exact clone was created
//                r,    // We created a copy of existing route. No need to follow context.
//                htmlAttributes
//            );
//        }
        public static IHtmlContent ActionLinkCloneGeneral(this IHtmlHelper helper, string linkText, IBaseRoute currentRoute, Action<IBaseRoute> action, object htmlAttributes = null)
        {
            var r = currentRoute.Clone(action);
            return ActionLink(
                helper,
                linkText,
                null, // No need to follow context as exact clone was created
                r,    // We created a copy of existing route. No need to follow context.
                htmlAttributes
            );
        }

//        public static IHtmlContent ActionLinkClone<T>(this IHtmlHelper helper, IUrlHelper uh, string linkText, T currentRoute, Action<T> action, object htmlAttributes = null) where T : IBaseRoute
//        {
//            var r = currentRoute.Clone(action);
//            return ActionLink(
//                helper,
//                uh,
//                linkText,
//                null, // No need to follow context as exact clone was created
//                r,    // We created a copy of existing route. No need to follow context.
//                htmlAttributes
//            );
//        }
                public static IHtmlContent ActionLinkClone<T>(this IHtmlHelper helper, string linkText, T currentRoute, Action<T> action, object htmlAttributes = null) where T : IBaseRoute
                {
                    var r = currentRoute.Clone(action);
                    return ActionLink(
                        helper,
                        linkText,
                        null, // No need to follow context as exact clone was created
                        r,    // We created a copy of existing route. No need to follow context.
                        htmlAttributes
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

        public static void MapRoute<T>(this IRouteBuilder routes, bool isLocalizationSupported) where T : IBaseRoute, new()
        {
            var r = new T();
            if (isLocalizationSupported)
            {
                if (!String.IsNullOrEmpty(r.Area))
                {
                    routes.MapAreaRoute(r.GetNameLocalized(), r.Area, r.GetUrlLocalized(), r.GetDefaults(), r.GetConstraintsLocalized()/*, r.GetNamespaces()*/);
                    var upl = r.GetLocalizedUrlPerLocale();
                    if (upl != null)
                    {
                        foreach (var key in upl.Keys)
                        {
                            var url = upl[key];
                            routes.MapAreaRoute(r.GetNameLocalized()+$"-{key}", r.Area, url, r.GetDefaults(), r.GetConstraintsLocalized()/*, r.GetNamespaces()*/);
                        }
                    }
                }
                else
                {
                    routes.MapRoute(r.GetNameLocalized(), r.GetUrlLocalized(), r.GetDefaults(), r.GetConstraintsLocalized()/*, r.GetNamespaces()*/);
                    var upl = r.GetLocalizedUrlPerLocale();
                    if (upl != null)
                    {
                        foreach (var key in upl.Keys)
                        {
                            var url = upl[key];
                            routes.MapRoute(r.GetNameLocalized()+$"-{key}", url, r.GetDefaults(), r.GetConstraintsLocalized()/*, r.GetNamespaces()*/);
                        }
                    }
                }
            }
            if (!String.IsNullOrEmpty(r.Area))
            {
                routes.MapAreaRoute(r.GetName(), r.Area, r.GetUrl(), r.GetDefaults(), r.GetConstraints()/*, r.GetNamespaces()*/);
            }
            else
            {
                routes.MapRoute(r.GetName(), r.GetUrl(), r.GetDefaults(), r.GetConstraints()/*, r.GetNamespaces()*/);
            }
        }
        //        public static void MapRoute<T>(this RouteCollection routes, bool isLocalizationSupported) where T : IBaseRoute, new()
        //        {
        //            var r = new T();
        //            if (isLocalizationSupported)
        //            {
        //                routes.MapRoute(r.GetNameLocalized(), r.GetUrlLocalized(), r.GetDefaults(), r.GetConstraintsLocalized(), r.GetNamespaces());
        //            }
        //            routes.MapRoute(r.GetName(), r.GetUrl(), r.GetDefaults(), r.GetConstraints(), r.GetNamespaces());
        //        }
        //        public static void MapRoute<T>(this AreaRegistrationContext context, bool isLocalizationSupported) where T : IBaseRoute, new()
        //        {
        //            var r = new T();
        //            if (isLocalizationSupported)
        //            {
        //                context.MapRoute(r.GetNameLocalized(), r.GetUrlLocalized(), r.GetDefaults(), r.GetConstraintsLocalized(), r.GetNamespaces());
        //            }
        //            context.MapRoute(r.GetName(), r.GetUrl(), r.GetDefaults(), r.GetConstraints(), r.GetNamespaces());
        //        }

        public static String RouteUrlClone<T>(this IUrlHelper u, T currentRoute, Action<T> postRouting = null) where T : IBaseRoute
        {
            var r = currentRoute.Clone(postRouting);
            return u.RouteUrl(currentRoute, r);
        }

        public static String RouteUrl<T>(this IUrlHelper u, IBaseRoute currentRoute, T r, object extraParams = null, Action<T> postRouting = null) where T : IBaseRoute
        {
            //UrlHelperExtensions.RouteUrl(u,)

            if (currentRoute != null)
                r.FollowContext(currentRoute);

            postRouting?.Invoke(r);
            var domain = r.GetDomain();

            // ------------------------
            // Route local url
            // ------------------------
            if (String.IsNullOrEmpty(domain))
            {
                var rd = new RouteValueDictionary(r);
                if (String.IsNullOrEmpty(r.Area))
                {
                    rd.Remove("Area");
                }
                string routeName = null;
                if (String.IsNullOrEmpty(r.Locale))
                {
                    rd.Remove("Locale");
                }
                else
                {
                    // Try to get localized URL for specific locale
                    // In example example below two urls point to the same action but looks very different based on used locale:
                    // /en/my-account
                    // /ru/моя-учётная-запись
                    // /рус/моя-учётная-запись
                    // /русский/моя-учётная-запись
                    var upl = r.GetLocalizedUrlPerLocale();
                    if (upl != null)
                    {
                        if (upl.ContainsKey(r.Locale))
                        {
                            routeName = r.GetNameLocalized()+$"-{r.Locale}";
                        }
                    }
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

                if (String.IsNullOrEmpty(routeName))
                {
                    routeName = String.IsNullOrEmpty(r.Locale) ? r.GetName() : r.GetNameLocalized();
                }
                
                return u.RouteUrl(
                    routeName,
                    rd
                    );
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

        public static MvcForm BeginForm(this IHtmlHelper h, IBaseRoute currentRoute, IBaseRoute newRoute = null, FormMethod formMethod = FormMethod.Post, object htmlAttributes = null)
        {
//            var uh = new UrlHelper(h.ViewContext);
            if (newRoute == null)
                newRoute = currentRoute.Clone();
            //            var url = uh.RouteUrl(currentRoute, newRoute);
            newRoute.FollowContext(currentRoute);

            var f = h.BeginRouteForm(String.IsNullOrEmpty(newRoute.Locale)? newRoute.GetName(): newRoute.GetNameLocalized(), newRoute,FormMethod.Post,null,htmlAttributes);
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

        public static MvcForm BeginFormFileUpload(this IHtmlHelper h, IBaseRoute currectRoute, IBaseRoute newRoute = null, FormMethod formMethod = FormMethod.Post, object htmlAttributes = null)
        {
            var rv=new RouteValueDictionary(htmlAttributes);
            if (!rv.ContainsKey("enctype"))
            {
                rv.Add("enctype", "multipart/form-data");
            }
            return h.BeginForm(currectRoute, newRoute, htmlAttributes: rv);
        }

//        public static MvcHtmlString FileUploadFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, System.Linq.Expressions.Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
//        {
//            return htmlHelper.TextBoxFor(expression, new RouteValueDictionary(htmlAttributes) { { "type", "file" } });
//        }

    }
}