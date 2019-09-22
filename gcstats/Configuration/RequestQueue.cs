using gcstats.Common;
using MediatR;
using System.Threading.Tasks;

namespace gcstats.Configuration
{
    class RequestQueue<T> : IWriteQueue<T> where T : IRequest
    {
        private readonly WorkBuffer<T> buffer;

        public RequestQueue(IMediator mediator)
        {
            buffer = new WorkBuffer<T>(request => mediator.Send(request));
        }

        public Task Start() => buffer.Start();

        public void Enqueue(T request) => buffer.Add(request);

        public void Close() => buffer.Close();
    }
}
