using Autofac;
using gcstats.Configuration;
using Newtonsoft.Json;
using System.IO;

namespace gcstats.Modules
{
    class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(
                JsonConvert.DeserializeObject<AppSettings>(
                    File.ReadAllText($"{Directory.GetCurrentDirectory()}/appsettings.json")))
                .AsSelf()
                .SingleInstance();
        }
    }
}
