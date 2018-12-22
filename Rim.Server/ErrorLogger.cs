using System;
using System.Collections.Generic;
using System.Text;

namespace Rim.Server
{
    public class ErrorLogger : IErrorLogger
    {
        public HttpServer Server { get; }

        public ErrorLogger(HttpServer server)
        {
            Server = server;
        }

        public void Log(Exception exception)
        {

        }
    }
}
