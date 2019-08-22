using gcstats.Queries;
using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class SaveStreamedPageData
    {
        public class Request : IRequest
        {
            public Request(Func<bool> isStreaming,
                           ConcurrentQueue<Tuple<DownloadPage.Request, ParsePlayerDataFromPage.Result[]>> workQueue,
                           int sleepTime)
            {
                IsStreaming = isStreaming;
                WorkQueue = workQueue;
                SleepTime = sleepTime;
            }

            public Func<bool> IsStreaming { get; }
            public ConcurrentQueue<Tuple<DownloadPage.Request, ParsePlayerDataFromPage.Result[]>> WorkQueue { get; }
            public int SleepTime { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                while (request.IsStreaming() || !request.WorkQueue.IsEmpty)
                {
                    var results = new List<Tuple<DownloadPage.Request, ParsePlayerDataFromPage.Result[]>>();

                    var upsertPlayerCommands = Enumerable.Empty<UpsertPlayer.Request>();
                    var insertPerformanceCommands = Enumerable.Empty<InsertPerformance.Request>();
                    var count = 0;

                    while (request.WorkQueue.TryDequeue(out var result))
                    {
                        upsertPlayerCommands = upsertPlayerCommands.Concat(
                            result.Item2.Select(x => new UpsertPlayer.Request(
                                x.LodestoneId,
                                x.PlayerName,
                                x.PortraitUrl,
                                x.CurrentFaction,
                                x.CurrentFactionRank,
                                x.Server)));

                        var indexId = await mediator.Send(new GetIndexFromQueryData.Request(
                            result.Item1.TallyingPeriodId,
                            result.Item1.TimePeriod,
                            result.Item1.Server,
                            result.Item1.Faction,
                            result.Item1.Page));

                        insertPerformanceCommands = insertPerformanceCommands.Concat(
                            result.Item2.Select(x => new InsertPerformance.Request(
                                    x.LodestoneId,
                                    x.Rank,
                                    x.CompanySeals,
                                    x.TargetFaction,
                                    indexId
                                    )));
                        count++;
                    }

                    await mediator.Send(new UpsertPlayers.Request(upsertPlayerCommands));
                    await mediator.Send(new InsertPerformances.Request(insertPerformanceCommands));

                    await Task.Delay(request.SleepTime);
                    Console.WriteLine($"Saved {count} pages. Sleeping for {request.SleepTime} ms.");
                }

                return Unit.Value;
            }
        }
    }
}