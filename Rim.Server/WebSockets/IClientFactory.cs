using System.Net.Sockets;

namespace Rim.Server.WebSockets
{
    public interface IClientFactory
    {
        ServerSocket Create(HttpServer server, HttpRequest request, TcpClient client);

    }
}
