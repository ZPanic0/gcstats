using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetTallyingPeriodIdsToCurrent
    {
        public class Request : IRequest<IEnumerable<int>> { }

        public class Handler : IRequestHandler<Request, IEnumerable<int>>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<IEnumerable<int>> Handle(Request request, CancellationToken cancellationToken)
            {
                var tallyingPeriodIds = Enumerable.Empty<int>();
                var lastTallyingPeriodId = await mediator.Send(new GetLastWeekTallyingPeriodId.Request());
                var lastTallyingPeriodYear = lastTallyingPeriodId / 100;

                for (var year = 2014; year < lastTallyingPeriodYear; year++)
                {
                    var weeksInYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        new DateTime(year, 12, 31),
                        CalendarWeekRule.FirstFullWeek,
                        DayOfWeek.Monday);
                    var startOfYear = year * 100 + 1;
                    tallyingPeriodIds = tallyingPeriodIds
                        .Concat(Enumerable.Range(startOfYear, weeksInYear));
                }

                tallyingPeriodIds = tallyingPeriodIds
                    .Concat(Enumerable.Range(lastTallyingPeriodYear * 100 + 1, lastTallyingPeriodId % (lastTallyingPeriodYear * 100)));

                return tallyingPeriodIds;
            }
        }
    }
}
