using System;
using System.Collections.Generic;

namespace gcstats.Common
{
    public class PageCache
    {
        private readonly Dictionary<long, string> loadedFiles;

        public PageCache()
        {
            loadedFiles = new Dictionary<long, string>(2050);
        }

        public bool Contains(long indexId)
        {
            return loadedFiles.ContainsKey(indexId);
        }

        public string PopPage(long indexId)
        {
            var page = loadedFiles[indexId];

            loadedFiles.Remove(indexId);

            return page;
        }

        public void Load(IEnumerable<Tuple<long, string>> pages)
        {
            foreach (var page in pages)
            {
                Load(page);
            }
        }

        public void Load(Tuple<long, string> page)
        {
            if (!loadedFiles.ContainsKey(page.Item1))
            {
                loadedFiles.Add(page.Item1, page.Item2);
            }
        }
    }
}
