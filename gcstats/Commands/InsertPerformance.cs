using Dapper;
using gcstats.Common;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class InsertPerformance
    {
        public class Request : IRequest<int>
        {
            public Request(int lodestoneId, int rank, int score, Faction faction, long indexId)
            {
                LodestoneId = lodestoneId;
                Rank = rank;
                Score = score;
                Faction = faction;
                IndexId = indexId;
            }

            public int LodestoneId { get; }
            public int Rank { get; }
            public int Score { get; }
            public Faction Faction { get; }
            public long IndexId { get; }
        }

        public class Handler : IRequestHandler<Request, int>
        {
            private const string sql = @"
                INSERT INTO
                  Performance (
                    LodestoneId,
                    Rank,
                    Score,
                    FactionId,
                    IndexId
                  )
                VALUES
                  (
                    @LodestoneId,
                    @Rank,
                    @Score,
                    @Faction,
                    @IndexId
                  )";

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
