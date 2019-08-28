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

namespace gcstats.Queries
{
    public static class ParsePlayerDataFromPage
    {
        public class Request : IRequest<Result[]>
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

        public class Handler : IRequestHandler<Request, Result[]>
        {
            private readonly Paths pathSettings;
            private readonly HtmlDocument document;

            public Handler(Paths pathSettings, HtmlDocument document)
            {
                this.pathSettings = pathSettings;
                this.document = document;
            }
            public Task<Result[]> Handle(Request request, CancellationToken cancellationToken)
            {
                document.LoadHtml(request.PageHtml);

                return Task.FromResult(GetResults(request.TargetFaction, request).ToArray());
            }

            private IEnumerable<Result> GetResults(Faction targetFaction, Request request)
            {
                foreach (var row in document.DocumentNode.SelectNodes(pathSettings.BasePath) ?? Enumerable.Empty<HtmlNode>())
                {
                    var playerName = row.SelectSingleNode(pathSettings.PlayerName)?.InnerText ?? string.Empty;

                    var serverAndDatacenterMatch = Regex.Matches(
                        row.SelectSingleNode(pathSettings.ServerAndDatacenterName).InnerText,
                        "[A-z]+");

                    var factionAndRankNameMatch = Regex.Matches(
                            row.SelectSingleNode(pathSettings.FactionAndRankName)?.Attributes["alt"].Value
                            ?? "No Input", 
                        "[A-z ]+");

                    yield return new Result
                    {
                        Rank = int.Parse(row.SelectSingleNode(pathSettings.Rank).InnerText),
                        PortraitUrl = row.SelectSingleNode(pathSettings.PortraitUrl).Attributes["src"].Value,
                        PlayerName = playerName,
                        Server = Enum.Parse<Server>(serverAndDatacenterMatch.First().Value),
                        TargetFaction = targetFaction,
                        CurrentFaction = Enum.Parse<Faction>(factionAndRankNameMatch.First().Value.Replace(" ", string.Empty)),
                        CurrentFactionRank = Enum.Parse<FactionRank>(factionAndRankNameMatch.Last().Value.Replace(" ", string.Empty)),
                        CompanySeals = int.Parse(row.SelectSingleNode(pathSettings.CompanySeals).InnerText),
                        LodestoneId = int.Parse(Regex.Match(row.Attributes["data-href"].Value, "[0-9]+").Value)
                    };
                }
            }
        }
    }
}