using System.Threading;
using System.Threading.Tasks;
using gcstats.Common;
using MediatR;

namespace gcstats.Queries
{
    public static class GetIndexFromQueryData
    {
        public class Request : IRequest<long>
        {
            public Request(int tallyingPeriodId, TimePeriod timePeriod, Server server, Faction faction, int page)
            {
                TallyingPeriodId = tallyingPeriodId;
                TimePeriod = timePeriod;
                Server = server;
                Faction = faction;
                Page = page;
            }

            public int TallyingPeriodId { get; }
            public TimePeriod TimePeriod { get; }
            public Server Server { get; }
            public Faction Faction { get; }
            public int Page { get; }
        }

        public class Handler : IRequestHandler<Request, long>
        {
            public Task<long> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(
                    (long)request.TallyingPeriodId * 100000
                    + (int)request.TimePeriod * 10000
                    + (int)request.Server * 100
                    + (int)request.Faction * 10
                    + request.Page
                );
            }
        }
    }
}
