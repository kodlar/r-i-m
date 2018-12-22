namespace Rim.Server
{
    public enum Protocol
    {
        Unknown,
        Http,
        WebSocket
    }

    public class DnsInfo
    {
        public string IPAddress { get; set; }
        public string Hostname { get; set; }
        public bool SSL { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public Protocol Protocol { get; set; } 
    }

}
