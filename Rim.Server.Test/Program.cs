using Rim.Server.Http;
using Rim.Server.WebSockets;
using System;
using System.Net.Sockets;

namespace Rim.Server.Test
{

    public class ReqHandler : IHttpRequestHandler
    {
        public void Request(HttpServer server, HttpRequest request, HttpResponse response)
        {
            response.SetToJson(new
            {
                A = 4
            });
        }
    }

    class Program
    {
        public static DemoClient Client { get; set; }

        static void Main(string[] args)
        {
            ServerOptions options = new ServerOptions
            {
                Port = 83,
                MaximumPendingConnections = 1000,
                MaximumRequestLength = 1024 * 1024,
                PingInterval = 60000,
                UseSecureConnection = false,
                CertificateFilename = null,
                DebugErrors = false
            };
            
            HttpServer server = new HttpServer(new ReqHandler(), new ClientFactory(), null, null, options);
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
