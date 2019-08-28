using MediatR;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class DeletePerformancesByIndexIds
    {
        public class Request : IRequest
        {
            public Request(IEnumerable<DeletePerformancesByIndexId.Request> requests)
            {
                Requests = requests;
            }

            public IEnumerable<DeletePerformancesByIndexId.Request> Requests { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IDbConnection connection;
            private readonly IMediator mediator;

            public Handler(IDbConnection connection, IMediator mediator)
            {
                this.connection = connection;
                this.mediator = mediator;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var playerRequest in request.Requests)
                    {
                        await mediator.Send(playerRequest);
                    }

                    transaction.Commit();
                }

                return Unit.Value;
            }
        }
    }
}