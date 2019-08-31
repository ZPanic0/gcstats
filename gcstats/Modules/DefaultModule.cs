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
            builder.RegisterModule<SQLiteModule>();
            builder.RegisterModule<HttpClientModule>();
            builder.RegisterModule<HtmlMinifierModule>();
            builder.RegisterModule<LoggerModule>();

            builder
                .RegisterType<PageCache>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<Application>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
