using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetMissingIndexIds
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
            public async Task<IEnumerable<long>> Handle(Request request, CancellationToken cancellationToken)
            {
                var completedIndexIdsTask = mediator.Send(
                    new GetSavedPageIndexIds.Request(request.TallyingPeriodId));

                var allIndexIdsTask = mediator.Send(
                    new GetAllIndexIdsForTallyingPeriodId.Request(request.TallyingPeriodId));

                await Task.WhenAll(allIndexIdsTask, completedIndexIdsTask);

                return allIndexIdsTask.Result.Except(completedIndexIdsTask.Result);
            }
        }
    }
}