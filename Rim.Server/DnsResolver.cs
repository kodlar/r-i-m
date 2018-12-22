using System;
using System.Net;

namespace Rim.Server
{
    public class DnsResolver
    {
        public DnsInfo Resolve(string uri)
        {
            DnsInfo info = new DnsInfo();

            int protocol_index = uri.IndexOf("//");
            bool hasPath = uri.LastIndexOf('/') > uri.IndexOf("//") + 2;

            string host_search = "";

            if (protocol_index > 0)
                host_search = uri.Substring(protocol_index + 2);

            if (hasPath)
            {
                info.Path = host_search.Substring(host_search.IndexOf('/'));
                host_search = host_search.Substring(0, host_search.IndexOf('/'));
            }
            else
                info.Path = "/";

            int specified_port = 0;
            int port_index = host_search.IndexOf(':');
            if (port_index > 0)
            {
                specified_port = Convert.ToInt32(host_search.Split(':')[1].Trim());
                host_search = host_search.Substring(0, port_index);
            }

            bool byIP = host_search.Split('.').Length == 4;

            info.Hostname = host_search;

            if (byIP)
                info.IPAddress = host_search;
            else
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(host_search);
                info.IPAddress = hostEntry.AddressList[0].ToString();
            }

            if (protocol_index > 0)
            {
                string protocol = uri.Substring(0, protocol_index).ToLower().Replace(":", "");

                switch (protocol)
                {
                    case "http":
                        info.Protocol = Protocol.Http;
                        info.SSL = false;
                        break;

                    case "https":
                        info.Protocol = Protocol.Http;
                        info.SSL = true;
                        break;

                    case "ws":
                        info.Protocol = Protocol.WebSocket;
                        info.SSL = false;
                        break;

                    case "wss":
                        info.Protocol = Protocol.WebSocket;
                        info.SSL = true;
                        break;
                }
            }

            info.Port = info.SSL ? 443 : 80;

            if (specified_port > 0)
                info.Port = specified_port;

            return info;
        }
    }
}
