using Common.Enums;
using Common.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.Unit.Extensions
{
    public class ServerExtensionsTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void EmitsExpectedDatacenter(Server server, Datacenter expectedDatacenter)
        {
            server
                .GetDatacenter()
                .Should()
                .Be(expectedDatacenter);
        }

        [Fact]
        public void ThrowsOnNoInput()
        {
            Server.NoInput
                .Invoking(server => server.GetDatacenter())
                .Should().Throw<ArgumentException>();
        }

        public static IEnumerable<object[]> TestData() => new object[][]
        {
            //Elemental
            new object[] { Server.Aegis, Datacenter.Elemental },
            new object[] { Server.Atomos, Datacenter.Elemental },
            new object[] { Server.Carbuncle, Datacenter.Elemental },
            new object[] { Server.Garuda, Datacenter.Elemental },
            new object[] { Server.Gungnir, Datacenter.Elemental },
            new object[] { Server.Kujata, Datacenter.Elemental },
            new object[] { Server.Ramuh, Datacenter.Elemental },
            new object[] { Server.Tonberry, Datacenter.Elemental },
            new object[] { Server.Typhon, Datacenter.Elemental },
            new object[] { Server.Unicorn, Datacenter.Elemental },

            //Gaia
            new object[] { Server.Alexander, Datacenter.Gaia },
            new object[] { Server.Bahamut, Datacenter.Gaia },
            new object[] { Server.Durandal, Datacenter.Gaia },
            new object[] { Server.Fenrir, Datacenter.Gaia },
            new object[] { Server.Ifrit, Datacenter.Gaia },
            new object[] { Server.Ridill, Datacenter.Gaia },
            new object[] { Server.Tiamat, Datacenter.Gaia },
            new object[] { Server.Ultima, Datacenter.Gaia },
            new object[] { Server.Valefor, Datacenter.Gaia },
            new object[] { Server.Yojimbo, Datacenter.Gaia },
            new object[] { Server.Zeromus, Datacenter.Gaia },

            //Mana
            new object[] { Server.Anima, Datacenter.Mana },
            new object[] { Server.Asura, Datacenter.Mana },
            new object[] { Server.Belias, Datacenter.Mana },
            new object[] { Server.Chocobo, Datacenter.Mana },
            new object[] { Server.Hades, Datacenter.Mana },
            new object[] { Server.Ixion, Datacenter.Mana },
            new object[] { Server.Mandragora, Datacenter.Mana },
            new object[] { Server.Masamune, Datacenter.Mana },
            new object[] { Server.Pandaemonium, Datacenter.Mana },
            new object[] { Server.Shinryu, Datacenter.Mana },
            new object[] { Server.Titan, Datacenter.Mana },

            //Aether
            new object[] { Server.Adamantoise, Datacenter.Aether },
            new object[] { Server.Cactuar, Datacenter.Aether },
            new object[] { Server.Faerie, Datacenter.Aether },
            new object[] { Server.Gilgamesh, Datacenter.Aether },
            new object[] { Server.Jenova, Datacenter.Aether },
            new object[] { Server.Midgardsormr, Datacenter.Aether },
            new object[] { Server.Sargatanas, Datacenter.Aether },
            new object[] { Server.Siren, Datacenter.Aether },

            //Primal
            new object[] { Server.Behemoth, Datacenter.Primal },
            new object[] { Server.Excalibur, Datacenter.Primal },
            new object[] { Server.Exodus, Datacenter.Primal },
            new object[] { Server.Famfrit, Datacenter.Primal },
            new object[] { Server.Hyperion, Datacenter.Primal },
            new object[] { Server.Lamia, Datacenter.Primal },
            new object[] { Server.Leviathan, Datacenter.Primal },
            new object[] { Server.Ultros, Datacenter.Primal },

            //Crystal
            new object[] { Server.Balmung, Datacenter.Crystal },
            new object[] { Server.Brynhildr, Datacenter.Crystal },
            new object[] { Server.Coeurl, Datacenter.Crystal },
            new object[] { Server.Diabolos, Datacenter.Crystal },
            new object[] { Server.Goblin, Datacenter.Crystal },
            new object[] { Server.Malboro, Datacenter.Crystal },
            new object[] { Server.Mateus, Datacenter.Crystal },
            new object[] { Server.Zalera, Datacenter.Crystal },

            //Chaos
            new object[] { Server.Cerberus, Datacenter.Chaos },
            new object[] { Server.Louisoix, Datacenter.Chaos },
            new object[] { Server.Moogle, Datacenter.Chaos },
            new object[] { Server.Omega, Datacenter.Chaos },
            new object[] { Server.Ragnarok, Datacenter.Chaos },
            new object[] { Server.Spriggan, Datacenter.Chaos },

            //Light
            new object[] { Server.Lich, Datacenter.Light },
            new object[] { Server.Odin, Datacenter.Light },
            new object[] { Server.Phoenix, Datacenter.Light },
            new object[] { Server.Shiva, Datacenter.Light },
            new object[] { Server.Twintania, Datacenter.Light },
            new object[] { Server.Zodiark, Datacenter.Light }
        };
    }
}
