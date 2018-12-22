using Rim.Server.WebSockets;
using System;

namespace Rim.Client.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Second();
        }

        static void First()
        {
            DemoClient client = new DemoClient();
            client.Connect("ws://127.0.0.1");

            while (true)
            {
                string line = Console.ReadLine();
                if (client.IsConnected)
                    client.Send(line);
            }
        }

        static void Second()
        {
            ClientSocket socket = new ClientSocket();
            socket.MessageReceived += (client, message) =>
             {
                 Console.WriteLine("m: " + message);
             };

            socket.Connect("ws://127.0.0.1/test");

            while (true)
            {
                string line = Console.ReadLine();
                if (socket.IsConnected)
                    socket.Send(line);
            }
        }

    }
}
