using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Zavand.MvcMananaCore
{
    public class BaseModel<TRoute, TController> : IBaseModel<TRoute>
        where TRoute : IBaseRoute
        where TController : BaseController
    {
        private TController Controller { get; set; }
        private TRoute Route { get; set; }
        public CultureInfo Culture { get; set; }

        protected BaseModel()
        {
            Culture = new CultureInfo("en-US");
        }

        public virtual void SetupModel(TController controller, TRoute route)
        {
            SetupModelAsync(controller, route).Wait();
        }

        public virtual Task SetupModelAsync(TController controller, TRoute route)
        {
            Route = route;
            Controller = controller;
            return Task.CompletedTask;
        }

        public TController GetController()
        {
            return Controller;
        }

        public void SetController(TController c)
        {
            Controller = c;
        }

        public TRoute GetRoute()
        {
            return Route;
        }

        public virtual IUrlHelper GetUrlHelper()
        {
            return null;
        }

        public virtual bool IsValid()
        {
            return
                Controller != null &&
                Controller.ModelState != null &&
                Controller.ModelState.IsValid;
        }

        /// <summary>
        /// Call this method inside view and layout to restore CultureInfo and make localization work.
        /// async/await switches culture to default en-US
        /// Some details here:
        /// https://stackoverflow.com/questions/30662668/keep-currentculture-in-async-await
        /// </summary>
        public void RestoreCulture()
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = Culture;
        }

        public void SetRoute(object route)
        {
            Route = (TRoute) route;
        }

        public virtual void SetUrlHelper(IUrlHelper urlHelper)
        {

        }
    }
}
