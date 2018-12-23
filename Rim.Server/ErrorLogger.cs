using System;

namespace Rim.Server
{
    /// <summary>
    /// Default error logger class of the rim server
    /// </summary>
    public class ErrorLogger : IErrorLogger
    {
        /// <summary>
        /// Server of the error logger.
        /// If logging only client side, this value can be null
        /// </summary>
        public HttpServer Server { get; }

        public ErrorLogger(HttpServer server)
        {
            Server = server;
        }

        /// <summary>
        /// Logs the error
        /// </summary>
        public void Log(string hint, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
