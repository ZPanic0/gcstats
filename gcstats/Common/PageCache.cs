using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace gcstats.Common
{
    public class PageCache
    {
        private ConcurrentDictionary<long, string> loadedFiles;

        public PageCache()
        {
            loadedFiles = new ConcurrentDictionary<long, string>();
        }

        public bool Contains(long indexId)
        {
            return loadedFiles.ContainsKey(indexId);
        }

        public string PopPage(long indexId)
        {
            loadedFiles.Remove(indexId, out var page);

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
            Load(page.Item1, page.Item2);
        }

        public void Load(long indexId, string page)
        {
            if (!loadedFiles.ContainsKey(indexId))
            {
                loadedFiles.TryAdd(indexId, page);
            }
        }

        public IEnumerable<KeyValuePair<long, string>> ToArray()
        {
            return loadedFiles.ToArray();
        }

        public void Clear()
        {
            loadedFiles = new ConcurrentDictionary<long, string>();
        }
    }
}
