﻿namespace Zavand.MvcMananaCore
{
    public interface IPageable
    {
        int Page { get; set; }
        int PageSize { get; set; }
    }
}