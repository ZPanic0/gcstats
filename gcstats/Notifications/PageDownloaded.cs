using System.Threading;
using System.Threading.Tasks;
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
            public class CachePage : INotificationHandler<Notification>
            {
                public CachePage(PageCache pageCache)
                {
                    PageCache = pageCache;
                }

                public PageCache PageCache { get; }

                public Task Handle(Notification notification, CancellationToken cancellationToken)
                {
                    PageCache.Load(notification.IndexId, notification.Page);

                    return Task.CompletedTask;
                }
            }
        }
    }
}