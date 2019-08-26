using gcstats.Configuration;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class SavePagesToZip
    {
        public class Request : IRequest
        {
            public Request(IEnumerable<Tuple<long, string>> pages)
            {
                Pages = pages;
            }

            public IEnumerable<Tuple<long, string>> Pages { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            public Handler(AppSettings appSettings)
            {
                this.appSettings = appSettings;
            }

            private readonly AppSettings appSettings;

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                if (!request.Pages.Any())
                {
                    return Unit.Value;
                }

                await Task.WhenAll(request.Pages
                    .GroupBy(page => page.Item1 / 100000)
                    .Select(group => SaveFilesByTallyingPeriodId(group.Key, group.AsEnumerable())));

                return Unit.Value;
            }

            private async Task SaveFilesByTallyingPeriodId(long tallyingPeriodId, IEnumerable<Tuple<long, string>> pages)
            {
                using var file = File.Open(string.Format(
                        appSettings.ArchiveSettings.ArchivePathTemplate,
                        appSettings.BaseDirectory,
                        tallyingPeriodId),
                    FileMode.OpenOrCreate);
                using var archive = new ZipArchive(file, ZipArchiveMode.Update);
                foreach (var page in pages)
                {
                    var archiveEntry = archive.CreateEntry(string.Format(
                            appSettings.ArchiveSettings.FileNameTemplate, page.Item1),
                        CompressionLevel.Optimal);

                    using var writer = new StreamWriter(archiveEntry.Open());
                    await writer.WriteAsync(page.Item2);
                }
            }
        }
    }
}