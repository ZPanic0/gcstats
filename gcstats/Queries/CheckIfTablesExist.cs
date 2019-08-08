using System.Data;
using System.Linq;
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
            private const string Sql = @"
                SELECT 1
                WHERE  4 = (SELECT Count(*)
                            FROM   sqlite_master
                            WHERE  type = 'table'
                                   AND NAME IN( 'Datacenter', 'Faction', 'Server', 'RawHtml' ));";

            public Handler(IDbConnection connection)
            {
                Connection = connection;
            }

            public IDbConnection Connection { get; }

            public Task<bool> Handle(Request request, CancellationToken cancellationToken)
            {
                return Connection.QueryFirstOrDefaultAsync<bool>(Sql);
            }
        }
    }
}
