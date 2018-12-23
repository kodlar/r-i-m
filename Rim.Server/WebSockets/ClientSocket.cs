using Rim.Server.Http;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rim.Server.WebSockets
{

    public delegate void ClientStatusHandler(ClientSocket client);
    public delegate void ClientMessageHandler(ClientSocket client, string message);
    public delegate void ClientBinaryHandler(ClientSocket client, byte[] payload);

    public class ClientSocket : SocketBase
    {

        public event ClientStatusHandler Connected;
        public event ClientStatusHandler Disconnected;
        public event ClientMessageHandler MessageReceived;
        public event ClientBinaryHandler BinaryReceived;

        public string WebSocketKey { get; private set; }
        
        public bool Connect(string uri)
        {
            DnsResolver resolver = new DnsResolver();
            DnsInfo info = resolver.Resolve(uri);

            return Connect(info);
        }

        public bool Connect(string ip, int port, bool secure)
        {
            DnsInfo info = new DnsInfo
            {
                IPAddress = ip,
                Hostname = ip,
                Port = port,
                Path = "/",
                Protocol = Protocol.WebSocket,
                SSL = secure
            };

            return Connect(info);
        }

        public bool Connect(DnsInfo dns)
        {
            try
            {
                Client = new TcpClient();
                Client.Connect(dns.IPAddress, dns.Port);
                IsConnected = true;

                if (dns.SSL)
                    Stream = new SslStream(Client.GetStream(), true);
                else
                    Stream = Client.GetStream();

                byte[] request = CreateRequest(dns);
                Stream.Write(request, 0, request.Length);

                byte[] buffer = new byte[8192];
                int len = Stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, len);
                string first = response.Substring(0, 50);
                if (!first.Contains(" 101 "))
                {
                    Disconnect();
                    throw new NotSupportedException("Server doesn't support web socket protocol");
                }

                RequestReader reader = new RequestReader();
                HttpRequest requestResponse = reader.Read(response);

                if (!requestResponse.Headers.ContainsKey("Sec-WebSocket-Accept"))
                    throw new InvalidOperationException("Handshaking error, server didn't response Sec-WebSocket-Accept");

                string rkey = requestResponse.Headers["Sec-WebSocket-Accept"];

                using (SHA1 sha1 = SHA1.Create())
                {
                    byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(WebSocketKey + HttpServer.WEBSOCKET_GUID));
                    string fkey = Convert.ToBase64String(hash);
                    if (rkey != fkey)
                        throw new InvalidOperationException("Handshaking error, Invalid Key");
                }

                OnConnected();
                Task.Factory.StartNew(Read);
                return true;
            }
            catch
            {
                Disconnect();
                return false;
            }
        }

        private byte[] CreateRequest(DnsInfo dns)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(Guid.NewGuid().ToByteArray());
                WebSocketKey = Convert.ToBase64String(hash);
            }
            
            string request = "GET " + dns.Path + " HTTP/1.1" + Environment.NewLine +
                             "Host: " + dns.Hostname + Environment.NewLine +
                             "Connection: Upgrade" + Environment.NewLine +
                             "Pragma: no-cache" + Environment.NewLine +
                             "Cache-Control: no-cache" + Environment.NewLine +
                             "Upgrade: websocket" + Environment.NewLine +
                             "Sec-WebSocket-Version: 13" + Environment.NewLine +
                             "Accept-Encoding: gzip, deflate, br" + Environment.NewLine +
                             "Accept-Language: en-US,en;q=0.9" + Environment.NewLine +
                             "Sec-WebSocket-Key: " + WebSocketKey + Environment.NewLine +
                             "Sec-WebSocket-Extensions: permessage-deflate; client_max_window_bits" + Environment.NewLine;
            
            request += Environment.NewLine;
            return Encoding.UTF8.GetBytes(request);
        }
        
        protected override void OnBinaryReceived(byte[] payload)
        {
            BinaryReceived?.Invoke(this, payload);
        }

        protected override void OnConnected()
        {
            Connected?.Invoke(this);
        }

        protected override void OnDisconnected()
        {
            Disconnected?.Invoke(this);
        }

        protected override void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }

    }
}
