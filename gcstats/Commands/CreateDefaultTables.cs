using Dapper;
using MediatR;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public abstract class CreateDefaultTables
    {
        public class Request : IRequest { }

        public class Handler : IRequestHandler<Request>
        {
            private const string Sql = @"
                DROP TABLE IF EXISTS RawHtml;
                
                DROP TABLE IF EXISTS Faction;
                
                DROP TABLE IF EXISTS Server;
                
                DROP TABLE IF EXISTS Datacenter;
                
                CREATE TABLE Faction
                  (
                     Id   INTEGER PRIMARY KEY,
                     NAME TEXT NOT NULL
                  );
                
                CREATE TABLE Server
                  (
                     Id   INTEGER PRIMARY KEY,
                     NAME TEXT NOT NULL
                  );
                
                CREATE TABLE Datacenter
                  (
                     Id   INTEGER PRIMARY KEY,
                     NAME TEXT NOT NULL
                  );
                
                CREATE TABLE RawHtml
                  (
                     Id           INTEGER PRIMARY KEY,
                     FactionId    INTEGER NOT NULL,
                     ServerId     INTEGER NOT NULL,
                     DatacenterId INTEGER NOT NULL,
                     HtmlString   TEXT NOT NULL,
                     FOREIGN KEY(FactionId) REFERENCES Faction(Id),
                     FOREIGN KEY(ServerId) REFERENCES Server(Id),
                     FOREIGN KEY(DatacenterId) REFERENCES Datacenter(Id)
                  );
                
                INSERT INTO Datacenter
                            (Name)
                VALUES      ('Elemental'),
                            ('Gaia'),
                            ('Mana'),
                            ('Aether'),
                            ('Primal'),
                            ('Crystal'),
                            ('Chaos'),
                            ('Light');
                
                INSERT INTO Server
                            (Name)
                VALUES('Aegis'),
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
                            ('Zodiark');
                
                INSERT INTO Faction
                            (Name)
                VALUES('Maelstrom'),
                            ('Order Of The Twin Adder'),
                            ('Immortal Flames');";
            public Handler(IDbConnection connection)
            {
                Connection = connection;
            }

            public IDbConnection Connection { get; }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                await Connection.ExecuteAsync(Sql);

                return Unit.Value;
            }
        }
    }
}
