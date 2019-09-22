using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Configuration.Models;
using gcstats.Common.Enums;

namespace gcstats.Queries
{
    public static partial class GetPageReport
    {
        public class Request : IRequest<PageReport>
        {
            public Request(Faction faction, long indexId, string pageHtml)
            {
                Faction = faction;
                IndexId = indexId;
                PageHtml = pageHtml;
            }

            public Faction Faction { get; }
            public long IndexId { get; }
            public string PageHtml { get; }
        }

        public class Result
        {
            public Result(IEnumerable<Player> playerResults)
            {
                PlayerResults = playerResults;
            }
            public IEnumerable<Player> PlayerResults { get; }
        }

        public class Handler : IRequestHandler<Request, PageReport>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<PageReport> Handle(Request request, CancellationToken cancellationToken)
            {
                var performances = await mediator.Send(new ParsePlayerDataFromPage.Request(request.Faction, request.PageHtml));

                return new PageReport
                {
                    IndexId = request.IndexId,
                    Players = ConvertToPlayerRecords(performances)
                };
            }

            private IEnumerable<Player> ConvertToPlayerRecords(IEnumerable<ParsePlayerDataFromPage.Result> performances)
            {
                var results = new Dictionary<int, Player>();

                foreach (var performance in performances)
                {
                    if (!results.ContainsKey(performance.LodestoneId))
                    {
                        results.Add(performance.LodestoneId, new Player
                        {
                            LodestoneId = performance.LodestoneId,
                            PortraitUrl = performance.PortraitUrl,
                            PlayerName = performance.PlayerName,
                            Faction = performance.CurrentFaction,
                            FactionRank = performance.CurrentFactionRank,
                            Server = performance.Server,
                            Performances = new List<Performance>()
                        });
                    }

                    results[performance.LodestoneId].Performances.Add(new Performance
                    {
                        Faction = performance.TargetFaction,
                        Rank = performance.Rank,
                        Score = performance.CompanySeals
                    });
                }

                return results.Values;
            }
        }
    }
}
