using Dapper;
using gcstats.Queries;
using MediatR;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class SavePages
    {
        public class Request : IRequest
        {
            public IEnumerable<SavePage.Request> Pages { get; set; }
        }

        public class Handler : IRequestHandler<Request>
        {
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

            private readonly IDbConnection connection;
            private readonly IMediator mediator;

            public Handler(IDbConnection connection, IMediator mediator)
            {
                this.connection = connection;
                this.mediator = mediator;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var pageRequest in request.Pages)
                    {
                        await connection.ExecuteAsync(sql, new
                        {
                            TallyingPeriodId = pageRequest.TallyingPeriodId,
                            TimePeriodId = (int)pageRequest.TimePeriod,
                            FactionId = (int)pageRequest.Faction,
                            ServerId = (int)pageRequest.Server,
                            HtmlString = pageRequest.HtmlString,
                            Page = pageRequest.Page,
                            IndexId = await mediator.Send(new GetIndexFromQueryData.Request(
                        pageRequest.TallyingPeriodId,
                        pageRequest.TimePeriod,
                        pageRequest.Server,
                        pageRequest.Faction,
                        pageRequest.Page))
                        });
                    }

                    transaction.Commit();
                }
                connection.Close();

                return Unit.Value;
            }
        }
    }
}
