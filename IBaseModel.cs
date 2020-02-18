namespace Zavand.MvcMananaCore
{
    public interface IBaseModel<out TRoute>
        where TRoute : IBaseRoute
    {
        TRoute GetRoute();
    }
}