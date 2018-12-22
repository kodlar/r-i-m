namespace Rim.Server.Http
{
    public interface IHttpRequestHandler
    {
        void Request(HttpServer server, HttpRequest request, HttpResponse response);

    }
}
