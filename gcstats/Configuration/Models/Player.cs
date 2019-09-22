using gcstats.Common.Enums;
using ProtoBuf;
using System.Collections.Generic;

namespace gcstats.Configuration.Models
{
    [ProtoContract]
    public class Player
    {
        [ProtoMember(1)]
        public int LodestoneId { get; set; }
        [ProtoMember(2)]
        public string PlayerName { get; set; }
        [ProtoMember(3)]
        public string PortraitUrl { get; set; }
        [ProtoMember(4)]
        public Faction Faction { get; set; }
        [ProtoMember(5)]
        public FactionRank FactionRank { get; set; }
        [ProtoMember(6)]
        public Server Server { get; set; }
        [ProtoMember(7)]
        public List<Performance> Performances { get; set; }
    }
}
