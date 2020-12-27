using System;
using Microsoft.AspNetCore.Http;

namespace Zavand.MvcMananaCore
{
    public interface IRouteManager
    {
        void AddRoute(Type type);
        IBaseRoute CreateRouteByName(string routeName);
        IBaseRoute CreateRouteFromUrl(string url, HttpContext httpContext);
        void MakeTheSameAs(IBaseRoute rTo, IBaseRoute rFrom);
    }
}