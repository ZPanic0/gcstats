using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gcstats.Common;
using gcstats.Common.Enums;
using gcstats.Common.ProtobufModels;
using gcstats.Configuration.Models;
using gcstats.Queries;
using MediatR;
using ProtoBuf;

namespace gcstats.Commands
{
    public class BuildPlayerDataFilesFromCache
    {
        public class Request : IRequest { }

        public class Handler : IRequestHandler<Request>
        {
            private readonly AppSettings appSettings;
            private readonly IMediator mediator;
            private readonly Sets sets;

            public Handler(AppSettings appSettings, IMediator mediator, Sets sets)
            {
                this.appSettings = appSettings;
                this.mediator = mediator;
                this.sets = sets;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var playerDataStreams = GetPlayerDataStreams();
                var serverIndexStreams = GetIndexStreams();

                await AddToGroupStreams(playerDataStreams);
                ConsolidatePlayerRecords(playerDataStreams);
                BuildIndexFiles(playerDataStreams, serverIndexStreams);
                CloseAllStreams(
                    playerDataStreams.Select(x => x.Value)
                    .Concat(serverIndexStreams.Select(x => x.Value)));

                return Unit.Value;
            }

            private Dictionary<int, FileStream> GetPlayerDataStreams()
            {
                var streams = new Dictionary<int, FileStream>();

                for (var reportGroup = 0; reportGroup < 1000; reportGroup++)
                {
                    var filePath = string.Format(
                        appSettings.ProtobufSettings.PlayerGroupTemplate,
                        appSettings.BaseDirectory,
                        reportGroup);

                    streams.Add(reportGroup, File.Open(filePath, FileMode.Open));
                }

                return streams;
            }

            private Dictionary<Server, FileStream> GetIndexStreams()
            {
                var streams = new Dictionary<Server, FileStream>();

                foreach (var server in sets.Servers.All)
                {
                    var filePath = string.Format(appSettings.ProtobufSettings.IndexesTemplate, appSettings.BaseDirectory, server);

                    streams.Add(server, File.Open(filePath, FileMode.Open));
                }

                return streams;
            }

            private async Task AddToGroupStreams(Dictionary<int, FileStream> streams)
            {
                var tallyingPeriodIds = await mediator.Send(new GetTallyingPeriodIdsToCurrent.Request());

                foreach (var tallyingPeriodId in tallyingPeriodIds)
                {
                    foreach (var server in sets.Servers.All)
                    {
                        var reports = await mediator.Send(new GetCachedPageReports.Request(server, tallyingPeriodId));

                        var groups = reports
                            .AsParallel()
                            .Where(x => x.Players != null)
                            .SelectMany(x => x.Players)
                            .GroupBy(x => x.LodestoneId % 1000);

                        foreach (var reportGroup in groups)
                        {
                            Serializer.Serialize<IEnumerable<Player>>(streams[reportGroup.Key], reportGroup);
                        }
                    }
                }
            }

            private void ConsolidatePlayerRecords(Dictionary<int, FileStream> streams)
            {
                for (var reportGroup = 0; reportGroup < 1000; reportGroup++)
                {
                    var output = new List<Player>();

                    var fileStream = streams[reportGroup];

                    fileStream.Seek(0, SeekOrigin.Begin);

                    var players = Serializer.Deserialize<IEnumerable<Player>>(fileStream);

                    fileStream.SetLength(0);

                    foreach (var unmergedPlayerSet in players.GroupBy(player => player.LodestoneId))
                    {
                        var latestRecord = unmergedPlayerSet
                            .OrderByDescending(record => record.Performances.Max(performance => performance.IndexId))
                            .First();

                        latestRecord.Performances = unmergedPlayerSet
                            .SelectMany(x => x.Performances)
                            .ToList();

                        output.Add(latestRecord);
                    }

                    Serializer.Serialize(fileStream, output);
                }
            }

            private void BuildIndexFiles(Dictionary<int, FileStream> playerData, Dictionary<Server, FileStream> indexFiles)
            {
                var exportSet = new Index[1];

                for (var reportGroup = 0; reportGroup < 1000; reportGroup++)
                {
                    var fileStream = playerData[reportGroup];

                    fileStream.Seek(0, SeekOrigin.Begin);

                    foreach (var player in Serializer.Deserialize<IEnumerable<Player>>(fileStream))
                    {
                        exportSet[0] = new Index
                        {
                            LodestoneId = player.LodestoneId,
                            PlayerName = player.PlayerName,
                            Server = player.Server
                        };

                        Serializer.Serialize<IEnumerable<Index>>(indexFiles[player.Server], exportSet);
                    }
                }
            }

            private void CloseAllStreams(IEnumerable<FileStream> streams)
            {
                foreach (var stream in streams)
                {
                    stream.Close();
                }
            }
        }
    }
}
