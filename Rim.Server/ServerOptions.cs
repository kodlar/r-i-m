namespace Rim.Server
{
    public class ServerOptions
    {
        public int Port { get; set; }

        public int PingInterval { get; set; }

        public long MaximumRequestLength { get; set; }
        public int MaximumPendingConnections { get; set; }

        public bool UseSecureConnection { get; set; }
        public string CertificateFilename { get; set; }

        public bool DebugErrors { get; set; }
    }
}
