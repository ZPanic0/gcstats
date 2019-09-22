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
                .RegisterType<RequestQueue<SavePageToZip.Request>>()
                .As<IWriteQueue<SavePageToZip.Request>>();

            builder
                .RegisterType<RequestQueue<SavePageReport.Request>>()
                .As<IWriteQueue<SavePageReport.Request>>();
        }
    }
}
