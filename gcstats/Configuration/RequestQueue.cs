using gcstats.Common;
using MediatR;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace gcstats.Configuration
{
    class RequestQueue<T> : IWriteQueue<T> where T : IRequest
    {
        private readonly BufferBlock<T> block;
        private readonly IMediator mediator;

        public RequestQueue(IMediator mediator, int boundedCapacity)
        {
            this.mediator = mediator;
            block = new BufferBlock<T>(new DataflowBlockOptions { BoundedCapacity = boundedCapacity });
        }

        public async Task Start()
        {
            while (await block.OutputAvailableAsync())
            {
                await mediator.Send(block.Receive());
            }

            await block.Completion;
        }

        public void Enqueue(T request) => block.Post(request);

        public void Close() => block.Complete();

        public bool Any() => block.Count > 0;
    }
}
