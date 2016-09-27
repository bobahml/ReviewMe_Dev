using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using Scheduler.Providers;
using Scheduler.Services;
using ILogger = Infrastructure.ILogger;

namespace Scheduler
{
	internal static class IocContainer
	{
		public static IContainer GetContainer()
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule(new NLogModule());

			builder.RegisterType<MessageDataProvider>().As<IMessageDataProvider>().SingleInstance();
			builder.RegisterType<Services.Scheduler>().As<IScheduler>().SingleInstance();
			builder.RegisterType<MessageDataProvider>().As<IMessageDataProvider>().SingleInstance();
			builder.RegisterType<Sender>().As<ISender>().SingleInstance();
			builder.RegisterType<BusConnectionProvider>().As<IBusConnectionProvider>().SingleInstance();
			builder.RegisterType<SchedulerService>().SingleInstance();

			return builder.Build();
		}

	}

	public class NLogModule : LogModule<ILogger>
	{
		protected override ILogger CreateLoggerFor(Type type)
		{
			return new NLogLogger(type);
		}
	}

	public abstract class LogModule<TLogger> : Module
	{
		protected abstract TLogger CreateLoggerFor(Type type);

		protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
		{
			var type = registration.Activator.LimitType;
			if (HasPropertyDependencyOnLogger(type))
			{
				registration.Activated += InjectLoggerViaProperty;
			}

			if (HasConstructorDependencyOnLogger(type))
			{
				registration.Preparing += InjectLoggerViaConstructor;
			}
		}

		private static bool HasPropertyDependencyOnLogger(Type type)
		{
			return type.GetProperties().Any(property => property.CanWrite && property.PropertyType == typeof(TLogger));
		}

		private static bool HasConstructorDependencyOnLogger(Type type)
		{
			return type.GetConstructors()
					   .SelectMany(constructor => constructor.GetParameters()
															 .Where(parameter => parameter.ParameterType == typeof(TLogger)))
					   .Any();
		}

		private void InjectLoggerViaProperty(object sender, ActivatedEventArgs<object> @event)
		{
			var type = @event.Instance.GetType();
			var propertyInfo = type.GetProperties().First(x => x.CanWrite && x.PropertyType == typeof(TLogger));
			propertyInfo.SetValue(@event.Instance, CreateLoggerFor(type), null);
		}

		private void InjectLoggerViaConstructor(object sender, PreparingEventArgs @event)
		{
			var type = @event.Component.Activator.LimitType;
			@event.Parameters = @event.Parameters.Union(new[]
			{
				new ResolvedParameter((parameter, context) => parameter.ParameterType == typeof(TLogger), (p, i) => CreateLoggerFor(type))
			});
		}
	}
}
