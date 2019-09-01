using gcstats.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetSavedPage
    {
        public class Request : IRequest<string>
        {
            public Request(long indexId)
            {
                IndexId = indexId;
            }

            public long IndexId { get; }
        }

        public class Handler : IRequestHandler<Request, string>
        {
            public Handler(PageCache pageCache)
            {
                this.pageCache = pageCache;
            }

            private readonly PageCache pageCache;

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                if (!pageCache.Contains(request.IndexId))
                {
                    throw new Exception($"IndexId not found in PageCache. Id: {request.IndexId}");
                }

                return pageCache.PopPage(request.IndexId);
            }
        }
    }
}