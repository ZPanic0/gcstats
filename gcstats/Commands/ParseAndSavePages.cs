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
        public class Request : IRequest
        {
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
                await mediator.Send(new LoadFileToCache.Request(request.TallyingPeriodId));

                var indexIds = request.TallyingPeriodId == default
                    ? request.IndexIds
                    : await mediator.Send(new GetAllIndexIdsForTallyingPeriodId.Request(request.TallyingPeriodId));

                var commandSets = indexIds.Select(indexId => GetCommands(indexId)).ToArray();

                await Task.WhenAll(commandSets);

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var commandSet in commandSets)
                    {
                        var myv = await commandSet;
                        foreach (var command in myv)
                        {
                            await mediator.Send(command);
                        }
                    }

                    transaction.Commit();
                }

                return Unit.Value;
            }

            private async Task<IEnumerable<IRequest<int>>> GetCommands(long indexId)
            {
                var page = await mediator.Send(new GetSavedPage.Request(indexId));

                var queryData = await mediator.Send(new GetQueryDataFromIndex.Request(indexId));

                var playerData = await mediator.Send(
                    new ParsePlayerDataFromPage.Request(queryData.Faction, page));

                return GetCommandsForSet(indexId, playerData);
            }

            private IEnumerable<IRequest<int>> GetCommandsForSet(long indexId, IEnumerable<ParsePlayerDataFromPage.Result> playerData)
            {
                var commands = new List<IRequest<int>>(201)
                { new DeletePerformancesByIndexId.Request(indexId)};

                foreach (var item in playerData)
                {
                    commands.Add(new UpsertPlayer.Request(
                        item.LodestoneId,
                        item.PlayerName,
                        item.PortraitUrl,
                        item.CurrentFaction,
                        item.CurrentFactionRank,
                        item.Server));

                    commands.Add(new InsertPerformance.Request(
                        item.LodestoneId,
                        item.Rank,
                        item.CompanySeals,
                        item.TargetFaction,
                        indexId));
                }

                return commands;
            }
        }
    }
}
