using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Commands
{
    public static class SavePagesToZip
    {
        public class Request : IRequest
        {
            public Request(IEnumerable<Tuple<string, string>> pages)
            {
                Pages = pages;
            }

            public IEnumerable<Tuple<string, string>> Pages { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                using (var zipFile = new FileStream($"{Directory.GetCurrentDirectory()}/pages.zip", FileMode.OpenOrCreate))
                using (var archive = new ZipArchive(zipFile, ZipArchiveMode.Update))
                {
                    foreach (var page in request.Pages)
                    {
                        var archiveItem = archive.CreateEntry($"{page.Item1}.htm");
                        using (var writer = new StreamWriter(archiveItem.Open()))
                        {
                            await writer.WriteAsync(page.Item2);
                        }
                    }
                }

                return Unit.Value;
            }
        }
    }
}
