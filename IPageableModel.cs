namespace Zavand.MvcMananaCore
{
    public interface IPageableModel : IPageable
    {
        ulong Total { get; set; }
    }
}