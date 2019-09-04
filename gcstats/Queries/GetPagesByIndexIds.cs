using gcstats.Configuration;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetPagesByIndexIds
    {
        public class Request : IRequest
        {
            public Request(IEnumerable<long> indexIds)
            {
                IndexIds = indexIds;
            }
            public int MaxActiveRequests { get; set; } = 6;
            public IEnumerable<long> IndexIds { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator mediator;
            private readonly ILogger logger;

            public Handler(IMediator mediator, ILogger logger)
            {
                this.mediator = mediator;
                this.logger = logger;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var activeRequests = new List<Tuple<long, Task<string>>>();

                if (!request.IndexIds.Any())
                {
                    logger.WriteLine("No indexIds found. Skipping...");

                    return Unit.Value;
                }

                foreach(var indexId in request.IndexIds)
                {
                    while (activeRequests.Count >= request.MaxActiveRequests)
                    {
                        await Task.WhenAny(activeRequests.Select(x => x.Item2));

                        foreach (var activeRequest in activeRequests.ToArray())
                        {
                            if (activeRequest.Item2.IsCompletedSuccessfully)
                            {
                                await Publish(activeRequest);

                                activeRequests.Remove(activeRequest);
                            }
                        }
                    }

                    logger.WriteLine($"Queueing indexId {indexId}");
                    var queryData = await mediator.Send(new GetQueryDataFromIndex.Request(indexId));
                    activeRequests.Add(new Tuple<long, Task<string>>(indexId, mediator.Send(new DownloadPage.Request(
                        queryData.TallyingPeriodId,
                        queryData.TimePeriod,
                        queryData.Server,
                        queryData.Faction,
                        queryData.Page))));
                }

                await Task.WhenAll(activeRequests.Select(x => x.Item2));

                foreach (var activeRequest in activeRequests)
                {
                    await Publish(activeRequest);
                }

                return Unit.Value;
            }

            private async Task Publish(Tuple<long, Task<string>> pageResult)
            {
                await mediator.Publish(new PageDownloaded.Notification(
                    pageResult.Item1,
                    await mediator.Send(new TrimPageData.Request(await pageResult.Item2))));
            }
        }
    }
}