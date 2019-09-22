using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Common;
using gcstats.Configuration.Models;
using gcstats.Queries;
using MediatR;

namespace gcstats.Commands
{
    public class FillMissingPagesToZip
    {
        public class Request : IRequest { }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator mediator;
            private readonly AppSettings appSettings;
            private readonly ILogger logger;

            public Handler(IMediator mediator, AppSettings appSettings, ILogger logger)
            {
                this.mediator = mediator;
                this.appSettings = appSettings;
                this.logger = logger;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var tallyingPeriodIds = await mediator.Send(new GetTallyingPeriodIdsToCurrent.Request());

                foreach (var tallyingPeriodId in tallyingPeriodIds)
                {
                    logger.WriteLine($"Filling pages for TallyingPeriodId: {tallyingPeriodId}");

                    var missingIndexIds = (await mediator.Send(new GetMissingIndexIds.Request(tallyingPeriodId))).ToArray();

                    var filePath = string.Format(appSettings.ProtobufSettings.OutputPathTemplate, appSettings.BaseDirectory, tallyingPeriodId);

                    await mediator.Send(new GetPagesByIndexIds.Request(missingIndexIds));
                }

                return Unit.Value;
            }
        }
    }
}
