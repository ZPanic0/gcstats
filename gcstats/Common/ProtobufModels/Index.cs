using gcstats.Common.Enums;
using ProtoBuf;

namespace gcstats.Common.ProtobufModels
{
    [ProtoContract]
    public class Index
    {
        [ProtoMember(1)]
        public int LodestoneId { get; set; }
        [ProtoMember(2)]
        public string PlayerName { get; set; }
        public Server Server { get; set; }
    }
}
