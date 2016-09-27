using System;

namespace Infrastructure
{
	public class NullLogger : ILogger
	{
		public void Debug(string message)
		{
		}

		public void Info(string message)
		{
		}

		public void Error(string message)
		{
		}

		public void Error(string message, Exception exception)
		{
		}

		public void Fatal(string message)
		{
		}

		public void Fatal(string message, Exception exception)
		{
		}
	}
}