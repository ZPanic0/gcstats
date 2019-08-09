using MediatR;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public abstract class GetLastWeekTallyingPeriodId
    {
        public class Request : IRequest<int> { }

        public class Handler : IRequestHandler<Request, int>
        {
            public Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                var now = DateTime.Now;
                var lastCalculatedMonday = now.AddDays(-7 - (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7);
                var weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    lastCalculatedMonday,
                    CalendarWeekRule.FirstFullWeek,
                    DayOfWeek.Monday);

                return Task.FromResult(lastCalculatedMonday.Year * 100 + weekNumber);
            }
        }
    }
}
