using Autofac;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace gcstats.Modules
{
    public class HttpClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new HttpClient(new HttpClientHandler
            {
                MaxConnectionsPerServer = int.MaxValue
                
            }, true)).AsSelf().SingleInstance();
        }
    }
}