using Autofac;

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
            builder.RegisterModule<AutoMapperModule>();
            builder.RegisterModule<HtmlMinifierModule>();

            builder
                .RegisterType<Application>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
