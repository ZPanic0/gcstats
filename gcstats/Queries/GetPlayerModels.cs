using MediatR;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Configuration.Models;
using System;

namespace gcstats.Queries
{
    public static partial class GetPlayerModels
    {
        public class Request : IRequest<Result>
        {
            public Request(IEnumerable<Tuple<long, string>> pages)
            {
                Pages = pages;
            }

            public IEnumerable<Tuple<long, string>> Pages { get; }
        }

        public class Result
        {
            public Result(IEnumerable<Player> playerResults)
            {
                PlayerResults = playerResults;
            }
            public IEnumerable<Player> PlayerResults { get; }
        }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {

                var performanceResultSets = request.Pages.Select(page => GetPerformanceResults(page.Item1, page.Item2));

                await Task.WhenAll(performanceResultSets);
                
                return new Result(performanceResultSets
                    .SelectMany(x => GetPlayerDataFromPerformance(x.Result)).ToArray());
            }

            private async Task<IEnumerable<ParsePlayerDataFromPage.Result>> GetPerformanceResults(long indexId, string page)
            {
                var queryData = await mediator.Send(new GetQueryDataFromIndex.Request(indexId));

                return await mediator.Send(
                    new ParsePlayerDataFromPage.Request(queryData.Faction, page));
            }

            private IEnumerable<Player> GetPlayerDataFromPerformance(IEnumerable<ParsePlayerDataFromPage.Result> performances)
            {
                var results = new Dictionary<int, Player>();

                foreach (var performance in performances)
                {
                    if (!results.ContainsKey(performance.LodestoneId))
                    {
                        results.Add(performance.LodestoneId, new Player
                        {
                            LodestoneId = performance.LodestoneId,
                            Performances = new List<Performance>()
                        });
                    }

                    var player = results[performance.LodestoneId];

                    player.PortraitUrl = performance.PortraitUrl;
                    player.PlayerName = performance.PlayerName;
                    player.Faction = performance.CurrentFaction;
                    player.FactionRank = performance.CurrentFactionRank;
                    player.Server = performance.Server;

                    player.Performances.Add(new Performance
                    {
                        Faction = performance.TargetFaction,
                        Rank = performance.Rank,
                        Score = performance.CompanySeals
                    });
                }

                return results.Select(pair => pair.Value);
            }
        }
    }
}
