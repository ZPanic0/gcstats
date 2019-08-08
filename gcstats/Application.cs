using System;
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
            await Mediator.Send(new SetupTables.Request());
        }
    }
}
