using Castle.Facilities.QuartzIntegration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using olsScheduler.Service.Tasks;
using Quartz;
using Quartz.Impl;

namespace olsScheduler.Service.Windsor
{
    public class ProjectInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IDependency>().ImplementedBy<Dependency>().LifestyleTransient());
            container.Register(Classes.FromAssemblyContaining<olsTask>().BasedOn<IJob>().LifestyleTransient());
            container.Register(Component.For<IScheduler>().UsingFactoryMethod(() =>
            {
                var sched = new StdSchedulerFactory().GetScheduler();
                sched.JobFactory = new WindsorJobFactory(container.Kernel);
                return sched;
            }).LifestyleSingleton());
        }
    }
}