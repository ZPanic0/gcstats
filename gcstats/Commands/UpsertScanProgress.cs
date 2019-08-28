using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class UpsertScanProgress
    {
        public class Request : IRequest<int>
        {
            public Request(int tallyingPeriodId, bool scanCompleted)
            {
                TallyingPeriodId = tallyingPeriodId;
                ScanCompleted = scanCompleted;
            }

            public int TallyingPeriodId { get; }
            public bool ScanCompleted { get; }
        }

        public class Handler : IRequestHandler<Request, int>
        {
            private const string sql = @"
                REPLACE INTO
                  ScanProgress (TallyingPeriodId, ScanCompleted)
                VALUES
                  (@TallyingPeriodId, @ScanCompleted)";

            private readonly IDbConnection connection;

            public Handler(IDbConnection connection)
            {
                this.connection = connection;
            }
            public Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                return connection.ExecuteAsync(sql, request);
            }
        }
    }
}