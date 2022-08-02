using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace Zavand.MvcMananaCore
{
    public interface IBaseModel<out TRoute>
        where TRoute : IBaseRoute
    {
        TRoute GetRoute();
        IUrlHelper GetUrlHelper();
        CultureInfo Culture { get; set; }
        /// <summary>
        /// Call this method inside view and layout to restore CultureInfo and make localization work.
        /// async/await switches culture to default en-US
        /// Some details here:
        /// https://stackoverflow.com/questions/30662668/keep-currentculture-in-async-await
        /// </summary>
        void RestoreCulture();

        // TODO: think about TRoute as a type for route parameter
        void SetRoute(object route);
        void SetUrlHelper(IUrlHelper urlHelper);
    }

    // public static class BaseModelExtensions
    // {
    //     public static void CopyTo<TRoute>(this IBaseModel<TRoute> mFrom, IBaseModel<TRoute> mTo) where TRoute : IBaseRoute
    //     {
    //         if (mFrom == null || mTo == null)
    //             return;
    //
    //         mTo.Culture = mFrom.Culture;
    //         mTo.SetRoute(mFrom.GetRoute());
    //         mTo.SetUrlHelper(mFrom.GetUrlHelper());
    //     }
    // }
}
