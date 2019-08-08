using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;

namespace gcstats.Queries
{
    public abstract class CheckIfTablesExist
    {
        public class Request : IRequest<bool> { }

        public class Handler : IRequestHandler<Request, bool>
        {
            private const string sql = @"
                SELECT 1
                WHERE  4 = (SELECT Count(*)
                            FROM   sqlite_master
                            WHERE  type = 'table'
                                   AND NAME IN( 'Datacenter', 'Faction', 'Server', 'RawHtml' ));";

            private readonly IDbConnection connection;

            public Handler(IDbConnection connection)
            {
                this.connection = connection;
            }

            public Task<bool> Handle(Request request, CancellationToken cancellationToken)
            {
                return connection.QueryFirstOrDefaultAsync<bool>(sql);
            }
        }
    }
}
