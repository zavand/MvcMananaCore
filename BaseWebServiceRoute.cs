namespace Zavand.MvcMananaCore
{
    public class BaseWebServiceRoute:BaseRoute
    {
        public const string Prefix = "ws";

        public BaseWebServiceRoute()
        {
        }
        public override string GetUrl()
        {
            return $"{Prefix}/"+ base.GetUrl();
        }
    }
}