using System;
using Autofac;
using Infrastructure;
using NLog;
using NLog.Config;
using ILogger = Infrastructure.ILogger;

namespace Scheduler.Services
{
	public class SchedulerService : StartableService
	{
		public ILogger Logger { get; set; } = new NullLogger();
		private IScheduler _scheduler;
		private IContainer _container;
		public static void Main()
		{
			/*
			 
			﻿Необходимо разработать сервис по отправке отложенных сообщений.
			Сервис раз в 2 минуты должен забирать сообщения из базы данных MS SQL Server и отправлять их в RabbitMQ.
			Сообщения в базу будут записываться другими сервисами, структура таблицы Messages представлена в проекте Database. 
			В RabbitMQ сообщения должны попадать в разные очереди, в зависимости от типа сообщения. 
			В сообщениях должна быть информация об адресе отправителя, адресе получателя и текст сообщения.

			 */
			Run(new SchedulerService());
		}

		public override void Start()
		{
			LogManager.ThrowExceptions = true;
			LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");

			_container = IocContainer.GetContainer();
			_scheduler = _container.Resolve<IScheduler>();
			_scheduler.Start();
		}

		protected override void OnStop()
		{
			_scheduler?.Stop();
			_container?.Dispose();
			base.OnStop();
		}

		public override void OnError(Exception exception)
		{
			Logger.Fatal($"Exception handled in {nameof(OnError)} method", exception);
		}
	}
}
