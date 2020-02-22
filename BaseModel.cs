using System.Globalization;
using System.Threading;

namespace Zavand.MvcMananaCore
{
    public class BaseModel<TRoute, TController> : IBaseModel<TRoute>
        where TRoute : IBaseRoute
        where TController : BaseController
    {
        private TController Controller { get; set; }
        private TRoute Route { get; set; }
        public CultureInfo SavedCurrentCultureBeforeAsyncAwait { get; set; }

        public virtual void SetupModel(TController controller, TRoute route)
        {
            Route = route;
            Controller = controller;
        }

        public TController GetController()
        {
            return Controller;
        }

        public TRoute GetRoute()
        {
            return Route;
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
        public void RestoreCultureAfterAsyncAwait()
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = SavedCurrentCultureBeforeAsyncAwait;
        }
    }
}