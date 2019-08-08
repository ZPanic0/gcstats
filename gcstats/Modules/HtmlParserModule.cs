using Autofac;
using HtmlAgilityPack;

namespace gcstats.Modules
{
    class HtmlParserModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HtmlDocument>().AsSelf();
        }
    }
}
