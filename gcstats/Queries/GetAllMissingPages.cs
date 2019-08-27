using gcstats.Configuration;
using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetAllMissingPages
    {
        public class Request : IRequest
        {
            public Request(int tallyingPeriodId)
            {
                TallyingPeriodId = tallyingPeriodId;
            }
            public Func<ConcurrentQueue<Tuple<long, string>>, Func<bool>, Task> Callback { get; set; }
            public int MaxActiveRequests { get; set; } = 6;
            public int TallyingPeriodId { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator mediator;
            private readonly ILogger logger;
            private readonly object isFetchingLock = new object();
            private bool isFetching;

            private bool GetIsFetching()
            {
                lock (isFetchingLock)
                {
                    return isFetching;
                }
            }

            private void SetIsFetching(bool value)
            {
                lock (isFetchingLock)
                {
                    isFetching = value;
                }
            }

            public Handler(IMediator mediator, ILogger logger)
            {
                this.mediator = mediator;
                this.logger = logger;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var results = new ConcurrentQueue<Tuple<long, string>>();
                var activeRequests = new List<Tuple<long, Task<string>>>();
                var indexIds = new Queue<long>(await mediator.Send(new GetMissingIndexIds.Request(request.TallyingPeriodId)));

                if (!indexIds.Any())
                {
                    return Unit.Value;
                }

                SetIsFetching(true);
                var callback = request.Callback(results, GetIsFetching);
                activeRequests.Clear();

                while (indexIds.TryDequeue(out var indexId))
                {
                    while (activeRequests.Count >= request.MaxActiveRequests)
                    {
                        await Task.WhenAny(activeRequests.Select(x => x.Item2));

                        foreach (var activeRequest in activeRequests.ToArray())
                        {
                            if (activeRequest.Item2.IsCompletedSuccessfully)
                            {
                                results.Enqueue(new Tuple<long, string>(activeRequest.Item1, activeRequest.Item2.Result));
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

                foreach (var activeRequest in activeRequests.ToArray())
                {
                    results.Enqueue(new Tuple<long, string>(activeRequest.Item1, activeRequest.Item2.Result));
                }

                SetIsFetching(false);
                await callback;

                return Unit.Value;
            }
        }
    }
}