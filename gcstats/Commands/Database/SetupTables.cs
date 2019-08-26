using gcstats.Queries.Database;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands.Database
{
    public static class SetupTables
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
                var missingTables = await mediator.Send(new GetMissingTables.Request());

                if (missingTables.Any())
                {
                    Console.WriteLine("Tables missing or incomplete.");

                    foreach (var missingTableName in missingTables)
                    {
                        Console.WriteLine($"Regenerating {missingTableName}...");

                        await mediator.Send(GetCommand(missingTableName));
                    }

                    await VerifyTables();
                }

                return Unit.Value;
            }

            private IRequest<int> GetCommand(string tableName)
            {
                switch (tableName)
                {
                    case "Faction":
                        return new RegenerateFactionTable.Request();

                    case "Server":
                        return new RegenerateServerTable.Request();

                    case "Datacenter":
                        return new RegenerateDatacenterTable.Request();

                    case "TimePeriod":
                        return new RegenerateTimePeriodTable.Request();

                    case "Player":
                        return new RegeneratePlayerTable.Request();

                    case "Performance":
                        return new RegeneratePerformanceTable.Request();

                    default:
                        throw new ArgumentException($"Behavior for table name not defined: {tableName}");
                }
            }

            private async Task VerifyTables()
            {
                Console.WriteLine("Verifying...");

                if ((await mediator.Send(new GetMissingTables.Request())).Any())
                {
                    throw new Exception("Failed to create one or more database tables.");
                }
                else
                {
                    Console.WriteLine("Tables created successfully.");
                }
            }
        }
    }
}