using gcstats.Common;
using gcstats.Common.Extensions;
using gcstats.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class SaveAllMissingPages
    {
        public class Request : IRequest
        {
            public Func<string, Task> Callback { get; set; }
            public int Delay { get; set; } = 1000;
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
                foreach (var tallyingPeriodId in await GetTallyingPeriodIds())
                {
                    foreach (var datacenter in ((Datacenter[])Enum.GetValues(typeof(Datacenter))).Skip(1))
                    {
                        foreach (var server in datacenter.GetServers())
                        {
                            foreach (var faction in ((Faction[])Enum.GetValues(typeof(Faction))).Skip(1))
                            {
                                for (int page = 1; page <= 5; page++)
                                {
                                    Console.WriteLine($"Fetching for {tallyingPeriodId}, {server}, {faction}, Page {page}");

                                    var htmlStringResult = await mediator.Send(
                                    new GetPage.Request(
                                        tallyingPeriodId,
                                        TimePeriod.Weekly,
                                        server,
                                        faction,
                                        page));

                                    if (!htmlStringResult.RetrievedFromCache)
                                    {
                                        await Task.WhenAll(
                                            request.Callback?.Invoke(htmlStringResult.HtmlString) ?? Task.CompletedTask,
                                            mediator.Send(new SavePage.Request(
                                                tallyingPeriodId,
                                                TimePeriod.Weekly,
                                                server,
                                                faction,
                                                page,
                                                htmlStringResult.HtmlString)),
                                            Task.Delay(request.Delay)
                                        );
                                    }
                                }
                            }
                        }
                    }
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
        }
    }
}
