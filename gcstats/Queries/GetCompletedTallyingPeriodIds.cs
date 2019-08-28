using Dapper;
using MediatR;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetCompletedTallyingPeriodIds
    {
        public class Request : IRequest<IEnumerable<int>> { }

        public class Handler : IRequestHandler<Request, IEnumerable<int>>
        {
            private const string sql = @"
                SELECT
                  TallyingPeriodId
                FROM
                  ScanProgress
                WHERE
                  ScanCompleted = true";

            private readonly IDbConnection connection;

            public Handler(IDbConnection connection)
            {
                this.connection = connection;
            }

            public Task<IEnumerable<int>> Handle(Request request, CancellationToken cancellationToken)
            {
                return connection.QueryAsync<int>(sql);
            }
        }
    }
}
