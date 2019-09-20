using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Configuration.Models;
using gcstats.Queries;
using MediatR;

namespace gcstats.Commands
{
    public class SavePageToZip
    {
        public class Request : IRequest
        {
            public Request(long indexId, string page)
            {
                IndexId = indexId;
                Page = page;
            }

            public long IndexId { get; }
            public string Page { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            public Handler(AppSettings appSettings, IMediator mediator)
            {
                this.appSettings = appSettings;
                this.mediator = mediator;
            }

            private readonly AppSettings appSettings;
            private readonly IMediator mediator;

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var data = await mediator.Send(new GetQueryDataFromIndex.Request(request.IndexId));

                using var file = File.Open(string.Format(
                        appSettings.ArchiveSettings.ArchivePathTemplate,
                        appSettings.BaseDirectory,
                        data.TallyingPeriodId),
                    FileMode.OpenOrCreate);

                using var archive = new ZipArchive(file, ZipArchiveMode.Update);

                var archiveEntry = archive.CreateEntry(string.Format(
                            appSettings.ArchiveSettings.FileNameTemplate, request.IndexId),
                        CompressionLevel.Optimal);

                using var writer = new StreamWriter(archiveEntry.Open());
                await writer.WriteAsync(request.Page);

                return Unit.Value;
            }
        }
    }
}
