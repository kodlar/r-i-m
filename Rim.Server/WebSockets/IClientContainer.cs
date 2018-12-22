using System.Collections.Generic;

namespace Rim.Server.WebSockets
{

    public interface IClientContainer
    {

        int Count();

        void Add(ServerSocket client);

        void Remove(ServerSocket client);

        IEnumerable<ServerSocket> List();

    }

}
