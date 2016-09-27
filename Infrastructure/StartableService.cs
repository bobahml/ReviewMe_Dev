using System;
using System.ServiceProcess;
using System.Threading;

namespace Infrastructure
{
    [System.ComponentModel.DesignerCategory("Code")]
    public abstract class StartableService : ServiceBase
    {
        public static void Run<TService>(TService service) where TService : StartableService
        {
            if (Environment.UserInteractive)
            {
                service.OnStart(null);
                Console.ReadKey();
                service.Stop();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }

        public abstract void Start();
        public virtual void OnError(Exception exception)
        {
        }

        protected override void OnStart(string[] args)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    Start();
                }
                catch (Exception exception)
                {
                    OnError(exception);
                }

            });

            thread.Start();

            base.OnStart(args);
        }
	  
    }
}