using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using Scheduler.Entities;
using Scheduler.Providers;
using Scheduler.Services;

namespace Scheduler.Tests
{
	[TestFixture]
	public class BusRoutingTest
	{
		[Test]
		public void Publish()
		{
			var connection = new Mock<IModel>();
			var busConnectionProvider = Mock.Of<IBusConnectionProvider>(p => p.GetConnection() == connection.Object);
			var to = new Sender(busConnectionProvider);

			var msg = new Message
			{
				Id = 1,
				Type = 2,
			};

			to.Send(msg);


			connection.Verify(model => model.ExchangeDeclare(It.IsAny<string>(), "topic"), Times.Once);
		}
	}
}