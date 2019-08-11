using Autofac;
using WebMarkupMin.Core;

namespace gcstats.Modules
{
    public class HtmlMinifierModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<HtmlMinifier>()
                .As<IMarkupMinifier>();
        }
    }
}
