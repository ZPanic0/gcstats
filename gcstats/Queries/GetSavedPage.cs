using Dapper;
using gcstats.Common;
using MediatR;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetSavedPage
    {
        public class Request : IRequest<string>
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

        public class Handler : IRequestHandler<Request, string>
        {
            private const string sql = @"
                SELECT HtmlString
                FROM   RawHtml
                WHERE  TallyingPeriodId = @TallyingPeriodId
                       AND TimePeriodId = @TimePeriodId
                       AND FactionId = @FactionId
                       AND ServerId = @ServerId
                       AND Page = @Page
                LIMIT  1";

            private readonly IDbConnection connection;

            public Handler(IDbConnection connection)
            {
                this.connection = connection;
            }

            public Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                ValidateInput(request);

                return connection.QuerySingleOrDefaultAsync<string>(sql, new
                {
                    TallyingPeriodId = request.TallyingPeriodId,
                    TimePeriodId = (int)request.TimePeriod,
                    FactionId = (int)request.Faction,
                    ServerId = (int)request.Server,
                    Page = request.Page
                });
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