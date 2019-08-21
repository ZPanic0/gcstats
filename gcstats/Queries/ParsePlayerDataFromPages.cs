using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gcstats.Queries
{
    public static class ParsePlayerDataFromPages
    {
        public class Request : IRequest<Result>
        {
            public Request(IEnumerable<ParsePlayerDataFromPage.Request> requests)
            {
                Requests = requests;
            }

            public IEnumerable<ParsePlayerDataFromPage.Request> Requests { get; }
        }

        public class Result
        {
            public IEnumerable<ParsePlayerDataFromPage.Result> Results { get; set; }
        }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var tasks = new List<Task<ParsePlayerDataFromPage.Result[]>>();

                foreach (var item in request.Requests)
                {
                    tasks.Add(mediator.Send(item));
                }

                await Task.WhenAll(tasks);

                return new Result
                {
                    Results = tasks.SelectMany(task => task.Result)
                };
            }
        }
    }
}
