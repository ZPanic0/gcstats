using gcstats.Queries;
using MediatR;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class ParseAndSavePages
    {
        public class Request : IRequest {
            public Request(IEnumerable<long> indexIds)
            {
                IndexIds = indexIds;
            }

            public Request(int tallyingPeriodId)
            {
                TallyingPeriodId = tallyingPeriodId;
            }

            public IEnumerable<long> IndexIds { get; }
            public int TallyingPeriodId { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator mediator;
            private readonly IDbConnection connection;

            public Handler(IMediator mediator, IDbConnection connection)
            {
                this.mediator = mediator;
                this.connection = connection;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var indexIds = request.TallyingPeriodId == default 
                    ? request.IndexIds 
                    : await mediator.Send(new GetAllIndexIdsForTallyingPeriodId.Request(request.TallyingPeriodId));

                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    await foreach (var command in GetAllCommands(indexIds))
                    {
                        await mediator.Send(command);
                    }

                    transaction.Commit();
                }

                return Unit.Value;
            }

            private async IAsyncEnumerable<IRequest<int>> GetAllCommands(IEnumerable<long> indexIds)
            {
                foreach (var indexId in indexIds)
                {
                    var pageTask = mediator.Send(new GetSavedPage.Request(indexId));

                    var queryDataTask = mediator.Send(new GetQueryDataFromIndex.Request(indexId));

                    await Task.WhenAll(pageTask, queryDataTask);

                    var playerData = await mediator.Send(
                        new ParsePlayerDataFromPage.Request(queryDataTask.Result.Faction, pageTask.Result));

                    foreach (var command in GetCommandsForSet(indexId, playerData))
                    {
                        yield return command;
                    }
                }
            }

            private IEnumerable<IRequest<int>> GetCommandsForSet(long indexId, ParsePlayerDataFromPage.Result[] playerData)
            {
                yield return new DeletePerformancesByIndexId.Request(indexId);

                foreach (var item in playerData.Select(x => new UpsertPlayer.Request(
                    x.LodestoneId,
                    x.PlayerName,
                    x.PortraitUrl,
                    x.CurrentFaction,
                    x.CurrentFactionRank,
                    x.Server)))
                {
                    yield return item;
                }

                foreach (var item in playerData.Select(x => new InsertPerformance.Request(
                    x.LodestoneId,
                    x.Rank,
                    x.CompanySeals,
                    x.TargetFaction,
                    indexId)))
                {
                    yield return item;
                }
            }
        }
    }
}
