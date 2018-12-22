using Rim.Server.Http;

namespace Rim.Server.Test
{
    public class WebController : IHttpRequestHandler
    {

        public void Request(HttpServer server, HttpRequest request, HttpResponse response)
        {
            response.SetToHtml();
            response.Write("Hello World!");
        }

    }
}
