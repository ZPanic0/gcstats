using gcstats.Queries;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public abstract class SetupTables
    {
        public class Request : IRequest { }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                if (!await mediator.Send(new CheckIfTablesExist.Request()))
                {
                    Console.WriteLine("Tables missing or incomplete. Creating...");
                    await mediator.Send(new CreateDefaultTables.Request());
                    Console.WriteLine("Verifying...");
                    if (await mediator.Send(new CheckIfTablesExist.Request()))
                    {
                        Console.WriteLine("Tables created successfully.");
                    }
                    else
                    {
                        throw new Exception("Failed to create database tables");
                    }
                }

                return Unit.Value;
            }
        }
    }
}
