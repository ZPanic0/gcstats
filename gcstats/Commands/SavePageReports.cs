using System.Collections.Generic;
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
    public class SavePageReports
    {
        public class Request : IRequest {
            public Request(IEnumerable<PageReport> reports, Server server, int tallyingPeriodId)
            {
                Reports = reports;
                Server = server;
                TallyingPeriodId = tallyingPeriodId;
            }

            public IEnumerable<PageReport> Reports { get; }
            public Server Server { get; }
            public int TallyingPeriodId { get; }
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
                    string.Format(
                        appSettings.ProtobufSettings.CachePathTemplate,
                        appSettings.BaseDirectory,
                        request.Server,
                        request.TallyingPeriodId),
                    FileMode.Append);

                Serializer.Serialize(fileStream, request.Reports);

                return Unit.Task;
            }
        }
    }
}
