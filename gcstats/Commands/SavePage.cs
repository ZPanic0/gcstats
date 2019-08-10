using Dapper;
using gcstats.Common;
using gcstats.Common.Extensions;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Queries;

namespace gcstats.Commands
{
    public static class SavePage
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
            private readonly IMediator mediator;

            private const string sql = @"
                INSERT INTO RawHtml
                            (TallyingPeriodId,
                             TimePeriodId,
                             FactionId,
                             ServerId,
                             HtmlString,
                             Page,
                             IndexId)
                VALUES      (@TallyingPeriodId,
                             @TimePeriodId,
                             @FactionId,
                             @ServerId,
                             @HtmlString,
                             @Page,
                             @IndexId)";

            public Handler(IDbConnection connection, IMediator mediator)
            {
                this.connection = connection;
                this.mediator = mediator;
            }

            public async Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                return await connection.ExecuteAsync(sql, new
                {
                    TallyingPeriodId = request.TallyingPeriodId,
                    TimePeriodId = (int)request.TimePeriod,
                    FactionId = (int)request.Faction,
                    ServerId = (int)request.Server,
                    HtmlString = request.HtmlString,
                    Page = request.Page,
                    IndexId = await mediator.Send(new GetIndexFromQueryData.Request(
                        request.TallyingPeriodId,
                        request.TimePeriod,
                        request.Server,
                        request.Faction,
                        request.Page))
                });
            }
        }
    }
}