using gcstats.Commands;
using gcstats.Commands.Database;
using gcstats.Common;
using gcstats.Modules;
using gcstats.Queries;
using MediatR;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gcstats
{
    public class Application
    {
        private readonly IMediator mediator;

        public Application(IMediator mediator)
        {
            this.mediator = mediator;
        }

        internal async Task Execute()
        {
            await mediator.Send(new SetupTables.Request());
            await mediator.Send(new GetAllMissingPages.Request
            {
                Callback = (queue, getLockState) => mediator.Send(new SaveStreamedPageData.Request(getLockState, queue, 10000))
            }); 
        }
    }
}