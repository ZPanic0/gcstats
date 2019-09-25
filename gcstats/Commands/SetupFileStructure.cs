using gcstats.Common;
using gcstats.Configuration.Models;
using gcstats.Queries;
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
            private readonly ILogger logger;
            private readonly Sets sets;
            private readonly IMediator mediator;

            public Handler(AppSettings appSettings, ILogger logger, Sets sets, IMediator mediator)
            {
                this.appSettings = appSettings;
                this.logger = logger;
                this.sets = sets;
                this.mediator = mediator;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                logger.WriteLine("Checking for missing files and folders and rebuilding...");
                Directory.CreateDirectory($"{appSettings.BaseDirectory}/pages/");
                Directory.CreateDirectory($"{appSettings.BaseDirectory}/cache/");
                Directory.CreateDirectory($"{appSettings.BaseDirectory}/out/");

                CreateProtobufCacheFolders();
                await CreateProtobufCacheFiles();

                logger.WriteLine("Done building file and folder structure.");

                return Unit.Value;
            }

            private void CreateProtobufCacheFolders()
            {
                foreach (var server in sets.Servers.All)
                {
                    Directory.CreateDirectory($"{appSettings.BaseDirectory}/cache/{server}/");
                }
            }

            private async Task CreateProtobufCacheFiles()
            {
                foreach (var server in sets.Servers.All)
                {
                    Directory.CreateDirectory($"{appSettings.BaseDirectory}/cache/{server}/");

                    foreach (var tallyingPeriodId in await mediator.Send(new GetTallyingPeriodIdsToCurrent.Request()))
                    {

                        var filePath = string.Format(
                            appSettings.ProtobufSettings.CachePathTemplate,
                            appSettings.BaseDirectory,
                            server,
                            tallyingPeriodId);

                        if (!File.Exists(filePath))
                        {
                            File.Create(filePath).Close();
                        }
                    }
                }
            }
        }
    }
}
