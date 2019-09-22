using gcstats.Common;
using gcstats.Common.Enums;
using gcstats.Configuration.Models;
using MediatR;
using System;
using System.Net;
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
            public int Delay { get; set; } = 1000;
        }

        public class Handler : IRequestHandler<Request, string>
        {
            private const string exceptionMessageTemplate = "Exception thrown for {0}, {1}, {2}, {3}, Page {4}\nMessage: {5}\nRetrying in {6} ms...";
            private const string failedResponseMessageTemplate = "Request failed for {0}, {1}, {2}, {3}, Page {4}\nStatusCode: {5}\nRetrying in {6} ms...";
            private readonly AppSettings appSettings;
            private readonly HttpClient client;
            private readonly ILogger logger;

            public Handler(AppSettings appSettings, HttpClient client, ILogger logger)
            {
                this.appSettings = appSettings;
                this.client = client;
                this.logger = logger;
            }

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                ValidateInput(request);

                return await GetString(request, request.Delay);
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

            private async Task<string> GetString(Request request, int delay)
            {
                try
                {
                    var result = await client.GetAsync(string.Format(
                        appSettings.LodestoneUrlTemplate,
                        request.TimePeriod.ToString().ToLower(),
                        request.TallyingPeriodId,
                        request.Page,
                        (int)request.Faction,
                        request.Server));

                    if (!result.IsSuccessStatusCode)
                    {
                        logger.WriteLine(string.Format(
                            failedResponseMessageTemplate,
                            request.TallyingPeriodId,
                            request.TimePeriod,
                            request.Server,
                            request.Faction,
                            request.Page,
                            result.StatusCode,
                            delay));

                        await Task.Delay(delay);
                        return await GetString(request, delay * 2);
                    }

                    return await result.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    logger.WriteLine(string.Format(
                        exceptionMessageTemplate, 
                        request.TallyingPeriodId, 
                        request.TimePeriod, 
                        request.Server, 
                        request.Faction, 
                        request.Page, 
                        ex.Message,
                        delay));

                    await Task.Delay(delay);
                    return await GetString(request, delay * 2);
                }
            }
        }
    }
}