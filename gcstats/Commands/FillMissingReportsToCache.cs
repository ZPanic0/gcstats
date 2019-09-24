using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Common;
using gcstats.Common.ProtobufModels;
using gcstats.Configuration.Models;
using gcstats.Queries;
using MediatR;
using ProtoBuf;

namespace gcstats.Commands
{
    public class FillMissingReportsToCache
    {
        public class Request : IRequest { }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IWriteQueue<SavePageReport.Request> protobufCacheQueue;
            private readonly Sets sets;
            private readonly AppSettings appSettings;
            private readonly IMediator mediator;
            private readonly ILogger logger;

            public Handler(IWriteQueue<SavePageReport.Request> protobufCacheQueue, Sets sets, AppSettings appSettings, IMediator mediator, ILogger logger)
            {
                this.protobufCacheQueue = protobufCacheQueue;
                this.sets = sets;
                this.appSettings = appSettings;
                this.mediator = mediator;
                this.logger = logger;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                while (protobufCacheQueue.Any())
                {
                    await Task.Delay(100);
                }

                Parallel.ForEach(await GetAllMissingIndexIds(), async indexId =>
                {
                    var data = await mediator.Send(new GetQueryDataFromIndex.Request(indexId));

                    var page = await mediator.Send(new GetPageFromZip.Request(indexId));

                    var report = await mediator.Send(new GetPageReport.Request(data.Faction, indexId, page));

                    logger.WriteLine($"Saving report for IndexId {indexId}");
                    protobufCacheQueue.Enqueue(new SavePageReport.Request(report, data.Server));
                });

                return Unit.Value;
            }

            private async Task<IEnumerable<long>> GetIndexIdsFromCache()
            {
                var indexIds = new List<long>();

                foreach (var server in sets.Servers.Values.SelectMany(x => x))
                {
                    indexIds.AddRange(
                        (await mediator.Send(new GetCachedPageReportsByServer.Request(server)))
                        .Select(x => x.IndexId));
                }

                return indexIds;
            }

            private async Task<IEnumerable<long>> GetAllMissingIndexIds()
            {
                var existingIndexIds = await GetIndexIdsFromCache();

                var indexIds = new List<long>();

                foreach (var tallyingPeriodId in await mediator.Send(new GetTallyingPeriodIdsToCurrent.Request()))
                {
                    var allIndexIds = await mediator.Send(
                            new GetAllIndexIdsForTallyingPeriodId.Request(tallyingPeriodId));

                    indexIds.AddRange(allIndexIds.Except(existingIndexIds));
                }

                return indexIds;
            }
        }
    }
}
