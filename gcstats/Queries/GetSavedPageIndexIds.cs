using gcstats.Configuration;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class GetSavedPageIndexIds
    {
        public class Request : IRequest<IEnumerable<long>>
        {
            public Request(int tallyingPeriodId)
            {
                TallyingPeriodId = tallyingPeriodId;
            }

            public int TallyingPeriodId { get; }
        }

        public class Handler : IRequestHandler<Request, IEnumerable<long>>
        {
            private readonly AppSettings appSettings;

            public Handler(AppSettings appSettings)
            {
                this.appSettings = appSettings;
            }

            public Task<IEnumerable<long>> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(GetIndexIds(request.TallyingPeriodId));
            }

            private IEnumerable<long> GetIndexIds(int tallyingPeriodId)
            {
                var path = string.Format(
                    appSettings.ArchiveSettings.ArchivePathTemplate, 
                    appSettings.BaseDirectory, 
                    tallyingPeriodId);

                if (!File.Exists(path))
                {
                    return Array.Empty<long>();
                }

                using var zipFile = new FileStream(path, FileMode.Open);
                using var archive = new ZipArchive(zipFile, ZipArchiveMode.Read);
                return archive.Entries
                    .Select(x => long.Parse(x.Name.Split('.').First()))
                    .ToArray();
            }
        }
    }
}