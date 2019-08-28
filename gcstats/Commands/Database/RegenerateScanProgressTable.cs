using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands.Database
{
    public static class RegenerateScanProgressTable
    {
        public class Request : IRequest<int> { }

        public class Handler : IRequestHandler<Request, int>
        {
            private const string sql = @"
                DROP TABLE IF EXISTS ScanProgress;
                CREATE TABLE ScanProgress (
                  TallyingPeriodId INTEGER PRIMARY KEY,
                  ScanCompleted INTEGER NOT NULL
                )";

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