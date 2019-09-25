using gcstats.Commands;
using gcstats.Common;
using MediatR;
using System.Threading.Tasks;

namespace gcstats
{
    public class Application
    {
        private readonly IMediator mediator;
        private readonly IWriteQueue<SavePageToZip.Request> zipQueue;
        private readonly IWriteQueue<SavePageReport.Request> protobufCacheQueue;
        private readonly IWriteQueue<SavePageReports.Request> bulkProtobufCacheQueue;

        public Application(
            IMediator mediator,
            IWriteQueue<SavePageToZip.Request> zipQueue,
            IWriteQueue<SavePageReport.Request> protobufCacheQueue,
            IWriteQueue<SavePageReports.Request> bulkProtobufCacheQueue)
        {
            this.mediator = mediator;
            this.zipQueue = zipQueue;
            this.protobufCacheQueue = protobufCacheQueue;
            this.bulkProtobufCacheQueue = bulkProtobufCacheQueue;
        }

        internal async Task Execute()
        {
            await mediator.Send(new SetupFileStructure.Request());

            var zipQueueTask = Task.Run(() => zipQueue.Start());
            var protobufCacheQueueTask = Task.Run(() => protobufCacheQueue.Start());

            await mediator.Send(new FillMissingPagesToZip.Request());

            zipQueue.Close();
            protobufCacheQueue.Close();
            await Task.WhenAll(zipQueueTask, protobufCacheQueueTask);

            var bulkTask = bulkProtobufCacheQueue.Start();

            await mediator.Send(new FillMissingReportsToCache.Request());

            bulkProtobufCacheQueue.Close();
            await bulkTask;
        }
    }
}