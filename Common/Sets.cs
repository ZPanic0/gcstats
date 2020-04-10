using Common.Enums;
using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CQRS.Common
{
    public class Sets
    {
        public Sets()
        {
            Datacenters = Array.AsReadOnly(((Datacenter[])Enum.GetValues(typeof(Datacenter))).Skip(1).ToArray());
            Servers = BuildServers();
            Factions = Array.AsReadOnly(((Faction[])Enum.GetValues(typeof(Faction))).Skip(1).ToArray());
            PageNumbers = Array.AsReadOnly(Enumerable.Range(1, 5).ToArray());
        }

        public IEnumerable<Datacenter> Datacenters { get; }
        public (ReadOnlyDictionary<Datacenter, ReadOnlyCollection<Server>> Dictionary, ReadOnlyCollection<Server> All) Servers { get; }
        public IEnumerable<Faction> Factions { get; }
        public IEnumerable<int> PageNumbers { get; }

        private (ReadOnlyDictionary<Datacenter, ReadOnlyCollection<Server>> Dictionary, ReadOnlyCollection<Server> All) BuildServers()
        {
            var dictionary = Datacenters.ToDictionary(
                datacenter => datacenter, 
                datacenter => new ReadOnlyCollection<Server>(datacenter.GetServers().ToArray()));

            return (
                Dictionary: new ReadOnlyDictionary<Datacenter, ReadOnlyCollection<Server>>(dictionary), 
                All: Array.AsReadOnly(dictionary.Values.SelectMany(x => x).ToArray()));
        }
    }
}
