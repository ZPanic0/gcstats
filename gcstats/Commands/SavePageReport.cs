using System.IO;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Common.ProtobufModels;
using gcstats.Configuration.Models;
using gcstats.Queries;
using MediatR;
using ProtoBuf;

namespace gcstats.Commands
{
    public class SavePageReport
    {
        public class Request : IRequest
        {
            public Request(PageReport report)
            {
                Report = report;
            }

            public PageReport Report { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly AppSettings appSettings;
            private readonly IMediator mediator;

            public Handler(AppSettings appSettings, IMediator mediator)
            {
                this.appSettings = appSettings;
                this.mediator = mediator;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var data = await mediator.Send(new GetQueryDataFromIndex.Request(request.Report.IndexId));

                using var fileStream = File.Open(
                    string.Format(
                        appSettings.ProtobufSettings.CachePathTemplate, 
                        appSettings.BaseDirectory, 
                        data.Server,
                        data.TallyingPeriodId), 
                    FileMode.Append);

                Serializer.Serialize(fileStream, new[] { request.Report });

                return Unit.Value;
            }
        }
    }
}
