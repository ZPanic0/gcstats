using System.Data;
using System.Data.SQLite;
using System.IO;
using Autofac;

namespace gcstats.Modules
{
    class SQLiteModule : Module
    {
        private const string FileName = "db.sqlite";
        private static readonly string ConnectionString = $"Data Source={FileName};Version=3";
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<IDbConnection>(context =>
            {
                if (!File.Exists(FileName))
                {
                    SQLiteConnection.CreateFile(FileName);
                }

                return new SQLiteConnection(ConnectionString);
            });
        }
    }
}
