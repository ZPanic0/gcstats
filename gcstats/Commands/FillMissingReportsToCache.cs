using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Common;
using gcstats.Common.Enums;
using gcstats.Common.ProtobufModels;
using gcstats.Queries;
using MediatR;

namespace gcstats.Commands
{
    public class FillMissingReportsToCache
    {
        public class Request : IRequest { }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IWriteQueue<SavePageReports.Request> protobufCacheQueue;
            private readonly Sets sets;
            private readonly IMediator mediator;
            private readonly ILogger logger;

            public Handler(IWriteQueue<SavePageReports.Request> protobufCacheQueue,
                Sets sets,
                IMediator mediator,
                ILogger logger)
            {
                this.protobufCacheQueue = protobufCacheQueue;
                this.sets = sets;
                this.mediator = mediator;
                this.logger = logger;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                logger.WriteLine("Filling PageReports to protobuf files...");

                var tasks = new List<Task>();
                var tallyingPeriodIds = (await mediator.Send(new GetTallyingPeriodIdsToCurrent.Request())).ToArray();

                var workGroups = sets.Servers.All.Select((server, index) => (server, index)).GroupBy(x => x.index % 4);

                foreach (var group in workGroups)
                {
                    tasks.Add(Task.Run(() => Task.WhenAll(group.Select(x => ProcessServer(x.server, tallyingPeriodIds)))));
                }

                await Task.WhenAll(tasks);

                logger.WriteLine("Done.");

                return Unit.Value;
            }

            private async Task Process(int tallyingPeriodId, Server server)
            {
                var reports = new List<PageReport>();

                var existingReports = await mediator.Send(new GetCachedPageReports.Request(server, tallyingPeriodId));

                var indexIds = await mediator.Send(
                    new GetFilteredIndexIds.Request(tallyingPeriodId)
                    {
                        Servers = new[] { server }
                    });

                await foreach (var indexId in indexIds)
                {
                    if (existingReports.Any(report => report.IndexId == indexId))
                    {
                        continue;
                    }

                    var data = await mediator.Send(new GetQueryDataFromIndex.Request(indexId));

                    var page = await mediator.Send(new GetPageFromZip.Request(indexId));

                    var report = await mediator.Send(new GetPageReport.Request(data.Faction, indexId, page));

                    reports.Add(report);
                }

                if (reports.Any())
                {
                    protobufCacheQueue.Enqueue(new SavePageReports.Request(reports, server, tallyingPeriodId));
                }
            }

            private async Task ProcessServer(Server server, IEnumerable<int> tallyingPeriodIds)
            {
                foreach (var tallyingPeriodId in tallyingPeriodIds)
                {
                    await Process(tallyingPeriodId, server);
                }
            }
        }
    }
}
