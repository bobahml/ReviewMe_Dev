using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Scheduler.Entities;
using Scheduler.Providers;
using Scheduler.Services;

namespace Scheduler.Tests
{
	[TestFixture]
	public class SchedulerTest
	{
		[Test]
		public void NoMessages()
		{
			var provider = Mock.Of<IMessageDataProvider>(t=>t.GetMessagesByDate(It.IsAny<DateTime>(), It.IsAny<int>()) == new Message[0]);
			var sender = new Mock<ISender>();
			sender.Setup(s => s.Send(It.IsAny<Message>())).Verifiable();

			var to = new Services.Scheduler(provider, sender.Object);
			to.Interval = 1;
			to.Start();
			to.Stop();

			sender.Verify(s=>s.Send(It.IsAny<Message>()), Times.Never);
		}


		[Test]
		public async Task Send()
		{
			var messages = new[] {new Message(), new Message() };

			var provider = Mock.Of<IMessageDataProvider>(t => t.GetMessagesByDate(It.IsAny<DateTime>(), It.IsAny<int>()) == messages);
			var sender = new Mock<ISender>();
			sender.Setup(s => s.Send(It.IsAny<Message>())).Verifiable();

			var to = new Services.Scheduler(provider, sender.Object);
			to.Start();
			await Task.Delay(100);
			to.Stop();

			sender.Verify(s => s.Send(It.IsAny<Message>()), Times.Exactly(messages.Length));
		}

	}
}
