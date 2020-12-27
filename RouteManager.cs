using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zavand.MvcMananaCore
{
    public class RouteManager : IRouteManager
    {
        private readonly EndpointDataSource _endpointDataSource;
        private readonly LinkParser _linkParser;
        protected readonly HashSet<Type> _allRoutes = new HashSet<Type>();

        public RouteManager(EndpointDataSource endpointDataSource, LinkParser linkParser)
        {
            _endpointDataSource = endpointDataSource;
            _linkParser = linkParser;
        }

        public void AddRoute(Type type)
        {
            _allRoutes.Add(type);
        }

        public IBaseRoute CreateRouteByName(string routeName)
        {
            foreach (var type in _allRoutes)
            {
                var r = Activator.CreateInstance(type) as IBaseRoute;
                var routeLocales = r?.GetAllRouteLocales();
                if (routeLocales == null)
                    continue;
                foreach (var routeLocale in routeLocales)
                {
                    r.SetRouteLocale(routeLocale);
                    if (r.GetName() == routeName || r.GetNameLocalized() == routeName)
                        return r;
                }
            }

            return null;
        }

        public IBaseRoute CreateRouteFromUrl(string url, HttpContext httpContext)
        {
            var uri = new Uri(url);
            var routes = new List<IBaseRoute>();
            var selectedEndpoints = new HashSet<string>();
            foreach (var e in _endpointDataSource.Endpoints)
            {
                try
                {
                    var name = e.Metadata.OfType<RouteNameMetadata>().FirstOrDefault()?.RouteName;
                    var rvd = _linkParser.ParsePathByEndpointName(name, uri.AbsolutePath);
                    if (rvd == null) continue;

                    // TODO: For some reason there are several endpoints with the same name. Lets filter out extra.
                    if (selectedEndpoints.Contains(name))
                        continue;

                    selectedEndpoints.Add(name);
                    var r = CreateRouteByName(name);
                    ApplyRouteValues(r, rvd);
                    routes.Add(r);
                }
                catch (Exception ex)
                {
                    var a = 0;
                }
            }

            IBaseRoute route;

            if (routes.Count == 1)
            {
                route = routes.First();
            }
            else
            {
                // Need to determine route.
                // Most likely routes' urls look the same for multiple route locales.
                // We can use HttpContext to make that decision.

                var routeLocale = DetectRouteLocale(httpContext);
                route = routes.FirstOrDefault(m => m.GetRouteLocale() == routeLocale);
            }

            if (route != null)
                ApplyRouteParamsFromQueryString(route, uri);

            return route;
        }

        protected virtual void ApplyRouteParamsFromQueryString(IBaseRoute r, Uri uri)
        {

        }

        protected virtual string DetectRouteLocale(HttpContext httpContext)
        {
            var c = CultureInfo.CurrentCulture;
            while (!c.IsNeutralCulture)
                c = c.Parent;

            return c.Name;
        }

        public virtual void ApplyRouteValues(IBaseRoute r, RouteValueDictionary rvd)
        {
            if (r == null || rvd == null)
                return;

            var properties = r.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in properties)
            {
                if(rvd.ContainsKey(p.Name))
                    p.SetValue(r,Convert.ChangeType(rvd[p.Name], p.PropertyType));
            }
        }

        public virtual void MakeTheSameAs(IBaseRoute rTo, IBaseRoute rFrom)
        {
            rTo.MakeTheSameAs(rFrom);
        }
    }
}