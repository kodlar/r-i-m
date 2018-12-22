using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Rim.Server.Test
{
    public class DemoClient : WebSocketClient
    {
        public DemoClient(HttpServer server, HttpRequest request, TcpClient client) : base(server, request, client)
        {
        }

        public override void OnBinaryReceived(byte[] payload)
        {
        }

        public override void OnConnected()
        {
            Program.Client = this;
            Console.WriteLine("connected");
        }

        public override void OnDisconnected()
        {
            Console.WriteLine("disconnected");
        }

        public override void OnMessageReceived(string message)
        {
            Console.WriteLine("# " + message);
        }
    }

}
