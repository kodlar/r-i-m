using Rim.Server.WebSockets;
using System;

namespace Rim.Client.Test
{
    public class DemoClient : ClientSocket
    {
        protected override void OnBinaryReceived(byte[] payload)
        {
        }

        protected override void OnConnected()
        {
            Console.WriteLine("client is connected");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine("client is disconnected");
        }

        protected override void OnMessageReceived(string message)
        {
            Console.WriteLine("> " + message);
        }
    }
}
