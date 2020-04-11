using CQRS.Common;
using FluentAssertions;
using MediatR;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Handler = CQRS.Queries.GetAllIndexIdsForTallyingPeriodId.Handler;
using Request = CQRS.Queries.GetAllIndexIdsForTallyingPeriodId.Request;

namespace Tests.Unit.Queries
{
    public class GetAllIndexIdsForTallyingPeriodIdTests
    {
        [Fact]
        public async Task EmitsExpectedIds()
        {
            var mediatorMock = new Mock<IMediator>();
            var sets = new Sets();
            var expectedCallCount = sets.Servers.All.Count() * sets.Factions.Count() * sets.PageNumbers.Count();

            mediatorMock
                .Setup(mediator => mediator.Send(It.IsAny<IRequest<long>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var results = (await new Handler(mediatorMock.Object, sets)
                .Handle(new Request(1), CancellationToken.None))
                .ToList();

            mediatorMock
                .Verify(mediator => mediator.Send(It.IsAny<IRequest<long>>(), It.IsAny<CancellationToken>()),
                    Times.Exactly(expectedCallCount));

            results.Count
                .Should()
                .Be(expectedCallCount);
        }
    }
}
