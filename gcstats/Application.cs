using gcstats.Commands;
using gcstats.Common;
using gcstats.Configuration;
using gcstats.Configuration.Models;
using gcstats.Queries;
using MediatR;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace gcstats
{
    public class Application
    {
        private readonly IMediator mediator;
        private readonly AppSettings appSettings;
        private readonly PageCache pageCache;
        private readonly ILogger logger;

        public Application(IMediator mediator, AppSettings appSettings, PageCache pageCache, ILogger logger)
        {
            this.mediator = mediator;
            this.appSettings = appSettings;
            this.pageCache = pageCache;
            this.logger = logger;
        }

        internal async Task Execute()
        {
            await mediator.Send(new SetupFileStructure.Request());

            var tallyingPeriodIds = await mediator.Send(new GetTallyingPeriodIdsToCurrent.Request());

            foreach (var tallyingPeriodId in tallyingPeriodIds)
            {
                logger.WriteLine($"Processing TallyingPeriodId {tallyingPeriodId}...");

                var missingIndexIds = (await mediator.Send(new GetMissingIndexIds.Request(tallyingPeriodId))).ToArray();

                var filePath = string.Format(appSettings.ProtobufSettings.PathTemplate, Directory.GetCurrentDirectory(), tallyingPeriodId);

                if (!missingIndexIds.Any() && File.Exists(filePath))
                {
                    try
                    {
                        using var fileStream = File.OpenRead(filePath);

                        Serializer.Deserialize<IEnumerable<Player>>(fileStream);

                        logger.WriteLine($"Both zip archive and protobuf cache for TallyingPeriodId {tallyingPeriodId} appear correct. Skipping...");
                        continue;
                    }
                    catch (Exception)
                    {
                        logger.WriteLine($"Failed to deserialize protobuf cache file for TallyingPeriodId {tallyingPeriodId}");
                        logger.WriteLine("Deleting file and recreating...");
                    }
                }

                logger.WriteLine($"Loading existing indexId files...");
                await mediator.Send(new LoadFileToCache.Request(tallyingPeriodId));

                logger.WriteLine($"Retrieving missing indexId pages...");
                await mediator.Send(new GetPagesByIndexIds.Request(missingIndexIds));

                var results = pageCache.ToArray().Select(result => new Tuple<long, string>(result.Key, result.Value));
                pageCache.Clear();

                var savePagesTask = Task.CompletedTask;

                if (missingIndexIds.Any())
                {
                    logger.WriteLine("Saving pages...");
                    savePagesTask = mediator.Send(new SavePagesToZip.Request(results));
                }
                else
                {
                    logger.WriteLine("No new pages were downloaded. Skipping archive save.");
                }

                var modelTask = mediator.Send(new GetPlayerModels.Request(results));

                await Task.WhenAll(savePagesTask, modelTask);

                logger.WriteLine("Saving protobuf cache file...");
                await mediator.Send(new SavePlayerModelsToProtobufFile.Request(tallyingPeriodId, (await modelTask).PlayerResults));

                logger.WriteLine($"Done processing for TallyingPeriodId {tallyingPeriodId}\n");
            }
        }
    }
}