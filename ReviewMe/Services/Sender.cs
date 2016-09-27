using System.Text;
using Infrastructure;
using Newtonsoft.Json;
using Scheduler.Entities;
using Scheduler.Providers;

namespace Scheduler.Services
{
	public interface ISender
	{
		void Send(Message message);
	}

	public class Sender : ISender
	{
		private readonly IBusConnectionProvider _busConnectionProvider;
		public ILogger Logger { get; set; } = new NullLogger();

		public Sender(IBusConnectionProvider busConnectionProvider)
		{
			_busConnectionProvider = busConnectionProvider;
		}

		public void Send(Message message)
		{
			const string exchange = "messages";
			var channel = _busConnectionProvider.GetConnection();

			channel.ExchangeDeclare(exchange, type: "topic");

			var body = MessageSerializer.Serialize(message);

			channel.BasicPublish(exchange, 
								 routingKey: message.Type.ToString(),
								 basicProperties: null,
								 body: body);
			Logger.Info($"Sent message id={message.Id}");
		}

	
	}

	internal static class MessageSerializer
	{
		public static byte[] Serialize(Message message)
		{
			var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RabbitMessage
			{
				Destination = message.Destination,
				Source = message.Source,
				Text = message.Text
			}));
			return body;
		}


		private class RabbitMessage
		{
			public string Destination { get; set; }

			public string Source { get; set; }

			public string Text { get; set; }
		}
	}
}