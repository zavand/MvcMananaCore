using Newtonsoft.Json;

namespace Zavand.MvcMananaCore
{
    public class BaseWebServiceController : BaseController
    {
        protected JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
    }
}