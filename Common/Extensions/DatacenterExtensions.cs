using Common.Enums;
using System;
using System.Collections.Generic;

namespace Common.Extensions
{
    public static class DatacenterExtensions
    {
        public static IEnumerable<Server> GetServers(this Datacenter datacenter) => 
            Array.AsReadOnly(GetServerArray(datacenter));

        private static Server[] GetServerArray(Datacenter datacenter) => datacenter switch
        {
            Datacenter.Elemental => new[] {
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
            },
            Datacenter.Gaia => new[] {
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
            },
            Datacenter.Mana => new[] {
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
            },
            Datacenter.Aether => new[] {
                Server.Adamantoise,
                Server.Cactuar,
                Server.Faerie,
                Server.Gilgamesh,
                Server.Jenova,
                Server.Midgardsormr,
                Server.Sargatanas,
                Server.Siren
            },
            Datacenter.Primal => new[] {
                Server.Behemoth,
                Server.Excalibur,
                Server.Exodus,
                Server.Famfrit,
                Server.Hyperion,
                Server.Lamia,
                Server.Leviathan,
                Server.Ultros
            },
            Datacenter.Crystal => new[] {
                Server.Balmung,
                Server.Brynhildr,
                Server.Coeurl,
                Server.Diabolos,
                Server.Goblin,
                Server.Malboro,
                Server.Mateus,
                Server.Zalera
            },
            Datacenter.Chaos => new[] {
                Server.Cerberus,
                Server.Louisoix,
                Server.Moogle,
                Server.Omega,
                Server.Ragnarok,
                Server.Spriggan
            },
            Datacenter.Light => new[] {
                Server.Lich,
                Server.Odin,
                Server.Phoenix,
                Server.Shiva,
                Server.Twintania,
                Server.Zodiark
            },
            Datacenter.NoInput => throw new ArgumentException("No datacenter provided."),
            _ => throw new NotImplementedException(),
        };
    }
}
