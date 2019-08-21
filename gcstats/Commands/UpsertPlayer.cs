using Dapper;
using gcstats.Common;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class UpsertPlayer
    {
        public class Request : IRequest
        {
            public Request(int lodestoneId, string playerName, string portraitUrl, Faction faction, FactionRank factionRank, Server server)
            {
                LodestoneId = lodestoneId;
                PlayerName = playerName;
                PortraitUrl = portraitUrl;
                Faction = faction;
                FactionRank = factionRank;
                Server = server;
            }
            public int LodestoneId { get;}
            public string PlayerName { get;}
            public string PortraitUrl { get;}
            public Faction Faction { get; }
            public FactionRank FactionRank { get; }
            public Server Server { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private const string sql = @"
                REPLACE INTO Player (
                  LodestoneId,
                  Name,
                  PortraitUrl,
                  FactionId,
                  FactionRankId,
                  ServerId
                )
                VALUES
                  (
                    @LodestoneId,
                    @PlayerName,
                    @PortraitUrl,
                    @Faction,
                    @FactionRank,
                    @Server
                  )";

            private readonly IDbConnection connection;

            public Handler(IDbConnection connection)
            {
                this.connection = connection;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                await connection.ExecuteAsync(sql, request);

                return Unit.Value;
            }
        }
    }
}