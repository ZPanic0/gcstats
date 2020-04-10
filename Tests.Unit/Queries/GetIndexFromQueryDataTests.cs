using System.Threading.Tasks;
using Xunit;
using Request = CQRS.Queries.GetIndexFromQueryData.Request;
using Handler = CQRS.Queries.GetIndexFromQueryData.Handler;
using System.Threading;
using FluentAssertions;
using System.Collections.Generic;
using Common.Enums;

namespace Tests.Unit.Queries
{
    public class GetIndexFromQueryDataTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public async Task ReturnsExpectedId(Request request, long expectedId)
        {
            (await new Handler()
                .Handle(request, CancellationToken.None))
                .Should()
                .Be(expectedId);
        }

        public static IEnumerable<object[]> TestData => new object[][] {
            new object[] { new Request(201401, Server.Aegis, Faction.Maelstrom, 1), 2014010111 },
            new object[] { new Request(202013, Server.Zodiark, Faction.ImmortalFlames, 5), 2020136835 }
        };
    }
}
