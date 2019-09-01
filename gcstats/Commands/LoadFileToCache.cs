using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Common;
using gcstats.Configuration.Models;
using MediatR;

namespace gcstats.Commands
{
    public class LoadFileToCache
    {
        public class Request : IRequest {
            public Request(int tallyingPeriodId)
            {
                TallyingPeriodId = tallyingPeriodId;
            }

            public int TallyingPeriodId { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly AppSettings appSettings;
            private readonly PageCache pageCache;

            public Handler(AppSettings appSettings, PageCache pageCache)
            {
                this.appSettings = appSettings;
                this.pageCache = pageCache;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var path = string.Format(
                    appSettings.ArchiveSettings.ArchivePathTemplate,
                    appSettings.BaseDirectory,
                    request.TallyingPeriodId);

                using var file = File.OpenRead(path);
                using var archive = new ZipArchive(file, ZipArchiveMode.Read);

                foreach (var entry in archive.Entries)
                {
                    using var archiveEntry = new StreamReader(entry.Open());
                    pageCache.Load(new Tuple<long, string>(long.Parse(entry.Name.Split('.')[0]), await archiveEntry.ReadToEndAsync()));
                }

                return Unit.Value;
            }
        }
    }
}
