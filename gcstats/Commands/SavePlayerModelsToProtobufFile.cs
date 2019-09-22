using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Common.ProtobufModels;
using gcstats.Configuration.Models;
using MediatR;
using ProtoBuf;

namespace gcstats.Commands
{
    public class SavePlayerModelsToProtobufFile
    {
        public class Request : IRequest {
            public Request(int tallyingPeriodId, IEnumerable<Player> players)
            {
                TallyingPeriodId = tallyingPeriodId;
                Players = players;
            }

            public int TallyingPeriodId { get; }
            public IEnumerable<Player> Players { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly AppSettings settings;

            public Handler(AppSettings settings)
            {
                this.settings = settings;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                using var fileStream = File.Open(
                    string.Format(settings.ProtobufSettings.OutputPathTemplate, settings.BaseDirectory, request.TallyingPeriodId),
                    FileMode.OpenOrCreate,
                    FileAccess.Write,
                    FileShare.None);

                Serializer.Serialize(fileStream, request.Players);

                return Unit.Value;
            }
        }
    }
}
