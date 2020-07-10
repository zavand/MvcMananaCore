namespace Zavand.MvcMananaCore
{
    public interface IPageableModel<out TRoute> : IPageable, IBaseModel<TRoute>
        where TRoute : IBaseRoute
    {
        ulong Total { get; set; }
    }
}