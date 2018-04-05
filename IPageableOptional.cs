namespace Zavand.MvcMananaCore
{
    public interface IPageableOptional
    {
        int? Page { get; set; }
        int? PageSize { get; set; }
    }
}