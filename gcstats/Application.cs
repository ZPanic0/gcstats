using System.Threading.Tasks;
using gcstats.Commands;
using MediatR;

namespace gcstats
{
    public class Application
    {
        private readonly IMediator mediator;

        public Application(IMediator mediator)
        {
            this.mediator = mediator;
        }

        internal async Task Execute()
        {
            await mediator.Send(new SetupTables.Request());
            await mediator.Send(new SaveAllMissingPages.Request());
        }
    }
}
