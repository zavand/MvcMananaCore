using System;
using System.Collections.Generic;

namespace Zavand.MvcMananaCore
{
    public interface IBaseRoute
    {
        string Locale { get; set; }
        string Controller { get; set; }
        string Action { get; set; }
        string Area { get; set; }
        string GetName();
        string GetNameLocalized();
        string GetUrl();
        string GetUrlLocalized();
        object GetDefaults();
        object GetConstraints();
        object GetConstraintsLocalized(string[] locales = null);
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

        /// <summary>
        /// Get localized URL for specific locale
        /// In example example below two urls point to the same action but looks very different based on used locale:
        /// /en/my-account
        /// /ru/моя-учётная-запись
        /// /рус/моя-учётная-запись
        /// /русский/моя-учётная-запись
        /// </summary>
        /// <returns></returns>
        Dictionary<String,string> GetLocalizedUrlPerLocale();
    }
}