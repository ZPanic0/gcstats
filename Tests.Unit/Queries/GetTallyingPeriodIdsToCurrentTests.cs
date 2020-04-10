using FluentAssertions;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Handler = CQRS.Queries.GetTallyingPeriodIdsToCurrent.Handler;
using Request = CQRS.Queries.GetTallyingPeriodIdsToCurrent.Request;

namespace Tests.Unit.Queries
{
    public class GetTallyingPeriodIdsToCurrentTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public async Task ReturnsExpectedIds(int latestId, IEnumerable<int> expectedIds)
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(x => x.Send(It.IsAny<IRequest<int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(latestId);

            (await new Handler(mediatorMock.Object)
                .Handle(new Request(DateTime.Now), CancellationToken.None))
                .Should()
                .BeEquivalentTo(expectedIds);
        }

        public static IEnumerable<object[]> TestData => new object[][] {
            //Exercising unique week counting logic of the lodestone
            new object[] {
                202013,
                Enumerable.Range(1, 52).Select(week => 201400 + week)
                .Concat(Enumerable.Range(1, 52).Select(week => 201500 + week))
                .Concat(Enumerable.Range(1, 52).Select(week => 201600 + week))
                .Concat(Enumerable.Range(1, 52).Select(week => 201700 + week))
                .Concat(Enumerable.Range(1, 53).Select(week => 201800 + week))
                .Concat(Enumerable.Range(1, 52).Select(week => 201900 + week))
                .Concat(Enumerable.Range(1, 13).Select(week => 202000 + week))
            }
        };
    }
}
