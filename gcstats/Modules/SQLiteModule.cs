using Autofac;
using gcstats.Configuration.Models;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace gcstats.Modules
{
    internal class SQLiteModule : Module
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

                    var connection = new SQLiteConnection(string.Format(
                        settings.ConnectionStringTemplate,
                        settings.FileName
                        ));

                    connection.Open();

                    Configure(connection, context.Resolve<DatabaseSettings>().Pragma);

                    return connection;
                })
                .AsSelf()
                .As<IDbConnection>()
                .SingleInstance();
        }

        private void Configure(SQLiteConnection connection, string pragma)
        {
            using SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = pragma;
            command.ExecuteNonQuery();
        }
    }
}