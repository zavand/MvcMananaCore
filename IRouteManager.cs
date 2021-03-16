using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Zavand.MvcMananaCore
{
    public interface IRouteManager
    {
        void AddRoute(Type type);
        IBaseRoute CreateRouteByName(string routeName);
        IBaseRoute CreateRouteFromUrl(string url, HttpContext httpContext);
        void MakeTheSameAs(IBaseRoute rTo, IBaseRoute rFrom);

        /// <summary>
        /// Adds duplicated route.<br/>
        /// Duplicated route is the route which have the same url templates for different route locales
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selectMany"></param>
        void AddDuplicatedRoute(Type type, IEnumerable<int> lcids);
        bool IsDuplicatedRoute(Type type, int lcid);
    }
}