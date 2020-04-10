using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
using MediatR;

namespace CQRS.Queries
{
    public static class GetIndexFromQueryData
    {
        public class Request : IRequest<long>
        {
            public Request(int tallyingPeriodId, Server server, Faction faction, int page)
            {
                TallyingPeriodId = tallyingPeriodId;
                Server = server;
                Faction = faction;
                Page = page;
            }

            public int TallyingPeriodId { get; }
            public Server Server { get; }
            public Faction Faction { get; }
            public int Page { get; }
        }

        public class Handler : IRequestHandler<Request, long>
        {
            public Task<long> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(
                    (long)request.TallyingPeriodId * 10000
                    + (int)request.Server * 100
                    + (int)request.Faction * 10
                    + request.Page
                );
            }
        }
    }
}
