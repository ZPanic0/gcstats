using gcstats.Common;
using gcstats.Configuration.Models;
using MediatR;
using System;
using System.IO;
using System.IO.Compression;
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
            public Handler(AppSettings appSettings, PageCache pageCache)
            {
                this.appSettings = appSettings;
                this.pageCache = pageCache;
            }

            private readonly AppSettings appSettings;
            private readonly PageCache pageCache;

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                if (!pageCache.Contains(request.IndexId))
                {
                    await LoadFile(request);
                }

                return pageCache.PopPage(request.IndexId);
            }

            private async Task LoadFile(Request request)
            {
                var path = string.Format(
                    appSettings.ArchiveSettings.ArchivePathTemplate,
                    appSettings.BaseDirectory,
                    request.IndexId / 100000);

                using var file = File.OpenRead(path);
                using var archive = new ZipArchive(file, ZipArchiveMode.Read);

                foreach (var entry in archive.Entries)
                {
                    using var archiveEntry = new StreamReader(entry.Open());
                    pageCache.Load(new Tuple<long, string>(long.Parse(entry.Name.Split('.')[0]), await archiveEntry.ReadToEndAsync()));
                }
            }
        }
    }
}