using Dapper;
using gcstats.Queries;
using MediatR;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class SavePages
    {
        public class Request : IRequest
        {
            public IEnumerable<SavePage.Request> Pages { get; set; }
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
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var pageRequest in request.Pages)
                    {
                        await mediator.Send(pageRequest);
                    }

                    transaction.Commit();
                }
                connection.Close();

                return Unit.Value;
            }
        }
    }
}
