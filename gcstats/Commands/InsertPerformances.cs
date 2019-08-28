using MediatR;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class InsertPerformances
    {
        public class Request : IRequest
        {
            public Request(IEnumerable<InsertPerformance.Request> requests)
            {
                Requests = requests;
            }

            public IEnumerable<InsertPerformance.Request> Requests { get; }
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
