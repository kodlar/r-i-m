using System.Collections.Generic;

namespace Rim.Server
{
    /// <summary>
    /// HttpRequest for HttpServer object.
    /// It is used both WebSocket and Http requests
    /// </summary>
    public class HttpRequest
    {
        /// <summary>
        /// Http Method name as uppercase (ex: GET, POST, PUT)
        /// </summary>
        public string Method { get; internal set; }

        /// <summary>
        /// Requested host name
        /// </summary>
        public string Host { get; internal set; }

        /// <summary>
        /// Request path. Does not include protocol or hostname. Starts with "/"
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// If true, request is secure (with SSL)
        /// </summary>
        public bool IsSecureConnection { get; internal set; }
        
        /// <summary>
        /// Requester client IP Address
        /// </summary>
        public string IpAddress { get; internal set; }

        /// <summary>
        /// If true, request has web socket key header and it's websocket request
        /// </summary>
        public bool IsWebSocket { get; set; }

        /// <summary>
        /// For web socket requests, includes key for websocket client. Otherwise it's null
        /// </summary>
        public string WebSocketKey { get; set; }

        /// <summary>
        /// Client's accepted encodings.
        /// If this propert includes "gzip" server response will be gzipped.
        /// Otherwise server will response as plain text with content-length.
        /// Rim server does not support truncated length format
        /// </summary>
        public string AcceptEncoding { get; set; }

        /// <summary>
        /// Request content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// All other headers as key and value
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
    }
}
