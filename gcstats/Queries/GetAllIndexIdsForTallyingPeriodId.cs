using gcstats.Common;
using gcstats.Common.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
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

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public Task<IEnumerable<long>> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(GetIndexIdRequests(request.TallyingPeriodId));
            }

            private IEnumerable<long> GetIndexIdRequests(int tallyingPeriodId)
            {
                foreach (var datacenter in ((Datacenter[])Enum.GetValues(typeof(Datacenter))).Skip(1))
                    foreach (var server in datacenter.GetServers())
                        foreach (var faction in ((Faction[])Enum.GetValues(typeof(Faction))).Skip(1))
                            foreach (var page in Enumerable.Range(1, 5))
                                yield return mediator.Send(new GetIndexFromQueryData.Request(
                                    tallyingPeriodId,
                                    TimePeriod.Weekly,
                                    server,
                                    faction,
                                    page)).Result;
            }
        }
    }
}