using System.Net.Sockets;

namespace Rim.Server.WebSockets
{
    public interface IClientFactory
    {
        WebSocketClient Create(HttpServer server, HttpRequest request, TcpClient client);

    }
}
