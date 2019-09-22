using gcstats.Common.Enums;
using gcstats.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gcstats.Common
{
    public class Sets
    {
        public Sets()
        {
            Datacenters = ((Datacenter[])Enum.GetValues(typeof(Datacenter))).Skip(1).ToArray();
            Servers = BuildServersDictionary();
            Factions = ((Faction[])Enum.GetValues(typeof(Faction))).Skip(1).ToArray();
            PageNumbers = Enumerable.Range(1, 5).ToArray();
        }

        public IEnumerable<Datacenter> Datacenters { get; }
        public Dictionary<Datacenter, IEnumerable<Server>> Servers { get; }
        public IEnumerable<Faction> Factions { get; }
        public IEnumerable<int> PageNumbers { get; }

        private Dictionary<Datacenter, IEnumerable<Server>> BuildServersDictionary()
        {
            var dictionary = new Dictionary<Datacenter, IEnumerable<Server>>();

            foreach (var datacenter in Datacenters)
            {
                dictionary.Add(datacenter, datacenter.GetServers().ToArray());
            }

            return dictionary;
        }
    }
}
