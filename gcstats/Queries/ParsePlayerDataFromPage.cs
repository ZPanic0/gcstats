using gcstats.Common;
using gcstats.Configuration.Models;
using HtmlAgilityPack;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace gcstats.Queries
{
    public static class ParsePlayerDataFromPage
    {
        public class Request : IRequest<IEnumerable<Result>>
        {
            public Request(Faction targetFaction, string pageHtml)
            {
                TargetFaction = targetFaction;
                PageHtml = pageHtml;
            }

            public Faction TargetFaction { get; }
            public string PageHtml { get; }
        }

        public class Result
        {
            public int Rank { get; set; }
            public string PortraitUrl { get; set; }
            public string PlayerName { get; set; }
            public Server Server { get; set; }
            public Faction TargetFaction { get; set; }
            public Faction CurrentFaction { get; set; }
            public FactionRank CurrentFactionRank { get; set; }
            public int CompanySeals { get; set; }
            public int LodestoneId { get; set; }
        }

        public class Handler : IRequestHandler<Request, IEnumerable<Result>>
        {
            private readonly Paths pathSettings;
            private readonly HtmlDocument document;
            private static readonly Regex factionAndRankNameRegex = new Regex("[A-z ]+");
            private static readonly Regex serverAndDatacenterRegex = new Regex("[A-z]+");
            private static readonly Regex portraitUrlRegex = new Regex(@"(?:https:\/\/img2?\.finalfantasyxiv.com\/)(?<core>.*[(\.png)(\.jpg)])(.*)?");
            private static readonly Regex lodestoneIdRegex = new Regex("[0-9]+");

            public Handler(Paths pathSettings, HtmlDocument document)
            {
                this.pathSettings = pathSettings;
                this.document = document;
            }
            public async Task<IEnumerable<Result>> Handle(Request request, CancellationToken cancellationToken)
            {
                document.LoadHtml(request.PageHtml);

                var rows = document.DocumentNode.SelectNodes(pathSettings.BasePath) ?? Enumerable.Empty<HtmlNode>();


                return rows.Select(row => GetResult(request.TargetFaction, row)).ToArray();
            }

            private Result GetResult(Faction targetFaction, HtmlNode row)
            {
                var playerName = HttpUtility.HtmlDecode(row.SelectSingleNode(pathSettings.PlayerName)?.InnerText ?? string.Empty);

                var serverAndDatacenterMatch = serverAndDatacenterRegex
                    .Matches(row.SelectSingleNode(pathSettings.ServerAndDatacenterName).InnerText);

                var factionAndRankNameMatch = factionAndRankNameRegex
                    .Matches(
                        row.SelectSingleNode(pathSettings.FactionAndRankName)?.Attributes["alt"].Value
                        ?? "No Input");

                return new Result
                {
                    Rank = int.Parse(row.SelectSingleNode(pathSettings.Rank).InnerText),
                    PortraitUrl = portraitUrlRegex.Match(row.SelectSingleNode(pathSettings.PortraitUrl).Attributes["src"].Value).Groups["core"].Value,
                    PlayerName = playerName,
                    Server = Enum.Parse<Server>(serverAndDatacenterMatch.First().Value),
                    TargetFaction = targetFaction,
                    CurrentFaction = Enum.Parse<Faction>(factionAndRankNameMatch.First().Value.Replace(" ", string.Empty)),
                    CurrentFactionRank = Enum.Parse<FactionRank>(factionAndRankNameMatch.Last().Value.Replace(" ", string.Empty)),
                    CompanySeals = int.Parse(row.SelectSingleNode(pathSettings.CompanySeals).InnerText),
                    LodestoneId = int.Parse(lodestoneIdRegex.Match(row.Attributes["data-href"].Value).Value)
                };
            }
        }
    }
}