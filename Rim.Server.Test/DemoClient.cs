using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Rim.Server.Test
{
    public class DemoClient : ServerSocket
    {
        public DemoClient(HttpServer server, HttpRequest request, TcpClient client) : base(server, request, client)
        {
        }

        protected override void OnBinaryReceived(byte[] payload)
        {
        }

        protected override void OnConnected()
        {
            Program.Client = this;
            Console.WriteLine("connected");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine("disconnected");
        }

        protected override void OnMessageReceived(string message)
        {
            Console.WriteLine("# " + message);
        }
    }

}
