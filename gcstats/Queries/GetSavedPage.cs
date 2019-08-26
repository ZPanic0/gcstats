using gcstats.Configuration;
using MediatR;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetSavedPage
    {
        public class Request : IRequest<string>
        {
            public Request(long indexId)
            {
                IndexId = indexId;
            }

            public long IndexId { get; }
        }

        public class Handler : IRequestHandler<Request, string>
        {
            public Handler(AppSettings appSettings)
            {
                this.appSettings = appSettings;
            }

            private readonly AppSettings appSettings;

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                var path = string.Format(
                    appSettings.ArchiveSettings.ArchivePathTemplate,
                    appSettings.BaseDirectory,
                    request.IndexId / 100000);

                using var file = File.OpenRead(path);
                using var archive = new ZipArchive(file, ZipArchiveMode.Read);

                var fileName = string.Format(appSettings.ArchiveSettings.FileNameTemplate, request.IndexId);

                using var archiveEntry = new StreamReader(archive.Entries
                    .First(entry => entry.FullName == fileName)
                    .Open());

                return await archiveEntry.ReadToEndAsync();
            }
        }
    }
}