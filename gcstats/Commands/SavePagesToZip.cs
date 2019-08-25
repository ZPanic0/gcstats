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
            private static readonly string directory = Directory.GetCurrentDirectory();
            private static readonly string archivePathTemplate = "{0}/pages/{1}.zip";
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
                using var file = File.Open(string.Format(archivePathTemplate, directory, tallyingPeriodId), FileMode.OpenOrCreate);
                using var archive = new ZipArchive(file, ZipArchiveMode.Update);
                foreach (var page in pages)
                {
                    var archiveEntry = archive.CreateEntry($"{page.Item1}.htm", CompressionLevel.Optimal);

                    using var writer = new StreamWriter(archiveEntry.Open());
                    await writer.WriteAsync(page.Item2);
                }
            }
        }
    }
}