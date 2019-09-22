using Autofac;
using gcstats.Common;

namespace gcstats.Modules
{
    class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ConfigurationModule>();
            builder.RegisterModule<MediatRModule>();
            builder.RegisterModule<HtmlParserModule>();
            builder.RegisterModule<HttpClientModule>();
            builder.RegisterModule<HtmlMinifierModule>();
            builder.RegisterModule<LoggerModule>();
            builder.RegisterModule<WriteQueueModule>();

            builder
                .RegisterType<Sets>()
                .AsSelf();

            builder
                .RegisterType<Application>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
