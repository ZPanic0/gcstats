using Autofac;
using gcstats.Commands;
using gcstats.Common;
using gcstats.Configuration;
using MediatR;

namespace gcstats.Modules
{
    class WriteQueueModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(context => new RequestQueue<SavePageToZip.Request>(context.Resolve<IMediator>(), 10))
                .As<IWriteQueue<SavePageToZip.Request>>()
                .SingleInstance();

            builder
                .Register(context => new RequestQueue<SavePageReports.Request>(context.Resolve<IMediator>(), 10))
                .As<IWriteQueue<SavePageReports.Request>>()
                .SingleInstance();

            builder
                .Register(context => new RequestQueue<SavePageReport.Request>(context.Resolve<IMediator>(), 10))
                .As<IWriteQueue<SavePageReport.Request>>()
                .SingleInstance();
        }
    }
}
