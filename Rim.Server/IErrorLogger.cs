using System;

namespace Rim.Server
{
    public interface IErrorLogger
    {
        void Log(Exception exception);
    }
}
