using gcstats.Common;
using gcstats.Configuration;
using MediatR;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class DownloadPage
    {
        public class Request : IRequest<string>
        {
            public Request(int tallyingPeriodId, TimePeriod timePeriod, Server server, Faction faction, int page)
            {
                TallyingPeriodId = tallyingPeriodId;
                TimePeriod = timePeriod;
                Server = server;
                Faction = faction;
                Page = page;
            }

            public int TallyingPeriodId { get; }
            public TimePeriod TimePeriod { get; }
            public Server Server { get; }
            public Faction Faction { get; }
            public int Page { get; }
        }

        public class Handler : IRequestHandler<Request, string>
        {
            private readonly HttpClient client;
            private readonly AppSettings appSettings;

            public Handler(HttpClient client, AppSettings appSettings)
            {
                this.client = client;
                this.appSettings = appSettings;
            }

            public Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                ValidateInput(request);

                return client.GetStringAsync(
                        string.Format(
                            appSettings.LodestoneUrlTemplate,
                            request.TimePeriod.ToString().ToLower(),
                            request.TallyingPeriodId,
                            request.Page,
                            (int)request.Faction,
                            request.Server));
            }

            private void ValidateInput(Request request)
            {
                string errorMessage = null;

                if (request.TimePeriod == default)
                {
                    errorMessage = "No TimePeriod configured.";
                }

                if (request.Page < 1 || request.Page > 5)
                {
                    errorMessage = "Invalid page or no page configured. Valid pages: 1 - 5.";
                }

                if (request.TallyingPeriodId == default)
                {
                    errorMessage = "No TallyingPeriodId configured.";
                }

                if (request.Server == default)
                {
                    errorMessage = "No Server configured.";
                }

                if (request.Faction == default)
                {
                    errorMessage = "No Faction configured.";
                }

                if (errorMessage != null)
                {
                    throw new ArgumentException(errorMessage);
                }
            }

        }
    }
}
