using Common.Enums;
using Common.Extensions;
using CQRS.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests.Unit
{
    public class SetsTests
    {
        [Fact]
        public void EmitsAllDatacenters()
        {
            var result = new Sets().Datacenters;
            var datacenterValues = (Datacenter[])Enum.GetValues(typeof(Datacenter));

            result
                .Should()
                .HaveCount(datacenterValues.Length - 1);

            datacenterValues
                .Except(result)
                .Should()
                .BeEquivalentTo(new[] { Datacenter.NoInput });
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void EmitsExpectedServers(Datacenter datacenter, IEnumerable<Server> server)
        {
            new Sets().Servers.Dictionary[datacenter]
                .Should()
                .BeEquivalentTo(server);
        }

        public static IEnumerable<object[]> TestData() => new object[][] {
            new object[] { Datacenter.Elemental, Datacenter.Elemental.GetServers() },
            new object[] { Datacenter.Gaia, Datacenter.Gaia.GetServers() },
            new object[] { Datacenter.Mana, Datacenter.Mana.GetServers() },
            new object[] { Datacenter.Aether, Datacenter.Aether.GetServers() },
            new object[] { Datacenter.Primal, Datacenter.Primal.GetServers() },
            new object[] { Datacenter.Crystal, Datacenter.Crystal.GetServers() },
            new object[] { Datacenter.Chaos, Datacenter.Chaos.GetServers() },
            new object[] { Datacenter.Light, Datacenter.Light.GetServers() }
        };

        [Fact]
        public void EmitsAllServers()
        {
            var result = new Sets().Servers.All;
            var serverValues = (Server[])Enum.GetValues(typeof(Server));

            result
                .Should()
                .HaveCount(serverValues.Length - 1);

            serverValues
                .Except(result)
                .Should()
                .BeEquivalentTo(new[] { Datacenter.NoInput });
        }

        [Fact]
        public void EmitsAllFactions()
        {
            var result = new Sets().Factions;
            var factionValues = (Faction[])Enum.GetValues(typeof(Faction));

            result
                .Should()
                .HaveCount(factionValues.Length - 1);

            factionValues
                .Except(result)
                .Should()
                .BeEquivalentTo(new[] { Datacenter.NoInput });
        }

        [Fact]
        public void EmitsExpectedPageCounts()
        {
            var result = new Sets().PageNumbers;

            result
                .Should()
                .HaveCount(5);

            result
                .Should()
                .BeEquivalentTo(new[] { 1, 2, 3, 4, 5 });
        }
    }
}
