using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands.Database
{
    public static class RegenerateFactionTable
    {
        public class Request : IRequest<int> { }

        public class Handler : IRequestHandler<Request, int>
        {
            private const string sql = @"
                DROP TABLE IF EXISTS Faction;
                CREATE TABLE Faction (
                  Id INTEGER PRIMARY KEY,
                  NAME TEXT NOT NULL
                );
                INSERT INTO
                  Faction (Name)
                VALUES
                  ('Maelstrom'),
                  ('Order Of The Twin Adder'),
                  ('Immortal Flames');";

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