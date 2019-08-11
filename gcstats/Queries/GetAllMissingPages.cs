using gcstats.Common;
using gcstats.Common.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
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
            public Func<IEnumerable<Tuple<DownloadPage.Request, string>>, Task> Callback { get; set; }
            public int Delay { get; set; } = 1000;
            public int BatchSize { get; set; } = 15;
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
                foreach (var batch in GetBatches(GetRequests(await GetTallyingPeriodIds()), request.BatchSize))
                {
                    var tasks = new List<Task<string>>();

                    foreach (var pageRequest in batch)
                    {
                        Console.WriteLine($"Fetching for {pageRequest.TallyingPeriodId}, {pageRequest.Server}, {pageRequest.Faction}, Page {pageRequest.Page}");
                        tasks.Add(mediator.Send(pageRequest));
                    }

                    await Task.WhenAll(tasks);

                    await request.Callback(
                        batch.Zip(tasks, (batchItem, task) => 
                            new Tuple<DownloadPage.Request, string>(batchItem, task.Result)));
                }

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
                                    yield return new DownloadPage.Request(tallyingPeriodId, TimePeriod.Weekly, server, faction, page);
                                }
                            }
                        }
                    }
                }
            }

            private IEnumerable<IEnumerable<DownloadPage.Request>> GetBatches(IEnumerable<DownloadPage.Request> requests, int batchSize)
            {
                var index = 0;
                var length = requests.Count();

                while (index < length)
                {
                    yield return requests.Skip(index).Take(batchSize);

                    index += 5;
                }
            }
        }
    }
}