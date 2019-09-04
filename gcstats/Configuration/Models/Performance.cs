using gcstats.Common;
using ProtoBuf;

namespace gcstats.Configuration.Models
{
    [ProtoContract]
    public class Performance
    {
        [ProtoMember(1)]
        public int Rank { get; set; }
        [ProtoMember(2)]
        public int Score { get; set; }
        [ProtoMember(3)]
        public Faction Faction { get; set; }
        [ProtoMember(4)]
        public long IndexId { get; set; }

    }
}
