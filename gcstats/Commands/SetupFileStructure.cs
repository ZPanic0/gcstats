using gcstats.Configuration;
using gcstats.Configuration.Models;
using MediatR;
using System;
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
            private readonly ILogger logger;

            public Handler(AppSettings appSettings, ILogger logger)
            {
                this.appSettings = appSettings;
                this.logger = logger;
            }
            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var pagesDirectory = $"{appSettings.BaseDirectory}/pages/";
                if (!Directory.Exists(pagesDirectory))
                {
                    logger.WriteLine("Creating pages directory...");
                    Directory.CreateDirectory($"{appSettings.BaseDirectory}/pages/");
                }

                return Unit.Task;
            }
        }
    }
}
