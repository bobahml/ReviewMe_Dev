using System;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Scheduler.Entities;
using Scheduler.Services;

namespace Scheduler.Tests
{
	[TestFixture]
	public class MessageFormatTest
	{
		[Test]
		public void Serialize()
		{
			var msg = new Message
			{
				Id = 1,
				Type = 2,
				DelayDateTime = new DateTime(2016, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc),
				Destination = "Destination_test",
				Source = "Source_test",
				Text = "Text_test"
			};

			var messageBytes = MessageSerializer.Serialize(msg);

			dynamic def = JObject.Parse(Encoding.UTF8.GetString(messageBytes));
			
			Assert.AreEqual(msg.Source, (string)def.Source);
			Assert.AreEqual(msg.Destination, (string)def.Destination);
			Assert.AreEqual(msg.Text, (string)def.Text);
		}
	}
}
