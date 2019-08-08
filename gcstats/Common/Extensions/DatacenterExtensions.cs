using System;
using System.Collections.Generic;

namespace gcstats.Common.Extensions
{
    public static class DatacenterExtensions
    {
        public static IEnumerable<Server> GetServers(this Datacenter datacenter)
        {
            switch (datacenter)
            {
                case Datacenter.Elemental:
                    return Array.AsReadOnly(new[]
                    {
                        Server.Aegis,
                        Server.Atomos,
                        Server.Carbuncle,
                        Server.Garuda,
                        Server.Gungnir,
                        Server.Kujata,
                        Server.Ramuh,
                        Server.Tonberry,
                        Server.Typhon,
                        Server.Unicorn
                    });
                case Datacenter.Gaia:
                    return Array.AsReadOnly(new[]
                    {
                        Server.Alexander,
                        Server.Bahamut,
                        Server.Durandal,
                        Server.Fenrir,
                        Server.Ifrit,
                        Server.Ridill,
                        Server.Tiamat,
                        Server.Ultima,
                        Server.Valefor,
                        Server.Yojimbo,
                        Server.Zeromus
                    });
                case Datacenter.Mana:
                    return Array.AsReadOnly(new[]
                    {
                        Server.Anima,
                        Server.Asura,
                        Server.Belias,
                        Server.Chocobo,
                        Server.Hades,
                        Server.Ixion,
                        Server.Mandragora,
                        Server.Masamune,
                        Server.Pandaemonium,
                        Server.Shinryu,
                        Server.Titan
                    });
                case Datacenter.Aether:
                    return Array.AsReadOnly(new[]
                    {
                        Server.Adamantoise,
                        Server.Cactuar,
                        Server.Faerie,
                        Server.Gilgamesh,
                        Server.Jenova,
                        Server.Midgardsormr,
                        Server.Sargatanas,
                        Server.Siren
                    });
                case Datacenter.Primal:
                    return Array.AsReadOnly(new[]
                    {
                        Server.Behemoth,
                        Server.Excalibur,
                        Server.Exodus,
                        Server.Famfrit,
                        Server.Hyperion,
                        Server.Lamia,
                        Server.Leviathan,
                        Server.Ultros
                    });
                case Datacenter.Crystal:
                    return Array.AsReadOnly(new[]
                    {
                        Server.Balmung,
                        Server.Brynhildr,
                        Server.Coeurl,
                        Server.Diabolos,
                        Server.Goblin,
                        Server.Malboro,
                        Server.Mateus,
                        Server.Zalera
                    });
                case Datacenter.Chaos:
                    return Array.AsReadOnly(new[]
                    {
                        Server.Cerberus,
                        Server.Louisoix,
                        Server.Moogle,
                        Server.Omega,
                        Server.Ragnarok,
                        Server.Spriggan
                    });
                case Datacenter.Light:
                    return Array.AsReadOnly(new[]
                    {
                        Server.Lich,
                        Server.Odin,
                        Server.Phoenix,
                        Server.Shiva,
                        Server.Twintania,
                        Server.Zodiark
                    });
                case Datacenter.NoInput:
                    throw new ArgumentException("No datacenter provided.");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
