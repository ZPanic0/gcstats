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
            var directory = Directory.GetCurrentDirectory();

            var appSettings = JsonConvert.DeserializeObject<AppSettings>(
                    File.ReadAllText($"{directory}/appsettings.json"));

            appSettings.BaseDirectory = directory;

            builder
                .RegisterInstance(directory)
                .AsSelf()
                .SingleInstance();
        }
    }
}
