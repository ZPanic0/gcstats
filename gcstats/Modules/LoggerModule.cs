using Autofac;
using gcstats.Configuration;

namespace gcstats.Modules
{
    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<ConsoleLogger>()
                .As<ILogger>();
        }
    }
}
