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
    }
}
