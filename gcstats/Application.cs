using gcstats.Commands;
using gcstats.Common;
using gcstats.Configuration;
using gcstats.Configuration.Models;
using MediatR;
using System.Threading.Tasks;

namespace gcstats
{
    public class Application
    {
        private readonly IMediator mediator;
        private readonly AppSettings appSettings;
        private readonly IWriteQueue<SavePageToZip.Request> queue;
        private readonly ILogger logger;

        public Application(IMediator mediator, AppSettings appSettings, IWriteQueue<SavePageToZip.Request> queue, ILogger logger)
        {
            this.mediator = mediator;
            this.appSettings = appSettings;
            this.queue = queue;
            this.logger = logger;
        }

        internal async Task Execute()
        {
            await mediator.Send(new SetupFileStructure.Request());

            var queueTask = queue.Start();

            await mediator.Send(new FillMissingPagesToZip.Request());

            queue.Close();
            await queueTask;
        }
    }
}