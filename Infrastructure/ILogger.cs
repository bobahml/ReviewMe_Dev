using System;

namespace Infrastructure
{
    public interface ILogger
    {
        void Debug(string message);

        void Info(string message);

        void Error(string message);

        void Error(string message, Exception exception);

        void Fatal(string message);

        void Fatal(string message, Exception exception);
    }
}