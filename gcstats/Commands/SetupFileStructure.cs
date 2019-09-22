using gcstats.Common;
using gcstats.Configuration.Models;
using MediatR;
using System.IO;
using System.Linq;
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

            public Handler(AppSettings appSettings, ILogger logger, Sets sets)
            {
                this.appSettings = appSettings;
                this.logger = logger;
                this.sets = sets;
            }
            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                CreateDirectoryIfNotExists($"{appSettings.BaseDirectory}/pages/");
                CreateDirectoryIfNotExists($"{appSettings.BaseDirectory}/cache/");
                CreateDirectoryIfNotExists($"{appSettings.BaseDirectory}/out/");

                CreateProtobufCacheFiles();

                return Unit.Task;
            }

            private void CreateDirectoryIfNotExists(string directoryPath)
            {
                if (!Directory.Exists(directoryPath))
                {
                    logger.WriteLine($"Directory {directoryPath} not found. Creating...");
                    Directory.CreateDirectory(directoryPath);
                }
            }

            private void CreateProtobufCacheFiles()
            {
                foreach (var server in sets.Servers.Values.SelectMany(x => x))
                {
                    var filePath = string.Format(appSettings.ProtobufSettings.CachePathTemplate, appSettings.BaseDirectory, server);
                    if (!File.Exists(filePath))
                    {
                        logger.WriteLine($"File missing at {filePath}. Creating...");
                        using var fileStream = File.Create(filePath);
                    }
                }
            }
        }
    }
}
