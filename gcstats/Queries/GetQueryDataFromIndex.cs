using gcstats.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetQueryDataFromIndex
    {
        public class Request : IRequest<Result>
        {
            public Request(long index)
            {
                Index = index;
            }

            public long Index { get; }
        }

        public class Result
        {
            public int TallyingPeriodId { get; set; }
            public TimePeriod TimePeriod { get; set; }
            public Server Server { get; set; }
            public Faction Faction { get; set; }
            public int Page { get; set; }
        }

        public class Handler : IRequestHandler<Request, Result>
        {
            public Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Result
                {
                    TallyingPeriodId = (int)(request.Index / 100000 % 1000000),
                    TimePeriod = (TimePeriod)(request.Index / 10000 % 10),
                    Server = (Server)(request.Index / 100 % 100),
                    Faction = (Faction)(request.Index / 10 % 10),
                    Page = (int)(request.Index % 10)
                });
            }
        }
    }
}
