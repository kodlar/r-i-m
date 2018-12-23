namespace Rim.Server.Http
{
    /// <summary>
    /// Handles all non-websocket Http Requests
    /// </summary>
    public interface IHttpRequestHandler
    {

        /// <summary>
        /// Triggered when a non-websocket request available.
        /// </summary>
        void Request(HttpServer server, HttpRequest request, HttpResponse response);

    }
}
