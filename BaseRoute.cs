using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Zavand.MvcMananaCore
{
    public class BaseRoute : IBaseRoute
    {
        public const int DefaultHttpPort = 80;
        public const int DefaultHttpsPort = 443;

        private string _domain;
        private UrlProtocol _protocol = UrlProtocol.Inherited;
        /// <summary>
        /// Port to access the resource
        /// If port is zero (default value) it will not be added to the url
        /// </summary>
        private int _port;

        private IBaseRoute _parentRoute;
        private string _anchor;
        private readonly Dictionary<string, List<string>> _queryParams = new();
        private readonly HashSet<string> _queryParamsWithoutValue = new();

        public virtual void AddQueryParam(string name, string value)
        {
            if(!_queryParams.ContainsKey(name))
                _queryParams.Add(name, new List<string>());
            _queryParams[name].Add(value);
        }

        public virtual void SetQueryParam(string name, string value)
        {
            if(!_queryParams.ContainsKey(name))
                _queryParams.Add(name, new List<string>());
            else
                _queryParams[name].Clear();
            _queryParams[name].Add(value);
        }

        public void RemoveQueryParam(string name)
        {
            if (_queryParams.ContainsKey(name))
                _queryParams.Remove(name);
        }

        public virtual string GetQueryParam(string name)
        {
            if (!_queryParams.ContainsKey(name))
                return null;

            return String.Join(",", _queryParams[name]);
        }

        public virtual string[] GetQueryParams(string name)
        {
            if (!_queryParams.ContainsKey(name))
                return new string[0];

            return _queryParams[name].ToArray();
        }

        public void AddQueryParamWithoutValue(string name)
        {
            _queryParamsWithoutValue.Add(name);
            ;
        }

        public void RemoveQueryParamWithoutValue(string name)
        {
            _queryParamsWithoutValue.Remove(name);
        }

        public bool GetQueryParamWithoutValue(string name)
        {
            return _queryParamsWithoutValue.Contains(name);
        }

        public virtual void SetQueryParams(IQueryCollection q)
        {
            foreach (var k in q.Keys)
            {
                var kDecoded = HttpUtility.UrlDecode(k);
                foreach (var v in q[k])
                {
                    if (!String.IsNullOrEmpty(v))
                    {
                        if (q[k].Count == 1)
                            SetQueryParam(kDecoded, v);
                        else
                            AddQueryParam(kDecoded, v);
                    }
                    else
                        AddQueryParamWithoutValue(kDecoded);
                }
            }
        }

        public virtual string GetQueryString()
        {
            return String.Join("&", _queryParams
                .SelectMany(m => m.Value
                    .Where(v => !String.IsNullOrEmpty(v))
                    .Select(a => $"{HttpUtility.UrlEncode(m.Key)}={HttpUtility.UrlEncode(a)}")
                )
                .Union(_queryParamsWithoutValue)
            );
        }

        public virtual void Map(IEndpointRouteBuilder endpoints)
        {
            if (!String.IsNullOrEmpty(Area))
            {
                endpoints.MapAreaControllerRoute(GetName(), Area, GetUrl(), GetDefaults(), GetConstraints());
            }
            else
            {
                endpoints.MapControllerRoute(GetName(), GetUrl(), GetDefaults(), GetConstraints());
            }
        }

        public string GetAnchor()
        {
            return _anchor;
        }

        public void SetAnchor(string anchor)
        {
            _anchor = anchor;
        }

        public enum UrlProtocol
        {
            /// <summary>
            /// Protocol will be inherited from current request
            /// </summary>
            Inherited,
            Http,
            Https,
            Ftp,
            Ftps
        }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }

        /// <summary>
        /// Default name of controller
        /// If Controller property is equals to DefaultControllerName then it will be avoided in the route url
        /// </summary>
        protected string DefaultControllerName { get; set; }

        /// <summary>
        /// Default name of action
        /// If Action property is equals to DefaultActionName then it will be avoided in the route url
        /// </summary>
        protected string DefaultActionName { get; set; }

        /// <summary>
        /// If true then controller name will be excluded from url
        /// for actions from default controller that are different from default action.
        ///
        /// Setting that property to tru can cause problem if you have another controller or area
        /// with the same name as non-default action of default controller.
        ///
        /// Set this property to true if action names for default Controller
        /// is different from another controllers' and areas' names.
        ///
        /// Compare behaviour:
        ///
        ///     true                             false
        ///     -------                          -------
        ///     Controller: Default              Controller: Default
        ///     Action: Index                    Action: Index
        ///     Url:                             Url:
        ///     -------                          -------
        ///     Controller: Default              Controller: Default
        ///     Action: Photo                    Action: Photo
        ///    *Url: Photo                       Url: Default/Photo
        ///
        /// Setting the property to true will cause problem if you have controller with name Photo:
        ///
        ///     -------                          -------
        ///     Controller: Photo                Controller: Photo
        ///     Action: Index                    Action: Index
        ///    *Url: Photo                       Url: Photo
        ///
        /// As you can see urls are the same for following controller->action
        /// (marked with asterisk in the previous samples):
        ///     Default->Photo
        ///     Photo->Index
        ///
        /// The same situation with area names
        /// </summary>
        protected bool IsActionUnique { get; set; }

        public BaseRoute()
        {
            DefaultControllerName = "Default";
            DefaultActionName = "Index";
            IsActionUnique = true;
        }
        public virtual string GetName()
        {
            return $"Route_{Area}_{Controller}_{Action}";
        }

        public virtual string GetNameLocalized()
        {
            return GetName()+"_Localized";
        }

        public virtual string GetUrl()
        {
            var url = "";

            var c = "";
            var a = "";

            if (!String.IsNullOrEmpty(DefaultActionName) && Action != DefaultActionName)
            {
                a = Action;
            }

            if (String.IsNullOrEmpty(DefaultControllerName) || Controller != DefaultControllerName || (!IsActionUnique && !String.IsNullOrEmpty(a)))
            {
                c = Controller;
            }

            if (!string.IsNullOrEmpty(Area))
                url = Area;

            if (!String.IsNullOrEmpty(c))
            {
                if (!String.IsNullOrEmpty(url))
                    url += '/';
                url += c;
            }

            if (!String.IsNullOrEmpty(a))
            {
                if (!String.IsNullOrEmpty(url))
                    url += '/';
                url += a;
            }

            return url;
        }

        public virtual object GetDefaults()
        {
            return String.IsNullOrEmpty(Area)
                       ? (object) new
                                      {
                                          Controller,
                                          Action
                                      }
                       : new
                             {
                                 Area,
                                 Controller,
                                 Action
                             };
        }

        public virtual object GetConstraints()
        {
            return null;
        }

        public virtual string[] GetNamespaces()
        {
            return null;
        }
        /// <summary>
        /// Override this method when you need to inherit some parameters from provided route.
        /// FollowContext is used when route2 is generated from route1.
        /// For example you need to generate link to Page2 and put it on Page1.
        /// In that case route to Page2 may follow context of route of Page1.
        /// If route to Page1 includes locale information ru-RU, most likely you will want
        /// to include the same locale when you generate link to Page2.
        /// </summary>
        /// <param name="r"></param>
        public virtual void FollowContext(IBaseRoute r)
        {
        }
        /// <summary>
        /// This method is used by extension methods to create the clone of current route
        /// and delegate to modify some parameters of clone.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual T Clone<T>(Action<T> action) where T : IBaseRoute
        {
            var type = GetType();
            var t = (T) type.Assembly.CreateInstance(type.FullName);
            if (t != null)
            {
                t.MakeTheSameAs(this);
                if (action != null)
                    action.Invoke(t);
            }
            return t;
        }

        public IBaseRoute Clone()
        {
            var type = GetType();
            var t = (IBaseRoute)type.Assembly.CreateInstance(type.FullName);
            if (t != null)
            {
                t.MakeTheSameAs(this);
            }
            return t;
        }

        public IBaseRoute GetParentRoute()
        {
            return _parentRoute;
        }

        public void SetParentRoute(IBaseRoute parentRoute)
        {
            _parentRoute = parentRoute;
        }

        /// <summary>
        /// Override this method to properly make current route the same as provided route.
        /// </summary>
        /// <param name="r"></param>
        public virtual void MakeTheSameAs(IBaseRoute r)
        {
            SetAnchor(r.GetAnchor());

            if (r is IPageableRoute o2 && this is IPageableRoute o1)
            {
                o1.Page = o2.Page;
                o1.PageSize = o2.PageSize;
            }
        }
        public virtual string GetDomain()
        {
            return _domain;
        }
        public void SetDomain(string domain)
        {
            _domain = domain;
        }
        public UrlProtocol GetProtocol()
        {
            return _protocol;
        }
        public void SetProtocol(UrlProtocol protocol)
        {
            _protocol = protocol;
        }
        public int GetPort()
        {
            return _port;
        }
        public void SetPort(int  port)
        {
            _port = port;
        }

        public IBaseRoute GetRootRoute()
        {
            IBaseRoute r = this;
            while (r.GetParentRoute() != null)
            {
                r = r.GetParentRoute();
            }
            return r;
        }
    }
}