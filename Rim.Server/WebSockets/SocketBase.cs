using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Rim.Server.WebSockets
{
    public abstract class SocketBase
    {

        #region Properties

        internal TcpClient Client { get; set; }

        internal Stream Stream { get; set; }

        public bool IsConnected { get; protected set; }

        #endregion

        #region Methods

        protected void Read()
        {
            byte[] buffer = new byte[512];

            PackageReader reader = new PackageReader();

            try
            {
                while (IsConnected)
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
                Disconnect();
            }
        }

        public void Send(byte[] preparedData)
        {
            try
            {
                Stream.Write(preparedData, 0, preparedData.Length);
            }
            catch (Exception ex)
            {
                Disconnect();
            }
        }
        
        public virtual void Disconnect()
        {
            bool warn = IsConnected;

            try
            {
                IsConnected = false;
                Stream.Dispose();
                Client.Close();
                Client = null;
                Stream = null;
            }
            catch { }

            OnDisconnected();
        }

        #endregion

        #region Abstract Methods

        protected abstract void OnConnected();

        protected abstract void OnDisconnected();

        protected abstract void OnMessageReceived(string message);

        protected abstract void OnBinaryReceived(byte[] payload);

        #endregion


    }
}
