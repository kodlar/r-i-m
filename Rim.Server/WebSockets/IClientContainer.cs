using System.Collections.Generic;

namespace Rim.Server.WebSockets
{

    public interface IClientContainer
    {

        int Count();

        void Add(WebSocketClient client);

        void Remove(WebSocketClient client);

        IEnumerable<WebSocketClient> List();

    }

}
