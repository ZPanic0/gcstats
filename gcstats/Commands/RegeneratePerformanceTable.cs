using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class RegeneratePerformanceTable
    {
        public class Request : IRequest<int> { }

        public class Handler : IRequestHandler<Request, int>
        {
            private const string sql = @"
                DROP TABLE IF EXISTS Performance;
                CREATE TABLE Performance (
                  PlayerId INTEGER NOT NULL,
                  Rank INTEGER NOT NULL,
                  Score INTEGER NOT NULL,
                  IndexId INTEGER NOT NULL,
                  FOREIGN KEY(PlayerId) REFERENCES Player(Id)
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
