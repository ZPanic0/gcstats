using ProtoBuf;
using System.Collections.Generic;

namespace gcstats.Configuration.Models
{
    [ProtoContract]
    public class PageReport
    {
        [ProtoMember(1)]
        public long IndexId { get; set; }
        [ProtoMember(2)]
        public IEnumerable<Player> Players { get; set; }
    }
}
