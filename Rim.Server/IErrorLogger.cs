using System;

namespace Rim.Server
{
    /// <summary>
    /// Error Logging implementation for HttpServer, ClientSocket, ServerSocket, Firewall, ClientContainer and HttpRequestHandler objects
    /// </summary>
    public interface IErrorLogger
    {
        
        /// <summary>
        /// Logs the error
        /// </summary>
        void Log(string hint, Exception exception);

    }
}
