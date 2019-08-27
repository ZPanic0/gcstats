using gcstats.Commands;
using gcstats.Commands.Database;
using gcstats.Queries;
using MediatR;
using System.Threading.Tasks;

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
            var tallyingPeriodIdsTask = mediator.Send(new GetTallyingPeriodIdsToCurrent.Request());

            await Task.WhenAll(
                mediator.Send(new SetupTables.Request()),
                mediator.Send(new SetupFileStructure.Request()),
                tallyingPeriodIdsTask);

            Task activeParseRequest = null;

            foreach (var tallyingPeriodId in await tallyingPeriodIdsTask)
            {
                await mediator.Send(new GetAllMissingPages.Request(tallyingPeriodId)
                {
                    Callback = (queue, getLockState) => mediator.Send(new SaveStreamedPageData.Request(getLockState, queue, 10000))
                });

                if (activeParseRequest == null)
                {
                    activeParseRequest = mediator.Send(new ParseAndSavePages.Request(tallyingPeriodId));
                    continue;
                }

                await activeParseRequest;
                activeParseRequest = mediator.Send(new ParseAndSavePages.Request(tallyingPeriodId));
            }

            await activeParseRequest;
        }
    }
}