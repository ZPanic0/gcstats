using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace gcstats.Common
{
    /// <summary>
    /// Accepts work concurrently and processes it asynchronously on a single thread.
    /// </summary>
    /// <typeparam name="T">Any object that can be processed via delegate.</typeparam>
    public class WorkBuffer<T>
    {
        private readonly int initialDelayWaitCycle;
        private readonly BlockingCollection<T> workItems;
        private readonly Func<T, Task> workOperation;

        /// <param name="workOperation">The work operation to commit on each item pushed to this buffer.</param>
        /// <param name="initialDelayWaitCycle">The initial time to delay checking for work per failed check.</param>
        public WorkBuffer(Func<T, Task> workOperation, int initialDelayWaitCycle = 1000)
        {
            workItems = new BlockingCollection<T>();
            this.workOperation = workOperation;
            this.initialDelayWaitCycle = initialDelayWaitCycle;
        }

        public async Task Start()
        {
            await AwaitInitialWork();

            await DoWork();
        }

        public void Add(T workItem)
        {
            workItems.Add(workItem);
        }

        public void Close()
        {
            workItems.CompleteAdding();
        }

        private async Task AwaitInitialWork()
        {
            while (!workItems.Any())
            {
                await Task.Delay(initialDelayWaitCycle);
            }
        }

        private async Task DoWork()
        {
            T workItem = default;

            while (!workItems.IsCompleted)
            {
                try
                {
                    workItem = workItems.Take();
                }
                catch (InvalidOperationException) { }

                await workOperation(workItem);
            }
        }
    }
}
