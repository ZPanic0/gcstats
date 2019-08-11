using gcstats.Commands;
using gcstats.Queries;
using MediatR;
using System.Linq;
using System.Threading.Tasks;

namespace gcstats
{
    public class Application
    {
        private readonly IMediator mediator;

        public Application(IMediator mediator)
        {
            this.mediator = mediator;
        }

        internal async Task Execute()
        {
            await mediator.Send(new SetupTables.Request());
            await mediator.Send(new GetAllMissingPages.Request
            {
                Callback = async (set) =>
                {
                    await mediator.Send(new SavePages.Request {
                        Pages = set.Select(x => new SavePage.Request(
                            x.Item1.TallyingPeriodId,
                            x.Item1.TimePeriod,
                            x.Item1.Server,
                            x.Item1.Faction,
                            x.Item1.Page,
                            x.Item2))
                    });
                },
                BatchSize = 18
            });
        }
    }
}