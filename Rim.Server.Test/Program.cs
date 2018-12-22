using Rim.Server.WebSockets;
using System;
using System.Net.Sockets;

namespace Rim.Server.Test
{

    class Program
    {
        public static DemoClient Client { get; set; }

        static void Main(string[] args)
        {
            ServerOptions options = new ServerOptions
            {
                Port = 80,
                MaximumPendingConnections = 1000,
                MaximumRequestLength = 1024 * 1024,
                PingInterval = 60000,
                UseSecureConnection = false,
                CertificateFilename = null,
                DebugErrors = false
            };
            
            HttpServer server = new HttpServer(new ClientFactory(), options);
            server.RequestHandler = new WebController();
            server.Start(true);
            while(true)
            {
                string input = Console.ReadLine();
                if (Client != null)
                    Client.Send(input);
            }

        }
    }
}
