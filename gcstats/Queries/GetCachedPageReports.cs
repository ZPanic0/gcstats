﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Common.Enums;
using gcstats.Common.ProtobufModels;
using gcstats.Configuration.Models;
using MediatR;
using ProtoBuf;

namespace gcstats.Queries
{
    public class GetCachedPageReports
    {
        public class Request : IRequest<IEnumerable<PageReport>> {
            public Request(Server server, int tallyingPeriodId)
            {
                Server = server;
                TallyingPeriodId = tallyingPeriodId;
            }

            public Server Server { get; }
            public int TallyingPeriodId { get; }
        }

        public class Handler : IRequestHandler<Request, IEnumerable<PageReport>>
        {
            private readonly AppSettings appSettings;

            public Handler(AppSettings appSettings)
            {
                this.appSettings = appSettings;
            }

            public Task<IEnumerable<PageReport>> Handle(Request request, CancellationToken cancellationToken)
            {
                using var fileStream = File.OpenRead(string.Format(
                        appSettings.ProtobufSettings.CachePathTemplate,
                        appSettings.BaseDirectory,
                        request.Server,
                        request.TallyingPeriodId));

                return Task.FromResult(Serializer.Deserialize<IEnumerable<PageReport>>(fileStream));
            }
        }
    }
}
