using Autofac;
using gcstats.Modules;
using System.Threading.Tasks;

namespace gcstats
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ComposeContainer().Resolve<Application>().Execute();
        }

        static IContainer ComposeContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<DefaultModule>();

            return builder.Build();
        }
    }
}
