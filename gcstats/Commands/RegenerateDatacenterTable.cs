using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class RegenerateDatacenterTable
    {
        public class Request : IRequest { }

        public class Handler : IRequestHandler<Request>
        {
            private const string sql = @"
                DROP TABLE IF EXISTS Datacenter;
                CREATE TABLE Datacenter (
                  Id INTEGER PRIMARY KEY,
                  NAME TEXT NOT NULL
                );
                INSERT INTO
                  Datacenter (Name)
                VALUES
                  ('Elemental'),
                  ('Gaia'),
                  ('Mana'),
                  ('Aether'),
                  ('Primal'),
                  ('Crystal'),
                  ('Chaos'),
                  ('Light');";

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