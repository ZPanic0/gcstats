using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using gcstats.Common;
using gcstats.Common.Extensions;
using MediatR;

namespace gcstats.Commands
{
    public abstract class SavePage
    {
        public class Request : IRequest<int>
        {
            public Request(int tallyingPeriodId, TimePeriod timePeriod, Server server, Faction faction, int page, string htmlString)
            {
                TallyingPeriodId = tallyingPeriodId;
                TimePeriod = timePeriod;
                Server = server;
                Faction = faction;
                Page = page;
                HtmlString = htmlString;
            }

            public int TallyingPeriodId { get; }
            public TimePeriod TimePeriod { get; }
            public Server Server { get; }
            public Faction Faction { get; }
            public int Page { get; }
            public string HtmlString { get; }
        }

        public class Handler : IRequestHandler<Request, int>
        {
            private readonly IDbConnection connection;
            private const string sql = @"
                INSERT INTO RawHtml
                            (TallyingPeriodId,
                             TimePeriodId,
                             FactionId,
                             ServerId,
                             DatacenterId,
                             HtmlString)
                VALUES      (@TallyingPeriodId,
                             @TimePeriodId,
                             @FactionId,
                             @ServerId,
                             @DatacenterId,
                             @HtmlString)";

            public Handler(IDbConnection connection)
            {
                this.connection = connection;
            }

            public Task<int> Handle(Request request, CancellationToken cancellationToken)
            {

                return connection.ExecuteAsync(sql, new
                {
                    TallyingPeriodId = request.TallyingPeriodId,
                    TimePeriodId = (int)request.TimePeriod,
                    FactionId = (int)request.Faction,
                    ServerId = (int)request.Server,
                    DatacenterId = (int)request.Server.GetDatacenter(),
                    HtmlString = request.HtmlString
                });
            }
        }
    }
}
