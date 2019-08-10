using Dapper;
using gcstats.Common;
using gcstats.Common.Extensions;
using gcstats.Configuration;
using MediatR;
using System;
using System.Data;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetPage
    {
        public class Request : IRequest<Result>
        {
            public Request(int tallyingPeriodId, TimePeriod timePeriod, Server server, Faction faction, int page)
            {
                TallyingPeriodId = tallyingPeriodId;
                TimePeriod = timePeriod;
                Server = server;
                Faction = faction;
                Page = page;
            }

            public int TallyingPeriodId { get; }
            public TimePeriod TimePeriod { get; }
            public Server Server { get; }
            public Faction Faction { get; }
            public int Page { get; }
        }

        public class Result
        {
            public bool RetrievedFromCache { get; set; }
            public string HtmlString { get; set; }
        }

        public class Handler : IRequestHandler<Request, Result>
        {
            private const string sql = @"
                SELECT HtmlString
                FROM   RawHtml
                WHERE  TallyingPeriodId = @TallyingPeriodId
                       AND TimePeriodId = @TimePeriodId
                       AND FactionId = @FactionId
                       AND ServerId = @ServerId
                       AND DatacenterId = @DatacenterId
                       AND Page = @Page
                LIMIT  1";

            private readonly HttpClient client;
            private readonly AppSettings appSettings;
            private readonly IDbConnection connection;

            public Handler(HttpClient client, AppSettings appSettings, IDbConnection connection)
            {
                this.client = client;
                this.appSettings = appSettings;
                this.connection = connection;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                ValidateInput(request);

                var result = await connection.QuerySingleOrDefaultAsync<string>(sql, new
                {
                    TallyingPeriodId = request.TallyingPeriodId,
                    TimePeriodId = (int)request.TimePeriod,
                    FactionId = (int)request.Faction,
                    ServerId = (int)request.Server,
                    DatacenterId = (int)request.Server.GetDatacenter(),
                    Page = request.Page
                });

                return new Result
                {
                    RetrievedFromCache = result != null,
                    HtmlString = result ?? await client.GetStringAsync(
                        string.Format(
                            appSettings.LodestoneUrlTemplate,
                            request.TimePeriod.ToString().ToLower(),
                            request.TallyingPeriodId,
                            request.Page,
                            (int)request.Faction,
                            request.Server))
                };
            }

            private void ValidateInput(Request request)
            {
                string errorMessage = null;

                if (request.TimePeriod == default)
                {
                    errorMessage = "No TimePeriod configured.";
                }

                if (request.Page < 1 || request.Page > 5)
                {
                    errorMessage = "Invalid page or no page configured. Valid pages: 1 - 5.";
                }

                if (request.TallyingPeriodId == default)
                {
                    errorMessage = "No TallyingPeriodId configured.";
                }

                if (request.Server == default)
                {
                    errorMessage = "No Server configured.";
                }

                if (request.Faction == default)
                {
                    errorMessage = "No Faction configured.";
                }

                if (errorMessage != null)
                {
                    throw new ArgumentException(errorMessage);
                }
            }

        }
    }
}
