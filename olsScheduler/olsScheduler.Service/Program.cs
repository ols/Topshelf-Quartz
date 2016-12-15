using Castle.Windsor;
using NLog;
using olsScheduler.Service.Tasks;
using olsScheduler.Service.Windsor;
using Quartz;
using Topshelf;

namespace olsScheduler.Service
{
    public class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static IWindsorContainer _windsorContainer;

        public static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<Program>(s =>
                {
                    s.ConstructUsing(name => new Program());
                    s.WhenStarted(tc => Start());
                    s.WhenStopped(tc => Stop());
                });
                x.RunAsLocalSystem();
                x.SetDescription("olsScheduler");
                x.SetDisplayName("olsScheduler");
                x.SetServiceName("olsScheduler");
            });
        }

        private static void Stop()
        {
            var scheduler = _windsorContainer.Resolve<IScheduler>();
            scheduler.Shutdown();

            foreach (var target in LogManager.Configuration.AllTargets)
                target.Dispose();
        }

        private static void Start()
        {
            _logger.Info("Starting olsScheduler");

            _windsorContainer = new WindsorContainer()
                .Install(new ProjectInstaller());

            var scheduler = _windsorContainer.Resolve<IScheduler>();

            scheduler.Start();

            SetupScheduledTasks(scheduler);

            _logger.Info("Components registered and tasks setup for olsScheduler.");
        }

        private static void SetupScheduledTasks(IScheduler scheduler)
        {
            AddTask<olsTask>(scheduler, "1");
        }

        private static void AddTask<T>(IScheduler scheduler, string n)
        {
            var job = JobBuilder.Create(typeof(T))
                .WithIdentity(string.Concat("job", n), "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(string.Concat("trigger", n), "group1")
                .WithSimpleSchedule(x => x
                .WithIntervalInHours(1)
                .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}