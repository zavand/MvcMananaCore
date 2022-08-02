namespace Zavand.MvcMananaCore
{
    public interface IPageableModel<out TRoute> : IPageable, IBaseModel<TRoute>
        where TRoute : IBaseRoute
    {
        ulong Total { get; set; }
    }

    // public static class PageableModelExtensions
    // {
    //     public static void CopyTo<TRoute>(this IPageableModel<TRoute> mFrom, IPageableModel<TRoute> mTo) where TRoute : IBaseRoute
    //     {
    //         if (mFrom == null || mTo == null)
    //             return;
    //
    //         // ((IBaseModel<TRoute>) mFrom).CopyTo(mTo);
    //         ((IPageable) mFrom).CopyTo(mTo);
    //
    //         mTo.Total = mFrom.Total;
    //     }
    // }
}