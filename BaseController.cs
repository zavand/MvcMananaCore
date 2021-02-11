using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Zavand.MvcMananaCore
{
    // public interface IBaseController
    // {
    //     TModel GetModel<TModel, TRoute, TController>(TRoute r, TController c)
    //         where TModel : IBaseModel<TRoute>, new()
    //         where TRoute : IBaseRoute
    //         where TController : IBaseController;
    //
    //     Task<TModel> GetModelAsync<TModel, TRoute, TController>(TRoute r, TController c)
    //         where TModel : IBaseModel<TRoute>, new()
    //         where TRoute : IBaseRoute
    //         where TController : IBaseController;
    // }

    public class BaseController: Microsoft.AspNetCore.Mvc.Controller//, IBaseController
    {
        public virtual Task<TModel> PrepareModelBaseAsync<TModel, TRoute>(TModel m, TRoute r)
            where TModel : IBaseModel<TRoute>
            where TRoute : IBaseRoute
        {
            m.SetRoute(r);
            r.SetQueryParams(HttpContext.Request.Query);
            return Task.FromResult(m);
        }

        public virtual IActionResult RedirectToAction<TRoute>(IBaseRoute currentRoute, object extraParams = null, Action<TRoute> action = null, bool skipFollowContext = false)
            where TRoute : IBaseRoute, new()
        {
            var r = new TRoute();
            return RedirectToAction(currentRoute, r, extraParams, q => action?.Invoke((TRoute)q), skipFollowContext);
        }

        public virtual IActionResult RedirectToAction(IBaseRoute currentRoute, IBaseRoute r, object extraParams = null, Action<IBaseRoute> action = null, bool skipFollowContext = false)
        {
            var url = Url.RouteUrl(currentRoute, r, extraParams, action, skipFollowContext);
            if (!String.IsNullOrEmpty(url))
                return Redirect(TransformRedirectUrl(url));
            return Redirect(TransformRedirectUrl("~/"));
        }

        public virtual IActionResult RedirectToAction<TRoute>(IBaseRoute currentRoute, TRoute r, object extraParams = null, Action<TRoute> action = null, bool skipFollowContext = false)
            where TRoute : IBaseRoute, new()
        {
            var url = Url.RouteUrl(currentRoute, r, extraParams, action, skipFollowContext);
            if (!String.IsNullOrEmpty(url))
                return Redirect(TransformRedirectUrl(url));
            return Redirect(TransformRedirectUrl("~/"));
        }

        protected virtual string TransformRedirectUrl(string url)
        {
            return url;
        }
    }
}