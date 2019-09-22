using gcstats.Common.Enums;
using System;

namespace gcstats.Common.Extensions
{
    public static class ServerExtensions
    {
        public static Datacenter GetDatacenter(this Server server)
        {
            switch (server)
            {
                case Server.Aegis:
                case Server.Atomos:
                case Server.Carbuncle:
                case Server.Garuda:
                case Server.Gungnir:
                case Server.Kujata:
                case Server.Ramuh:
                case Server.Tonberry:
                case Server.Typhon:
                case Server.Unicorn:
                    return Datacenter.Elemental;
                case Server.Alexander:
                case Server.Bahamut:
                case Server.Durandal:
                case Server.Fenrir:
                case Server.Ifrit:
                case Server.Ridill:
                case Server.Tiamat:
                case Server.Ultima:
                case Server.Valefor:
                case Server.Yojimbo:
                case Server.Zeromus:
                    return Datacenter.Gaia;
                case Server.Anima:
                case Server.Asura:
                case Server.Belias:
                case Server.Chocobo:
                case Server.Hades:
                case Server.Ixion:
                case Server.Mandragora:
                case Server.Masamune:
                case Server.Pandaemonium:
                case Server.Shinryu:
                case Server.Titan:
                    return Datacenter.Mana;
                case Server.Adamantoise:
                case Server.Cactuar:
                case Server.Faerie:
                case Server.Gilgamesh:
                case Server.Jenova:
                case Server.Midgardsormr:
                case Server.Sargatanas:
                case Server.Siren:
                    return Datacenter.Aether;
                case Server.Behemoth:
                case Server.Excalibur:
                case Server.Exodus:
                case Server.Famfrit:
                case Server.Hyperion:
                case Server.Lamia:
                case Server.Leviathan:
                case Server.Ultros:
                    return Datacenter.Primal;
                case Server.Balmung:
                case Server.Brynhildr:
                case Server.Coeurl:
                case Server.Diabolos:
                case Server.Goblin:
                case Server.Malboro:
                case Server.Mateus:
                case Server.Zalera:
                    return Datacenter.Crystal;
                case Server.Cerberus:
                case Server.Louisoix:
                case Server.Moogle:
                case Server.Omega:
                case Server.Ragnarok:
                case Server.Spriggan:
                    return Datacenter.Chaos;
                case Server.Lich:
                case Server.Odin:
                case Server.Phoenix:
                case Server.Shiva:
                case Server.Twintania:
                case Server.Zodiark:
                    return Datacenter.Light;
                case Server.NoInput:
                    throw new ArgumentException("No server provided.");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
