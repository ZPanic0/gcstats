using gcstats.Commands;
using MediatR;
using System.Threading.Tasks;

namespace gcstats.Common
{
    public class PageWriteQueue : IWriteQueue<SavePageToZip.Request>
    {
        private readonly WorkBuffer<SavePageToZip.Request> buffer;

        public PageWriteQueue(IMediator mediator)
        {
            buffer = new WorkBuffer<SavePageToZip.Request>(request => mediator.Send(request));
        }

        public Task Start()
        {
            return buffer.Start();
        }

        public void Enqueue(SavePageToZip.Request request)
        {
            buffer.Add(request);
        }

        public void Close()
        {
            buffer.Close();
        }
    }
}
