namespace gcstats.Configuration.Models
{
    public class AppSettings
    {
        public string BaseDirectory { get; set; }
        public ArchiveSettings ArchiveSettings { get; set; }
        public ProtobufSettings ProtobufSettings { get; set; }
        public string LodestoneUrlTemplate { get; set; }
        public Paths Paths { get; set; }
    }
}