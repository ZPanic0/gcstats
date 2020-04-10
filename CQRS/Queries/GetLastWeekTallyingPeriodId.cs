using MediatR;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.Queries
{
    public static class GetLastWeekTallyingPeriodId
    {
        public class Request : IRequest<int> {
            public Request(DateTime now)
            {
                Now = now;
            }

            public DateTime Now { get; }
        }

        public class Handler : IRequestHandler<Request, int>
        {
            public Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                var lastCalculatedMonday = request.Now.AddDays(-7 - (7 + (request.Now.DayOfWeek - DayOfWeek.Monday)) % 7);
                var weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    lastCalculatedMonday,
                    CalendarWeekRule.FirstFullWeek,
                    DayOfWeek.Monday);

                return Task.FromResult(lastCalculatedMonday.Year * 100 + weekNumber);
            }
        }
    }
}
