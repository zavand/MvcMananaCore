namespace Zavand.MvcMananaCore
{
    public interface IBaseModel<out TRoute>
        where TRoute : BaseRoute
    {
        TRoute GetRoute();
    }
}