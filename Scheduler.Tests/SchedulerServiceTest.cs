using NUnit.Framework;
using Scheduler.Services;

namespace Scheduler.Tests
{
	[TestFixture]
    public class SchedulerServiceTest
	{
		[Test]
		public void Empty()
		{
			using (var to = new SchedulerService())
			{
				
			}
		}

		[Test]
		public void StartStop()
		{
			var to = new SchedulerService();
			to.Stop();
			to.Dispose();
		}
	}
}
