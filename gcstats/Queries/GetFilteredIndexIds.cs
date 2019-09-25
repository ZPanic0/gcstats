using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Common;
using gcstats.Common.Enums;
using MediatR;

namespace gcstats.Queries
{
    public class GetFilteredIndexIds
    {
        public class Request : IRequest<IAsyncEnumerable<long>>
        {
            public Request(int tallyingPeriodId)
            {
                TallyingPeriodId = tallyingPeriodId;
            }

            public int TallyingPeriodId { get; }
            public IEnumerable<Server> Servers { get; set; }
            public IEnumerable<Faction> Factions { get; set; }
            public IEnumerable<int> Pages { get; set; }
        }

        public class Handler : IRequestHandler<Request, IAsyncEnumerable<long>>
        {
            private readonly IMediator mediator;
            private readonly Sets sets;

            public Handler(IMediator mediator, Sets sets)
            {
                this.mediator = mediator;
                this.sets = sets;
            }
            public async Task<IAsyncEnumerable<long>> Handle(Request request, CancellationToken cancellationToken)
            {
                return GetIndexIdRequests(request);
            }

            private async IAsyncEnumerable<long> GetIndexIdRequests(Request request)
            {
                foreach (var server in request.Servers ?? sets.Servers.All)
                    foreach (var faction in request.Factions ?? sets.Factions)
                        foreach (var page in request.Pages ?? sets.PageNumbers)
                            yield return await mediator.Send(new GetIndexFromQueryData.Request(
                                request.TallyingPeriodId,
                                TimePeriod.Weekly,
                                server,
                                faction,
                                page));
            }
        }
    }
}
