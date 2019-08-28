using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;

namespace gcstats.Queries.Database
{
    public static class GetMissingTables
    {
        public class Request : IRequest<IEnumerable<string>> { }

        public class Handler : IRequestHandler<Request, IEnumerable<string>>
        {
            private const string sql = @"
                DROP TABLE IF EXISTS TableNames;
                CREATE TEMP TABLE TableNames (Name STRING NOT NULL, Weight INT NOT NULL);
                INSERT INTO
                  TableNames (Name, Weight)
                VALUES
                  ('Faction', 1),
                  ('Server', 1),
                  ('Datacenter', 1),
                  ('TimePeriod', 1),
                  ('Player', 1),
                  ('ScanProgress', 1),
                  ('Performance', 2);
                SELECT
                  TN.Name,
                  TN.Weight
                FROM
                  TableNames TN
                  LEFT JOIN sqlite_master SM ON TN.Name = sm.name
                  AND SM.type = 'table'
                WHERE
                  SM.name IS NULL
                ORDER BY
                  Weight;";

            private readonly IDbConnection connection;

            public Handler(IDbConnection connection)
            {
                this.connection = connection;
            }

            public Task<IEnumerable<string>> Handle(Request request, CancellationToken cancellationToken)
            {
                return connection.QueryAsync<string>(sql);
            }
        }
    }
}
