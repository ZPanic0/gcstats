using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public abstract class RegenerateFactionTable
    {
        public class Request : IRequest { }

        public class Handler : IRequestHandler<Request>
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
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                await connection.ExecuteAsync(sql);

                return Unit.Value;
            }
        }
    }
}