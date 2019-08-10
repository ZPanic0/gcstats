using Autofac;
using System.Net.Http;

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

            builder
                .RegisterType<Application>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
