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

            builder
                .RegisterType<Application>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
