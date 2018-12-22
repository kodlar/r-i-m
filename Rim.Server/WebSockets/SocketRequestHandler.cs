using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Rim.Server.WebSockets
{
    internal static class SocketRequestHandler
    {

        internal static ServerSocket HandshakeClient(HttpServer server, HttpRequest request, TcpClient tcp)
        {
            if (!request.IsWebSocket)
                return null;

            byte[] response = BuildResponse(request.WebSocketKey);

            Stream stream = tcp.GetStream();
            if (server.Options.UseSecureConnection)
                stream = new SslStream(stream, true);

            stream.Write(response, 0, response.Length);

            ServerSocket client = server.ClientFactory.Create(server, request, tcp);
            return client;
        }

        private static byte[] BuildResponse(string websocketKey)
        {
            byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
                                                     + "Connection: Upgrade" + Environment.NewLine
                                                     + "Upgrade: websocket" + Environment.NewLine
                                                     + "Sec-WebSocket-Accept: " + CreateWebSocketGuid(websocketKey) + Environment.NewLine
                                                     + Environment.NewLine);

            return response;
        }

        private static string CreateWebSocketGuid(string key)
        {
            byte[] keybytes = Encoding.UTF8.GetBytes(key + HttpServer.WEBSOCKET_GUID);
            return Convert.ToBase64String(SHA1.Create().ComputeHash(keybytes));
        }

    }
}
