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

            public Handler(IWriteQueue<SavePageReports.Request> protobufCacheQueue,
                Sets sets,
                IMediator mediator)
            {
                this.protobufCacheQueue = protobufCacheQueue;
                this.sets = sets;
                this.mediator = mediator;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                await AwaitQueueCatchUp();

                var queueTask = protobufCacheQueue.Start();
                var tasks = new List<Task>();
                var tallyingPeriodIds = (await mediator.Send(new GetTallyingPeriodIdsToCurrent.Request())).ToArray();

                var workGroups = sets.Servers.All.Select((server, index) => (server, index)).GroupBy(x => x.index % 4);

                foreach (var group in workGroups)
                {
                    tasks.Add(Task.Run(() => Task.WhenAll(group.Select(x => ProcessServer(x.server, tallyingPeriodIds)))));
                }

                await Task.WhenAll(tasks);

                protobufCacheQueue.Close();
                await queueTask;

                return Unit.Value;
            }

            private async Task AwaitQueueCatchUp()
            {
                while (protobufCacheQueue.Any())
                {
                    await Task.Delay(100);
                }
            }

            private async Task Process(int tallyingPeriodId, Server server)
            {
                var reports = new List<PageReport>();

                var indexIds = await mediator.Send(
                    new GetFilteredIndexIds.Request(tallyingPeriodId)
                    {
                        Servers = new[] { server }
                    });

                await foreach (var indexId in indexIds)
                {
                    var data = await mediator.Send(new GetQueryDataFromIndex.Request(indexId));

                    var page = await mediator.Send(new GetPageFromZip.Request(indexId));

                    var report = await mediator.Send(new GetPageReport.Request(data.Faction, indexId, page));

                    reports.Add(report);
                }

                protobufCacheQueue.Enqueue(new SavePageReports.Request(reports, server, tallyingPeriodId));
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
