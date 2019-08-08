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
            public Handler(IMediator mediator)
            {
                Mediator = mediator;
            }

            public IMediator Mediator { get; }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                if (!await Mediator.Send(new CheckIfTablesExist.Request()))
                {
                    Console.WriteLine("Tables missing or incomplete. Creating...");
                    await Mediator.Send(new CreateDefaultTables.Request());
                    Console.WriteLine("Verifying...");
                    if (await Mediator.Send(new CheckIfTablesExist.Request()))
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
