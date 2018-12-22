using System.Collections.Generic;

namespace Rim.Server
{
    public class HttpRequest
    {
        public string Method { get; internal set; }
        public string Host { get; internal set; }
        public string Path { get; internal set; }
        public bool IsSecureConnection { get; internal set; }
        
        public string IpAddress { get; internal set; }

        public bool IsWebSocket { get; set; }
        public string WebSocketKey { get; set; }

        public string AcceptEncoding { get; set; }

        public string Content { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}
