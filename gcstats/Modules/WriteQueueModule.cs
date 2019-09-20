using Autofac;
using gcstats.Commands;
using gcstats.Common;

namespace gcstats.Modules
{
    class WriteQueueModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<PageWriteQueue>()
                .As<IWriteQueue<SavePageToZip.Request>>();
        }
    }
}
