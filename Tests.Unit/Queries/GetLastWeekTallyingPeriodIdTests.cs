using FluentAssertions;
using CQRS.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Unit.Queries
{
    public class GetLastWeekTallyingPeriodIdTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public async Task ReturnsExpectedId(DateTime now, int expectedId)
        {
            (await new GetLastWeekTallyingPeriodId.Handler()
                .Handle(new GetLastWeekTallyingPeriodId.Request(now), CancellationToken.None))
                .Should()
                .Be(expectedId);
        }

        public static IEnumerable<object[]> TestData => new object[][]
        {
            new object[] { new DateTime(2020, 4, 10), 202013 },
        }
        //Any given day of a week should return the previous week's id
        .Concat(Enumerable.Range(23, 6).Select(day => new object[] { new DateTime(2020, 3, day), 202011 })); 
    }
}
