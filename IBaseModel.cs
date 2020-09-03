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
    }
}
