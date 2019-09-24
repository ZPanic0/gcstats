using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Configuration.Models;
using MediatR;

namespace gcstats.Queries
{
    public class GetPageFromZip
    {
        public class Request : IRequest<string>
        {
            public Request(long indexId)
            {
                IndexId = indexId;
            }

            public long IndexId { get;}
        }

        public class Handler : IRequestHandler<Request, string>
        {
            private readonly AppSettings appSettings;
            private readonly IMediator mediator;

            public Handler(AppSettings appSettings, IMediator mediator)
            {
                this.appSettings = appSettings;
                this.mediator = mediator;
            }

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                var data = await mediator.Send(new GetQueryDataFromIndex.Request(request.IndexId));

                using var fileStream = File.OpenRead(string.Format(
                        appSettings.ArchiveSettings.ArchivePathTemplate,
                        appSettings.BaseDirectory,
                        data.TallyingPeriodId));

                using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);

                var archiveEntry = archive.GetEntry(
                    string.Format(
                        appSettings.ArchiveSettings.FileNameTemplate, 
                        request.IndexId));

                using var reader = new StreamReader(archiveEntry.Open());

                return reader.ReadToEnd();
            }
        }
    }
}
