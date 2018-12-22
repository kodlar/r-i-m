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
    public abstract class WebSocketClient
    {

        #region Events - Properties
        
        public HttpServer Server { get; private set; }
        public HttpRequest Request { get; private set; }

        internal TcpClient Client { get; private set; }
        internal Stream Stream { get; private set; }

        public bool Connected { get; private set; }

        private Timer PingTimer { get; set; }

        #endregion

        protected WebSocketClient(HttpServer server, HttpRequest request, TcpClient client)
        {
            Server = server;
            Request = request;
            Client = client;
            Connected = true;

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
                    if (Connected)
                        Ping();
                };

                PingTimer.Start();
            }

            Connected = true;
            OnConnected();

            Read();
        }
        
        private void Read()
        {
            byte[] buffer = new byte[512];

            PackageReader reader = new PackageReader();

            try
            {
                while (Connected)
                {
                    int read = Stream.Read(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        Disconnect();
                        return;
                    }

                    int offset = 0;
                    while (offset < read)
                    {
                        offset += reader.Read(buffer, offset, read);

                        if (reader.IsReady)
                        {
                            switch (reader.OpCode)
                            {
                                case SocketOpCode.UTF8:
                                    string result = Encoding.UTF8.GetString(reader.Payload);
                                    Task.Factory.StartNew(() =>
                                    {
                                        OnMessageReceived(result);
                                    });
                                    break;

                                case SocketOpCode.Binary:
                                    byte[] payload = reader.Payload;
                                    Task.Factory.StartNew(() =>
                                    {
                                        OnBinaryReceived(payload);
                                    });
                                    break;

                                case SocketOpCode.Terminate:
                                    Disconnect();
                                    return;

                            }
                            reader = new PackageReader();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Server.Logger.Log(ex);
                Disconnect();
            }

        }

        public virtual void Send(string message)
        {
            try
            {
                byte[] data = PackageWriter.CreateUTF8(message);
                Stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Server.Logger.Log(ex);
                Disconnect();
            }
        }

        public void Send(byte[] preparedData)
        {
            try
            {
                Stream.Write(preparedData, 0, preparedData.Length);
            }
            catch(Exception ex)
            {
                Server.Logger.Log(ex);
                Disconnect();
            }
        }
        
        public void Ping()
        {
            try
            {
                byte[] data = PackageWriter.CreatePing();
                Stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Server.Logger.Log(ex);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            bool warn = Connected;
            try
            {
                if (PingTimer != null)
                {
                    PingTimer.Stop();
                    PingTimer.Dispose();
                }
            }
            catch { }

            try
            {
                Connected = false;
                Stream.Dispose();
                Client.Close();
                Client = null;
                Stream = null;
            }
            catch { }

            OnDisconnected();

            if (Server.Container != null)
                Server.Container.Remove(this);

        }

        #region Abstract Methods

        public abstract void OnConnected();
        public abstract void OnDisconnected();
        public abstract void OnMessageReceived(string message);
        public abstract void OnBinaryReceived(byte[] payload);

        #endregion

    }
}
