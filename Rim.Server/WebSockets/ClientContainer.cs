using System.Collections.Generic;

namespace Rim.Server.WebSockets
{
    public class ClientContainer : IClientContainer
    {
        private List<ServerSocket> Clients { get; set; }

        public ClientContainer()
        {
            Clients = new List<ServerSocket>();
        }

        public void Add(ServerSocket client)
        {
            lock (Clients)
                Clients.Add(client);
        }

        public int Count()
        {
            return Clients.Count;
        }

        public IEnumerable<ServerSocket> List()
        {
            List<ServerSocket> clients = new List<ServerSocket>();

            lock (Clients)
                foreach (ServerSocket client in Clients)
                    clients.Add(client);

            return clients;
        }

        public void Remove(ServerSocket client)
        {
            lock (Clients)
                Clients.Remove(client);
        }

    }
}
