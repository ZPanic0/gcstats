using gcstats.Common;
using gcstats.Common.Extensions;
using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetAllMissingPages
    {
        public class Request : IRequest
        {
            public Func<ConcurrentQueue<Tuple<DownloadPage.Request, string>>, Func<bool>, Task> Callback { get; set; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator mediator;
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

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var stopwatch = Stopwatch.StartNew();
                var results = new ConcurrentQueue<Tuple<DownloadPage.Request, string>>();
                SetIsFetching(true);

                var callback = request.Callback(results, GetIsFetching);

                Parallel.ForEach(GetRequests(await GetTallyingPeriodIds()),
                    new ParallelOptions { MaxDegreeOfParallelism = 4 },
                    pageRequest =>
                    {
                        Console.WriteLine($"Fetching for {pageRequest.TallyingPeriodId}, {pageRequest.Server}, {pageRequest.Faction}, Page {pageRequest.Page}");
                        var page = mediator.Send(pageRequest).Result;

                        results.Enqueue(new Tuple<DownloadPage.Request, string>(pageRequest, page));
                    });

                SetIsFetching(false);

                await callback;

                stopwatch.Stop();
                Console.WriteLine($"Finished in {stopwatch.ElapsedMilliseconds} ms.");

                return Unit.Value;
            }

            private async Task<IEnumerable<int>> GetTallyingPeriodIds()
            {
                var result = Enumerable.Empty<int>();
                var lastTallyingPeriodId = await mediator.Send(new GetLastWeekTallyingPeriodId.Request());
                var lastTallyingPeriodYear = lastTallyingPeriodId / 100;

                for (var year = 2014; year < lastTallyingPeriodYear; year++)
                {
                    var weeksInYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        new DateTime(year, 12, 31),
                        CalendarWeekRule.FirstFullWeek,
                        DayOfWeek.Monday);
                    result = result.Concat(Enumerable.Range(year * 100 + 1, year * 100 + weeksInYear));
                }

                result = result
                    .Concat(Enumerable.Range(lastTallyingPeriodYear * 100 + 1, lastTallyingPeriodId))
                    .Except(await mediator.Send(new GetWeekTallyingPeriodIdsForCompletedScans.Request()));

                return result;
            }

            private IEnumerable<DownloadPage.Request> GetRequests(IEnumerable<int> tallyingPeriodIds)
            {
                foreach (var tallyingPeriodId in tallyingPeriodIds)
                {
                    foreach (var datacenter in ((Datacenter[])Enum.GetValues(typeof(Datacenter))).Skip(1))
                    {
                        foreach (var server in datacenter.GetServers())
                        {
                            foreach (var faction in ((Faction[])Enum.GetValues(typeof(Faction))).Skip(1))
                            {
                                for (int page = 1; page <= 5; page++)
                                {
                                    yield return new DownloadPage.Request(
                                        tallyingPeriodId,
                                        TimePeriod.Weekly,
                                        server,
                                        faction,
                                        page);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}