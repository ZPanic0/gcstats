using ProtoBuf;
using System.Collections.Generic;

namespace gcstats.Common.ProtobufModels
{
    [ProtoContract]
    public class PageReport
    {
        [ProtoMember(1)]
        public long IndexId { get; set; }
        [ProtoMember(2)]
        public List<Player> Players { get; set; }
    }
}
