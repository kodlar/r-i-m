namespace Rim.Server
{
    /// <summary>
    /// HttpServer options.
    /// This object is loaded from "rimserver.json", "rim.json" or "server.json" file
    /// Or It can be passed as parameter to the HttpServer constructor method
    /// </summary>
    public class ServerOptions
    {
        /// <summary>
        /// Server Listening port number
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Ping Interval in milliseconds to active websocket connections.
        /// </summary>
        public int PingInterval { get; set; }

        /// <summary>
        /// After each TCP Client is accepted, the first data (this is the first HTTP Request) length
        /// </summary>
        public long MaximumRequestLength { get; set; }

        /// <summary>
        /// For TcpListener objects, maximum pending connections waiting for being accepted by the server.
        /// If a client behind the maximum pending connections, it will be rejected immediately.
        /// </summary>
        public int MaximumPendingConnections { get; set; }

        /// <summary>
        /// If true, server loads the certificate file and accepts connections with SSL
        /// </summary>
        public bool UseSecureConnection { get; set; }

        /// <summary>
        /// Certificate filename
        /// </summary>
        public string CertificateFilename { get; set; }

        /// <summary>
        /// If true, ErrorLogger runs and log all connection logs
        /// </summary>
        public bool DebugErrors { get; set; }

    }
}
