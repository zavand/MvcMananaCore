using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zavand.MvcMananaCore
{
    public interface IBaseRoute
    {
        string Controller { get; set; }
        string Action { get; set; }
        string Area { get; set; }
        string GetName();
        string GetNameLocalized();
        string GetUrl();
        object GetDefaults();
        object GetConstraints();
        string[] GetNamespaces();
        void FollowContext(IBaseRoute r);
        void MakeTheSameAs(IBaseRoute r);
        string GetDomain();
        void SetDomain(string domain);
        BaseRoute.UrlProtocol GetProtocol();
        void SetProtocol(BaseRoute.UrlProtocol protocol);
        int GetPort();
        void SetPort(int port);
        T Clone<T>(Action<T> action) where T : IBaseRoute;
        IBaseRoute Clone();
        IBaseRoute GetParentRoute();
        void SetParentRoute(IBaseRoute parentRoute);

        string GetAnchor();
        void SetAnchor(string anchor);

        /// <summary>
        /// Adds query param.
        /// If query param with the same name exists then it will be appended to collection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddQueryParam(string name, string value);
        /// <summary>
        /// Sets query param.
        /// If query param with the same name exists then it will be replaced.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetQueryParam(string name, string value);
        void RemoveQueryParam(string name);
        string GetQueryParam(string name);
        string[] GetQueryParams(string name);

        void AddQueryParamWithoutValue(string name);
        void RemoveQueryParamWithoutValue(string name);
        bool GetQueryParamWithoutValue(string name);
        void SetQueryParams(IQueryCollection q);
        string GetQueryString();

        void Map(IEndpointRouteBuilder endpoints);
    }
}