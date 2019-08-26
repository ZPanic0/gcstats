using gcstats.Configuration;
using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class SetupFileStructure
    {
        public class Request : IRequest { }

        public class Handler : IRequestHandler<Request>
        {
            private readonly AppSettings appSettings;

            public Handler(AppSettings appSettings)
            {
                this.appSettings = appSettings;
            }
            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                Directory.CreateDirectory($"{appSettings.BaseDirectory}/pages/");

                return Unit.Task;
            }
        }
    }
}
