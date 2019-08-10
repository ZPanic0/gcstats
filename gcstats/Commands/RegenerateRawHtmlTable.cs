using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class RegenerateRawHtmlTable
    {
        public class Request : IRequest<int> { }

        public class Handler : IRequestHandler<Request, int>
        {
            private const string sql = @"
                DROP TABLE IF EXISTS RawHtml;
                CREATE TABLE RawHtml (
                  Id INTEGER PRIMARY KEY,
                  TallyingPeriodId INTEGER NOT NULL,
                  TimePeriodId INTEGER NOT NULL,
                  FactionId INTEGER NOT NULL,
                  ServerId INTEGER NOT NULL,
                  Page INTEGER NOT NULL,
                  HtmlString TEXT NOT NULL,
                  IndexId INTEGER NOT NULL,
                  FOREIGN KEY(TimePeriodId) REFERENCES TimePeriod(Id),
                  FOREIGN KEY(FactionId) REFERENCES Faction(Id),
                  FOREIGN KEY(ServerId) REFERENCES Server(Id)
                );";

            private readonly IDbConnection connection;

            public Handler(IDbConnection connection)
            {
                this.connection = connection;
            }

            public Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                return connection.ExecuteAsync(sql);
            }
        }
    }
}