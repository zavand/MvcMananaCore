using System;

namespace Zavand.MvcMananaCore
{
    public class BaseModel<TRoute, TController> : IBaseModel<TRoute>
        where TRoute : BaseRoute
        where TController : BaseController
    {
        private TController Controller { get; set; }
        private TRoute Route { get; set; }
        //        public string Locale { get; set; }

        public virtual void SetupModel(TController controller, TRoute route)
        {
            Route = route;
            Controller = controller;

            var currentControllerContext = controller.ControllerContext;
            IBaseRoute currentRoute = route;
//            while (currentControllerContext.ParentActionViewContext != null)
//            {
//                var m = currentControllerContext.ParentActionViewContext?.Controller?.ViewData?.Model as IBaseModel;
//                if (m == null)
//                    break;
//                var parentRoute = m.GetBaseRoute();
//                currentRoute.SetParentRoute(parentRoute);
//                currentRoute.Locale = parentRoute.Locale;
//                currentRoute = parentRoute;
//                currentControllerContext = currentControllerContext.ParentActionViewContext;
//            }
        }

        //        protected virtual void SetLocale(string locale)
        //        {
        //            if (string.IsNullOrEmpty(locale))
        //            {
        //                Locale = GetDefaultLocale();
        //
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    var ci = new CultureInfo(locale);
        //                    Locale = ci.Name;
        //                }
        //                catch
        //                {
        //                    Locale = GetDefaultLocale();
        //                }
        //            }
        //            var c = GetCultureInfo();
        //            Thread.CurrentThread.CurrentCulture = new CultureInfo(c.LCID);
        //            Thread.CurrentThread.CurrentUICulture = new CultureInfo(c.LCID);
        //        }

        //        protected virtual string GetDefaultLocale()
        //        {
        //            return "en-US";
        //        }

        //        public CultureInfo GetCultureInfo()
        //        {
        //            try
        //            {
        //                return new CultureInfo(Locale);
        //            }
        //            // ReSharper disable EmptyGeneralCatchClause
        //            catch
        //            // ReSharper restore EmptyGeneralCatchClause
        //            {
        //            }
        //            var dci = new CultureInfo(GetDefaultLocale());
        //            return dci;
        //        }

        public TController GetController()
        {
            return Controller;
        }

        public TRoute GetRoute()
        {
            return Route;
        }

        //        public int GetLCID()
        //        {
        //            return GetCultureInfo().LCID;
        //        }

        public virtual bool IsValid()
        {
            return
                Controller != null &&
                Controller.ModelState != null &&
                Controller.ModelState.IsValid;
        }
    }
}