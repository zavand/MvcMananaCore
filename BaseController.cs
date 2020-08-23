using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Zavand.MvcMananaCore
{
    public class BaseController: Microsoft.AspNetCore.Mvc.Controller
    {
        public TModel GetModel<TModel, TRoute, TController>(TRoute r, TController c)
            where TModel : BaseModel<TRoute, TController>, new()
            where TRoute : IBaseRoute
            where TController : BaseController
        {
            var m = new TModel();
            m.SetupModel(c, r);
            return m;
        }

        public async Task<TModel> GetModelAsync<TModel, TRoute, TController>(TRoute r, TController c)
            where TModel : BaseModel<TRoute, TController>, new()
            where TRoute : IBaseRoute
            where TController : BaseController
        {
            var m = new TModel();
            await m.SetupModelAsync(c, r);
            return m;
        }

        public void SetupModel<TModel, TRoute, TController>(TModel m, TRoute r, TController c)
            where TModel : BaseModel<TRoute, TController>, new()
            where TRoute : IBaseRoute
            where TController : BaseController
        {
            m.SetupModel(c, r);
        }

        public virtual IActionResult RedirectToAction<TRoute>(IBaseRoute currentRoute, object extraParams = null, Action<TRoute> action = null)
            where TRoute : IBaseRoute, new()
        {
            var r = new TRoute();
            return RedirectToAction(currentRoute, r, extraParams, q => action?.Invoke((TRoute)q));
        }

        public virtual IActionResult RedirectToAction(IBaseRoute currentRoute, IBaseRoute r, object extraParams = null, Action<IBaseRoute> action = null)
        {
            var url = Url.RouteUrl(currentRoute, r, extraParams, action);
            if (!String.IsNullOrEmpty(url))
                return Redirect(url);
            return Redirect("~/");
        }
    }
}