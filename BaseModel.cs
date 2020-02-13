namespace Zavand.MvcMananaCore
{
    public class BaseModel<TRoute, TController> : IBaseModel<TRoute>
        where TRoute : BaseRoute
        where TController : BaseController
    {
        private TController Controller { get; set; }
        private TRoute Route { get; set; }

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
    }
}