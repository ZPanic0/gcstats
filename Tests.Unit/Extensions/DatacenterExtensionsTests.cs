using Common.Enums;
using Common.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.Unit.Extensions
{
    public class DatacenterExtensionsTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void EmitsExpectedServers(Datacenter datacenter, Server[] expectedServers)
        {
            datacenter
                .GetServers()
                .Should()
                .BeEquivalentTo(expectedServers);
        }

        [Fact]
        public void ThrowsOnNoInput()
        {
            Datacenter.NoInput
                .Invoking(datacenter => datacenter.GetServers())
                .Should().Throw<ArgumentException>();
        }

        public static IEnumerable<object[]> TestData() => new object[][] {
            new object[] {
                Datacenter.Elemental,
                new[] {
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
                }
            },
            new object[] {
                Datacenter.Gaia,
                new[] {
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
                }
            },
            new object[] {
                Datacenter.Mana,
                new[] {
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
                }
            },
            new object[] {
                Datacenter.Aether,
                new[] {
                    Server.Adamantoise,
                    Server.Cactuar,
                    Server.Faerie,
                    Server.Gilgamesh,
                    Server.Jenova,
                    Server.Midgardsormr,
                    Server.Sargatanas,
                    Server.Siren
                }
            },
            new object[] {
                Datacenter.Primal,
                new[] {
                    Server.Behemoth,
                    Server.Excalibur,
                    Server.Exodus,
                    Server.Famfrit,
                    Server.Hyperion,
                    Server.Lamia,
                    Server.Leviathan,
                    Server.Ultros
                }
            },
            new object[] {
                Datacenter.Crystal,
                new[] {
                    Server.Balmung,
                    Server.Brynhildr,
                    Server.Coeurl,
                    Server.Diabolos,
                    Server.Goblin,
                    Server.Malboro,
                    Server.Mateus,
                    Server.Zalera
                }
            },
            new object[] {
                Datacenter.Chaos,
                new[] {
                    Server.Cerberus,
                    Server.Louisoix,
                    Server.Moogle,
                    Server.Omega,
                    Server.Ragnarok,
                    Server.Spriggan,
                }
            },
            new object[] {
                Datacenter.Light,
                new[] {
                    Server.Lich,
                    Server.Odin,
                    Server.Phoenix,
                    Server.Shiva,
                    Server.Twintania,
                    Server.Zodiark
                }
            }
        };
    }
}