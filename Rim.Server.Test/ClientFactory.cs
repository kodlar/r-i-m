using Rim.Server.WebSockets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Rim.Server.Test
{
    public class ClientFactory : IClientFactory
    {
        public ServerSocket Create(HttpServer server, HttpRequest request, TcpClient client)
        {
            return new DemoClient(server, request, client);
        }
    }
}
