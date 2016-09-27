using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Scheduler.Providers;

namespace Scheduler.Services
{
	public interface IScheduler
	{
		void Start();
		void Stop();
	}
	internal class Scheduler : IScheduler
	{
		private readonly IMessageDataProvider _messageDataProvider;
		private readonly ISender _sender;
		public ILogger Logger { get; set; } = new NullLogger();

		public Scheduler(IMessageDataProvider messageDataProvider, ISender sender)
		{
			_messageDataProvider = messageDataProvider;
			_sender = sender;
		}


		private Task _mainLoopTask = Task.FromResult(0);
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		private int _interval = (int)TimeSpan.FromMinutes(2).TotalMilliseconds;
		/// <summary>
		/// Sleep interval in Milliseconds
		/// </summary>
		public int Interval
		{
			get { return _interval; }
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(nameof(Interval));

				_interval = value;
			}
		}

		/// <summary>
		/// Start processing
		/// </summary>
		/// <remarks>The method is not thread-safe</remarks>
		public void Start()
		{
			if (!_mainLoopTask.IsCompleted)
				throw new InvalidOperationException("Scheduler already started");

			if (Debugger.IsAttached)
			{
				Interval = 100;
			}

			_cancellationTokenSource = new CancellationTokenSource();
			_mainLoopTask = ProcessingLoop(_cancellationTokenSource.Token);
		}

		private async Task ProcessingLoop(CancellationToken token)
		{
			Logger.Debug("Start processing!");
			
			await Task.Yield();

			while (!token.IsCancellationRequested)
			{
				try
				{
					var messagesByDate = _messageDataProvider.GetMessagesByDate(DateTime.Now, 100);

					if (messagesByDate.Count > 0)
					{
						Logger.Debug($"Received {messagesByDate.Count} messages");

						foreach (var message in messagesByDate)
						{
							_sender.Send(message);
						}

						_messageDataProvider.DeleteMessages(messagesByDate);
					}
				}
				catch (OperationCanceledException) when (token.IsCancellationRequested)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error("Main loop error", e);
				}

				await Task.Delay(Interval, token).ConfigureAwait(false);
			}
		}


		public async void Stop()
		{
			try
			{
				_cancellationTokenSource.Cancel();
				await _mainLoopTask;
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception e)
			{
				Logger.Error($"Stop {nameof(Scheduler)} error", e);
			}
		}
	}
}
