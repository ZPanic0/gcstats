using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gcstats.Configuration
{
    class ParallelPublishMediator : Mediator
    {
        private readonly ILogger logger;
        private readonly List<Task> tasks;

        public ParallelPublishMediator(ServiceFactory serviceFactory, ILogger logger) : base(serviceFactory) {
            this.logger = logger;
            tasks = new List<Task>();
        }

        protected override Task PublishCore(IEnumerable<Func<Task>> allHandlers)
        {
            foreach (var task in tasks.Where(x => x.IsFaulted))
            {
                LogFailedTask(task);
            }

            tasks.RemoveAll(task => task.IsCompleted);

            foreach (var handler in allHandlers)
            {
                tasks.Add(Task.Run(() => handler()));
            }

            return Task.CompletedTask;
        }

        private void LogFailedTask(Task task)
        {
            logger.WriteLine(task.Exception.Message);

            throw task.Exception;
        }
    }
}
