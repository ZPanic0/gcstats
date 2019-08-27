using System.Data;
using System.Data.SQLite;
using System.IO;
using Autofac;
using gcstats.Configuration.Models;

namespace gcstats.Modules
{
    class SQLiteModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(context =>
                {
                    var settings = context.Resolve<AppSettings>().DatabaseSettings;
                    if (!File.Exists(settings.FileName))
                    {
                        SQLiteConnection.CreateFile(settings.FileName);
                    }

                    return new SQLiteConnection(string.Format(
                        settings.ConnectionStringTemplate,
                        settings.FileName
                        ));
                })
                .AsSelf()
                .As<IDbConnection>()
                .SingleInstance();
        }
    }
}
