using Autofac;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;

namespace gcstats.Modules
{
    public class HttpClientModule : Module
    {
        public static int ProxyCount => clientQueue.Count();
        private static readonly Queue<HttpClient> clientQueue;

        static HttpClientModule()
        {
            var proxyDetails = JsonConvert.DeserializeObject<IEnumerable<ProxyDetails>>(
                File.ReadAllText($"{Directory.GetCurrentDirectory()}/proxydetails.json"));

            clientQueue = new Queue<HttpClient>(proxyDetails
                .Select(x => new HttpClient(
                    new HttpClientHandler
                    {
                        Proxy = new WebProxy
                        {
                            Address = new Uri($"{x.Host}:{x.Port}"),
                            BypassProxyOnLocal = false,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(x.UserName, x.Password)
                        }
                    },
                    true)));
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(context =>
                    {
                        var client = clientQueue.Dequeue();
                        clientQueue.Enqueue(client);
                        return client;
                    })
                .ExternallyOwned();
        }
    }

    public class ProxyDetails
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
