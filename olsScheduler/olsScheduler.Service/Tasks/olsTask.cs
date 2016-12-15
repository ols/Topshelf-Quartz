using Quartz;

namespace olsScheduler.Service.Tasks
{
    public class olsTask : IJob
    {
        private readonly IDependency _dependency;

        public olsTask(
            IDependency dependency)
        {
            _dependency = dependency;
        }

        public void Execute(IJobExecutionContext context)
        {
            // Do stuff.
        }
    }
}