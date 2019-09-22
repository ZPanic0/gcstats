using System.IO;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Common.Enums;
using gcstats.Common.ProtobufModels;
using gcstats.Configuration.Models;
using MediatR;
using ProtoBuf;

namespace gcstats.Commands
{
    public class SavePageReport
    {
        public class Request : IRequest
        {
            public Request(PageReport report, Server server)
            {
                Report = report;
                Server = server;
            }

            public PageReport Report { get; }
            public Server Server { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly AppSettings appSettings;

            public Handler(AppSettings appSettings)
            {
                this.appSettings = appSettings;
            }

            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                using var fileStream = File.Open(
                    string.Format(appSettings.ProtobufSettings.CachePathTemplate, appSettings.BaseDirectory, request.Server), 
                    FileMode.Append);

                Serializer.Serialize(fileStream, request.Report);

                return Unit.Task;
            }
        }
    }
}
