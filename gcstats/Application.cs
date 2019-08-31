using gcstats.Commands;
using gcstats.Commands.Database;
using gcstats.Configuration;
using gcstats.Queries;
using MediatR;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace gcstats
{
    public class Application
    {
        private readonly IMediator mediator;
        private readonly ILogger logger;
        private readonly IDbConnection connection;

        public Application(IMediator mediator, ILogger logger, IDbConnection connection)
        {
            this.mediator = mediator;
            this.logger = logger;
            this.connection = connection;
        }

        internal async Task Execute()
        {
            await Task.WhenAll(
                mediator.Send(new SetupTables.Request()),
                mediator.Send(new SetupFileStructure.Request()));

            var tallyingPeriodIds = (await mediator.Send(new GetTallyingPeriodIdsToCurrent.Request()))
                .Except(await mediator.Send(new GetCompletedTallyingPeriodIds.Request()));

            int activeParseRequestTallyingPeriodId = 0;
            Task activeParseRequest = null;

            foreach (var tallyingPeriodId in tallyingPeriodIds)
            {
                logger.WriteLine($"Fetching TallyingPeriod {tallyingPeriodId}");

                await mediator.Send(new UpsertScanProgress.Request(tallyingPeriodId, false));

                await mediator.Send(new GetAllMissingPages.Request(tallyingPeriodId)
                {
                    Callback = (queue, getLockState) => mediator.Send(new SaveStreamedPageData.Request(getLockState, queue, 10000))
                });

                if (activeParseRequest == null)
                {
                    logger.WriteLine($"Parsing TallyingPeriod {tallyingPeriodId}");
                    activeParseRequest = mediator.Send(new ParseAndSavePages.Request(tallyingPeriodId));
                    activeParseRequestTallyingPeriodId = tallyingPeriodId;
                    continue;
                }

                await activeParseRequest;
                await mediator.Send(new UpsertScanProgress.Request(activeParseRequestTallyingPeriodId, true));
                logger.WriteLine($"Parsing TallyingPeriod {tallyingPeriodId}");
                activeParseRequest = mediator.Send(new ParseAndSavePages.Request(tallyingPeriodId));
                activeParseRequestTallyingPeriodId = tallyingPeriodId;
            }

            await activeParseRequest;
            await mediator.Send(new UpsertScanProgress.Request(activeParseRequestTallyingPeriodId, true));

            connection.Close();
        }
    }
}