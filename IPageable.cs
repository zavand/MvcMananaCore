namespace Zavand.MvcMananaCore
{
    public interface IPageable
    {
        int Page { get; set; }
        int PageSize { get; set; }
    }

    public static class PageableExtensions
    {
        public static void CopyTo(this IPageable mFrom, IPageable mTo)
        {
            if (mFrom == null || mTo == null)
                return;

            mTo.Page = mFrom.Page;
            mTo.PageSize = mFrom.PageSize;
        }
    }
}