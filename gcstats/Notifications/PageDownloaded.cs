using System.Threading;
using System.Threading.Tasks;
using gcstats.Commands;
using gcstats.Common;
using MediatR;

namespace gcstats.Queries
{
    public static class PageDownloaded
    {
        public class Notification : INotification
        {
            public Notification(long indexId, string page)
            {
                IndexId = indexId;
                Page = page;
            }

            public long IndexId { get; }
            public string Page { get; }
        }

        public static class Handlers
        {
            public class SavePage : INotificationHandler<Notification>
            {
                private readonly IWriteQueue<SavePageToZip.Request> queue;

                public SavePage(IWriteQueue<SavePageToZip.Request> queue)
                {
                    this.queue = queue;
                }

                public Task Handle(Notification notification, CancellationToken cancellationToken)
                {
                    queue.Enqueue(new SavePageToZip.Request(notification.IndexId, notification.Page));

                    return Task.CompletedTask;
                }
            }

            public class ProcessPageToRecordCache : INotificationHandler<Notification>
            {
                private readonly IMediator mediator;
                private readonly IWriteQueue<SavePageReport.Request> queue;

                public ProcessPageToRecordCache(IMediator mediator, IWriteQueue<SavePageReport.Request> queue)
                {
                    this.mediator = mediator;
                    this.queue = queue;
                }

                public async Task Handle(Notification notification, CancellationToken cancellationToken)
                {
                    var queryData = await mediator.Send(new GetQueryDataFromIndex.Request(notification.IndexId));

                    var report = await mediator.Send(new GetPageReport.Request(queryData.Faction, notification.IndexId, notification.Page));

                    queue.Enqueue(new SavePageReport.Request(report, queryData.Server));
                }
            }
        }
    }
}