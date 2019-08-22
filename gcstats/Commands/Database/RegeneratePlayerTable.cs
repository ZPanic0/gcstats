using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands.Database
{
    public static class RegeneratePlayerTable
    {
        public class Request : IRequest<int> { }

        public class Handler : IRequestHandler<Request, int>
        {
            private const string sql = @"
                DROP TABLE IF EXISTS Player;
                CREATE TABLE Player (
                  LodestoneId INTEGER PRIMARY KEY,
                  Name INTEGER NOT NULL,
                  PortraitUrl TEXT NOT NULL,
                  FactionId INTEGER NOT NULL,
                  FactionRankId INTEGER NOT NULL,
                  ServerId INTEGER NOT NULL
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
