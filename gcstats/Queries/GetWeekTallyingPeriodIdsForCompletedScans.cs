using Dapper;
using MediatR;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public abstract class GetWeekTallyingPeriodIdsForCompletedScans
    {
        public class Request : IRequest<IEnumerable<int>> { }

        public class Handler : IRequestHandler<Request, IEnumerable<int>>
        {
            private readonly IDbConnection connection;
            private const string sql = @"
                SELECT TallyingPeriodId
                FROM   (SELECT TallyingPeriodId,
                               Count(*) AS Progress
                        FROM   RawHtml
                        GROUP  BY TallyingPeriodId)
                WHERE  Progress == 1020";
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
