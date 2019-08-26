using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class DeletePerformancesByIndexId
    {
        public class Request : IRequest<int> {
            public Request(long indexId)
            {
                IndexId = indexId;
            }

            public long IndexId { get; }
        }

        public class Handler : IRequestHandler<Request, int>
        {
            private const string sql = @"
                DELETE FROM
                  Performance
                WHERE
                  IndexId = @IndexId";

            private readonly IDbConnection connection;

            public Handler(IDbConnection connection)
            {
                this.connection = connection;
            }
            public async Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                return await connection.ExecuteAsync(sql, request);
            }
        }
    }
}
