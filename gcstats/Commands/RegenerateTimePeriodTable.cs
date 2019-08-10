using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class RegenerateTimePeriodTable
    {
        public class Request : IRequest<int> { }

        public class Handler : IRequestHandler<Request, int>
        {
            private const string sql = @"
                DROP TABLE IF EXISTS TimePeriod;
                CREATE TABLE TimePeriod (
                  Id INTEGER PRIMARY KEY,
                  Name STRING NOT NULL
                );
                INSERT INTO
                  TimePeriod (Name)
                VALUES
                  ('Weekly'),
                  ('Monthly');";

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