namespace gcstats.Configuration
{
    public class AppSettings
    {
        public DatabaseSettings DatabaseSettings { get; set; }
        public string BaseDirectory { get; set; }
        public ArchiveSettings ArchiveSettings { get; set; }
        public string LodestoneUrlTemplate { get; set; }
        public Paths Paths { get; set; }
    }
}