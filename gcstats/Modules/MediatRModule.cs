using Autofac;
using gcstats.Configuration;
using MediatR;

namespace gcstats.Modules
{
    class MediatRModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<ParallelPublishMediator>()
                .As<IMediator>()
                .SingleInstance();

            builder
                .Register<ServiceFactory>(context =>
                    context.Resolve<IComponentContext>().Resolve);

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AsImplementedInterfaces();
        }
    }
}
