using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace Zavand.MvcMananaCore
{
    public class BaseModel<TRoute> : IBaseModel<TRoute>
        where TRoute : IBaseRoute
    {
        private TRoute Route { get; set; }
        public CultureInfo Culture { get; set; }

        protected BaseModel()
        {
            Culture = new CultureInfo("en-US");
        }

        public TRoute GetRoute()
        {
            return Route;
        }

        public virtual IUrlHelper GetUrlHelper()
        {
            return null;
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
