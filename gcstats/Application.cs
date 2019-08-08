using System;
using System.Data;
using System.Threading.Tasks;
using gcstats.Commands;
using gcstats.Queries;
using MediatR;

namespace gcstats
{
    public class Application
    {
        public Application(IMediator mediator)
        {
            Mediator = mediator;
        }

        public IMediator Mediator { get; }

        internal async Task Execute()
        {
            if (!await Mediator.Send(new CheckIfTablesExist.Request()))
            {
                Console.WriteLine("Tables missing or incomplete. Creating...");
                await Mediator.Send(new CreateDefaultTables.Request());
            }
        }
    }
}
