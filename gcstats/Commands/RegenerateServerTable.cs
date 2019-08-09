using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public abstract class RegenerateServerTable
    {
        public class Request : IRequest { }

        public class Handler : IRequestHandler<Request>
        {
            private const string sql = @"
                DROP TABLE IF EXISTS Server;
                CREATE TABLE Server (
                  Id INTEGER PRIMARY KEY,
                  NAME TEXT NOT NULL
                );
                INSERT INTO
                  Server (Name)
                VALUES
                  ('Aegis'),
                  ('Atomos'),
                  ('Carbuncle'),
                  ('Garuda'),
                  ('Gungnir'),
                  ('Kujata'),
                  ('Ramuh'),
                  ('Tonberry'),
                  ('Typhon'),
                  ('Unicorn'),
                  ('Alexander'),
                  ('Bahamut'),
                  ('Durandal'),
                  ('Fenrir'),
                  ('Ifrit'),
                  ('Ridill'),
                  ('Tiamat'),
                  ('Ultima'),
                  ('Valefor'),
                  ('Yojimbo'),
                  ('Zeromus'),
                  ('Anima'),
                  ('Asura'),
                  ('Belias'),
                  ('Chocobo'),
                  ('Hades'),
                  ('Ixion'),
                  ('Mandragora'),
                  ('Masamune'),
                  ('Pandaemonium'),
                  ('Shinryu'),
                  ('Titan'),
                  ('Adamantoise'),
                  ('Cactuar'),
                  ('Faerie'),
                  ('Gilgamesh'),
                  ('Jenova'),
                  ('Midgardsormr'),
                  ('Sargatanas'),
                  ('Siren'),
                  ('Behemoth'),
                  ('Excalibur'),
                  ('Exodus'),
                  ('Famfrit'),
                  ('Hyperion'),
                  ('Lamia'),
                  ('Leviathan'),
                  ('Ultros'),
                  ('Balmung'),
                  ('Brynhildr'),
                  ('Coeurl'),
                  ('Diabolos'),
                  ('Goblin'),
                  ('Malboro'),
                  ('Mateus'),
                  ('Zalera'),
                  ('Cerberus'),
                  ('Louisoix'),
                  ('Moogle'),
                  ('Omega'),
                  ('Ragnarok'),
                  ('Spriggan'),
                  ('Lich'),
                  ('Odin'),
                  ('Phoenix'),
                  ('Shiva'),
                  ('Twintania'),
                  ('Zodiark');";

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