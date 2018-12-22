using System.Collections.Generic;

namespace Rim.Server.WebSockets
{
    public class ClientContainer : IClientContainer
    {
        private List<WebSocketClient> Clients { get; set; }

        public ClientContainer()
        {
            Clients = new List<WebSocketClient>();
        }

        public void Add(WebSocketClient client)
        {
            lock (Clients)
                Clients.Add(client);
        }

        public int Count()
        {
            return Clients.Count;
        }

        public IEnumerable<WebSocketClient> List()
        {
            List<WebSocketClient> clients = new List<WebSocketClient>();

            lock (Clients)
                foreach (WebSocketClient client in Clients)
                    clients.Add(client);

            return clients;
        }

        public void Remove(WebSocketClient client)
        {
            lock (Clients)
                Clients.Remove(client);
        }

    }
}
