using gcstats.Common;
using gcstats.Configuration;
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
    public static class ParsePageDataFromHtml
    {
        public class Request : IRequest<Result[]>
        {
            public Request(string pageHtml)
            {
                PageHtml = pageHtml;
            }

            public string PageHtml { get; }
        }

        public class Result
        {
            public int Rank { get; set; }
            public string PortraitUrl { get; set; }
            public string PlayerName { get; set; }
            public Server Server { get; set; }
            public Datacenter Datacenter { get; set; }
            public Faction Faction { get; set; }
            public FactionRank FactionRank { get; set; }
            public int CompanySeals { get; set; }
            public int LodestoneId { get; set; }
        }

        public class Handler : IRequestHandler<Request, Result[]>
        {
            private readonly AppSettings appSettings;
            private readonly HtmlDocument document;

            public Handler(AppSettings appSettings, HtmlDocument document)
            {
                this.appSettings = appSettings;
                this.document = document;
            }
            public Task<Result[]> Handle(Request request, CancellationToken cancellationToken)
            {
                document.LoadHtml(request.PageHtml);

                return Task.FromResult(GetResults().ToArray());
            }

            private IEnumerable<Result> GetResults()
            {
                foreach (var row in document.DocumentNode.SelectNodes(appSettings.Paths.BasePath) ?? Enumerable.Empty<HtmlNode>())
                {
                    var serverAndDatacenterMatch = Regex.Matches(
                        row.SelectSingleNode(appSettings.Paths.ServerAndDatacenterName).InnerText,
                        "[A-z]+");

                    var factionAndRankNameMatch = Regex.Matches(
                        row.SelectSingleNode(appSettings.Paths.FactionAndRankName).Attributes["alt"].Value,
                        "[A-z ]+");

                    yield return new Result
                    {
                        Rank = int.Parse(row.SelectSingleNode(appSettings.Paths.Rank).InnerText),
                        PortraitUrl = row.SelectSingleNode(appSettings.Paths.PortraitUrl).Attributes["src"].Value,
                        PlayerName = row.SelectSingleNode(appSettings.Paths.PlayerName).InnerText,
                        Server = Enum.Parse<Server>(serverAndDatacenterMatch.First().Value),
                        Datacenter = Enum.Parse<Datacenter>(serverAndDatacenterMatch.Last().Value),
                        Faction = Enum.Parse<Faction>(factionAndRankNameMatch.First().Value),
                        FactionRank = Enum.Parse<FactionRank>(factionAndRankNameMatch.Last().Value.Replace(" ", string.Empty)),
                        CompanySeals = int.Parse(row.SelectSingleNode(appSettings.Paths.CompanySeals).InnerText),
                        LodestoneId = int.Parse(Regex.Match(row.Attributes["data-href"].Value, "[0-9]+").Value)
                    };
                }
            }
        }
    }
}