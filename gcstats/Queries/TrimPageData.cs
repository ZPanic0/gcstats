using gcstats.Configuration;
using HtmlAgilityPack;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMarkupMin.Core;

namespace gcstats.Queries
{
    public static class TrimPageData
    {
        public class Request : IRequest<string>
        {
            public Request(string pageHtml)
            {
                PageHtml = pageHtml;
            }

            public string PageHtml { get; }
        }

        public class Handler : IRequestHandler<Request, string>
        {
            private readonly HtmlDocument document;
            private readonly Paths pathSettings;
            private readonly IMarkupMinifier minifier;

            public Handler(HtmlDocument document, Paths pathSettings, IMarkupMinifier minifier)
            {
                this.document = document;
                this.pathSettings = pathSettings;
                this.minifier = minifier;
            }
            public Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                document.LoadHtml(request.PageHtml);
                var tableHtml = document.DocumentNode.SelectSingleNode(pathSettings.Table);
                var minifiedResult = minifier.Minify(tableHtml.OuterHtml);

                if (!minifiedResult.Errors.Any())
                {
                    return Task.FromResult(minifiedResult.MinifiedContent);
                }

                throw new InvalidOperationException(
                    string.Join('\n', (
                        new[] { "Compression of request yielded one or more errors." })
                        .Concat(minifiedResult.Errors.Select(
                    error => $"Message: {error.Message}\n Line: {error.LineNumber} Column: {error.ColumnNumber}"))));
            }
        }
    }
}