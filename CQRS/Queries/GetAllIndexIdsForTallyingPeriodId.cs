using CQRS.Common;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.Queries
{
    public static class GetAllIndexIdsForTallyingPeriodId
    {
        public class Request : IRequest<IEnumerable<long>>
        {
            public Request(int tallyingPeriodId)
            {
                TallyingPeriodId = tallyingPeriodId;
            }

            public int TallyingPeriodId { get; }
        }

        public class Handler : IRequestHandler<Request, IEnumerable<long>>
        {
            private readonly IMediator mediator;
            private readonly Sets sets;

            public Handler(IMediator mediator, Sets sets)
            {
                this.mediator = mediator;
                this.sets = sets;
            }

            public Task<IEnumerable<long>> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(GetIndexIdRequests(request.TallyingPeriodId));
            }

            private IEnumerable<long> GetIndexIdRequests(int tallyingPeriodId)
            {
                foreach (var datacenter in sets.Datacenters)
                    foreach (var server in sets.Servers.Dictionary[datacenter])
                        foreach (var faction in sets.Factions)
                            foreach (var page in sets.PageNumbers)
                                yield return mediator.Send(new GetIndexFromQueryData.Request(
                                    tallyingPeriodId,
                                    server,
                                    faction,
                                    page)).Result;
            }
        }
    }
}