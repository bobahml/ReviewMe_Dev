using System;
using Infrastructure;
using RabbitMQ.Client;
using System.Configuration;
using System.Threading.Tasks;

namespace Scheduler.Providers
{
	public interface IBusConnectionProvider
	{
		IModel GetConnection();
	}

	internal class BusConnectionProvider : IBusConnectionProvider
	{
		private IConnection _connection;
		private IModel _model;
		private readonly object _connectionLock = new object();
		private readonly Lazy<IConnectionFactory> _connectionFactory = new Lazy<IConnectionFactory>(CreateFactory);
		private readonly ILogger _logger;
		private const int SleepTime = 500;
		public BusConnectionProvider(ILogger logger)
		{
			_logger = logger;
		}

		public IModel GetConnection()
		{
			if (!IsConnectionValid())
				Reconnect();

			return _model;
		}

		private bool IsConnectionValid()
		{
			return _model != null && _model.IsOpen && _connection != null && _connection.IsOpen;
		}

		private void Reconnect()
		{
			if (IsConnectionValid())
				return;

			lock (_connectionLock)
			{
				while (!IsConnectionValid())
				{
					TryDisposeConnection();

					try
					{
					}
					catch (Exception e)
					{
						_logger.Error("CreateConnection error", e);
						Task.Delay(SleepTime).Wait();
					}

					CreateConnection();
				}
			}
		}

		private void CreateConnection()
		{
			_connection = _connectionFactory.Value.CreateConnection();
			_model = _connection.CreateModel();
		}

		private void TryDisposeConnection()
		{
			try
			{
				_model?.Dispose();
			}
			catch (Exception e)
			{
				_logger.Error("Connection error", e);
			}

			try
			{
				_connection?.Dispose();
			}
			catch (Exception e)
			{
				_logger.Error("Connection error", e);
			}
		}

		private static ConnectionFactory CreateFactory()
		{
			//TODO move bus settings to separate class
			var hostName = ConfigurationManager.AppSettings["HostName"];
			var userName = ConfigurationManager.AppSettings["UserName"];
			var password = ConfigurationManager.AppSettings["Password"];
			var virtualHost = ConfigurationManager.AppSettings["VirtualHost"];


			var factory = new ConnectionFactory();

			if (hostName != null)
				factory.HostName = hostName;
			if (userName != null)
				factory.UserName = userName;
			if (password != null)
				factory.Password = password;
			if (virtualHost != null)
				factory.VirtualHost = virtualHost;

			return factory;
		}
	}
}
