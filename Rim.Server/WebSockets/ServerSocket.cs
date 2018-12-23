using Rim.Server.WebSockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Rim.Server
{
    public abstract class ServerSocket : SocketBase
    {

        #region Events - Properties
        
        public HttpServer Server { get; private set; }
        public HttpRequest Request { get; private set; }

        private Timer PingTimer { get; set; }

        #endregion

        protected ServerSocket(HttpServer server, HttpRequest request, TcpClient client)
        {
            Server = server;
            Request = request;
            Client = client;
            IsConnected = true;

            Stream networkStream = client.GetStream();

            if (server.Options.UseSecureConnection)
                Stream = new SslStream(networkStream, true);
            else
                Stream = networkStream;

        }
        
        internal void Start()
        {
            if (Server.Container != null)
                Server.Container.Add(this);

            if (Server.Options.PingInterval > 0)
            {
                PingTimer = new Timer(Server.Options.PingInterval);
                PingTimer.AutoReset = true;
                PingTimer.Elapsed += (sender, e) =>
                {
                    if (IsConnected)
                        Ping();
                };

                PingTimer.Start();
            }

            IsConnected = true;
            OnConnected();

            Read();
        }
        
        public void Ping()
        {
            try
            {
                byte[] data = PackageWriter.CreatePing();
                Stream.Write(data, 0, data.Length);
            }
            catch
            {
                Disconnect();
            }
        }

        public override void Disconnect()
        {
            base.Disconnect();

            try
            {
                if (PingTimer != null)
                {
                    PingTimer.Stop();
                    PingTimer.Dispose();
                }
            }
            catch { }

            if (Server.Container != null)
                Server.Container.Remove(this);
        }

    }
}
