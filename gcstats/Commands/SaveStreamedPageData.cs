using gcstats.Queries;
using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace gcstats.Commands
{
    public static class SaveStreamedPageData
    {
        public class Request : IRequest
        {
            public Request(Func<bool> isStreaming,
                           ConcurrentQueue<Tuple<long, string>> workQueue,
                           int sleepTime)
            {
                IsStreaming = isStreaming;
                WorkQueue = workQueue;
                SleepTime = sleepTime;
            }

            public Func<bool> IsStreaming { get; }
            public ConcurrentQueue<Tuple<long, string>> WorkQueue { get; }
            public int SleepTime { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var trimmedPages = new List<Tuple<long, string>>();
                int count;

                while (request.IsStreaming() || !request.WorkQueue.IsEmpty)
                {
                    trimmedPages.Clear();
                    count = 0;

                    while (request.WorkQueue.TryDequeue(out var result))
                    {
                        trimmedPages.Add(new Tuple<long, string>(
                            result.Item1, 
                            await mediator.Send(new TrimPageData.Request(result.Item2))));

                        count++;
                    }

                    await mediator.Send(new SavePagesToZip.Request(trimmedPages));

                    await Task.Delay(request.SleepTime);
                    Console.WriteLine($"Saved {count} pages. Sleeping for {request.SleepTime} ms.");
                }

                return Unit.Value;
            }
        }
    }
}