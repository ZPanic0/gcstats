using Autofac;
using gcstats.Configuration.Models;
using Newtonsoft.Json;
using System.IO;

namespace gcstats.Modules
{
    class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var directory = Directory.GetCurrentDirectory();

            var appSettings = JsonConvert.DeserializeObject<AppSettings>(
                    File.ReadAllText($"{directory}/appsettings.json"));

            appSettings.BaseDirectory = directory;

            builder
                .RegisterInstance(appSettings)
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterInstance(appSettings.Paths)
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterInstance(appSettings.ProtobufSettings)
                .AsSelf()
                .SingleInstance();
        }
    }
}
