using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace gcstats.Configuration
{
    /// <summary>
    /// Accepts work concurrently and processes it asynchronously on a single thread.
    /// </summary>
    /// <typeparam name="T">Any object that can be processed via delegate.</typeparam>
    public class WorkBuffer<T>
    {
        private readonly BlockingCollection<T> workItems;
        private readonly Func<T, Task> workOperation;

        /// <param name="workOperation">The work operation to commit on each item pushed to this buffer.</param>
        public WorkBuffer(Func<T, Task> workOperation)
        {
            workItems = new BlockingCollection<T>();
            this.workOperation = workOperation;
        }

        public async Task Start()
        {
            foreach (var workItem in workItems.GetConsumingEnumerable())
            {
                await workOperation(workItem);
            }
        }

        public void Add(T workItem) => workItems.Add(workItem);

        public void Close() => workItems.CompleteAdding();

        public bool Any() => workItems.Any();
    }
}
