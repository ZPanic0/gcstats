using System.Data;
using System.Threading.Tasks;
using MediatR;

namespace gcstats
{
    public class Application
    {
        public Application(IMediator mediator, IDbConnection connection)
        {
            Mediator = mediator;
        }

        public IMediator Mediator { get; }

        internal async Task Execute()
        {

        }
    }
}
