namespace Zavand.MvcMananaCore
{
    public interface IPageableModel<out TRoute> : IPageable, IBaseModel<TRoute>
        where TRoute : BaseRoute
    {
        ulong Total { get; set; }
    }
}