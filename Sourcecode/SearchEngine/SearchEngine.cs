using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace SearchEngine
{
    public class SearchEngineConfig
    {
        public string StartSearchPattern { get; set; }
        public string EndSearchPattern { get; set; }
    }

    public class SearchEngineDirectoryConfig
    {
        public string FileNamePattern { get; set; }
        public string[] IgnoreFileNamePatterns { get; set; } = new string[] { };
        public bool SearchInSubDirectories { get; set; }
    }

    public class SearchEngineMatch
    {
        public string FoundSetence { get; set; }
        public int FoundSetenceStartIndex { get; set; }

        public int FoundSetenceEndIndex { get; set; }

        public string FoundFullSetence { get; set; }
        public int FoundFullSetenceStartIndex { get; set; }
        public int FoundFullSetenceEndIndex { get; set; }

        public string FilePath { get; set; }

        public SearchEngineMatch(string FoundSetence, int FoundSetenceStartIndex, int FoundSetenceEndIndex, string FoundFullSetence, int FoundFullSetenceStartIndex, int FoundFullSetenceEndIndex, string FilePath)
        {
            this.FoundSetence = FoundSetence;
            this.FoundSetenceStartIndex = FoundSetenceStartIndex;
            this.FoundSetenceEndIndex = FoundSetenceEndIndex;
            this.FoundFullSetence = FoundFullSetence;
            this.FoundFullSetenceStartIndex = FoundFullSetenceStartIndex;
            this.FoundFullSetenceEndIndex = FoundFullSetenceEndIndex;
            this.FilePath = FilePath;
        }
    }

    public class SearchEngine
    {
        private SearchEngineConfig config;
        public SearchEngineConfig Config
        {
            get
            {
                return Config;
            }
        }

        public SearchEngine(SearchEngineConfig config)
        {
            this.config = config;
        }

        public IEnumerable<SearchEngineMatch> SearchInDirectory(string startPath, SearchEngineDirectoryConfig directoryConfig)
        {
            return processDirectory(startPath, directoryConfig);
        }

        public IEnumerable<SearchEngineMatch> SearchInFile(string filePath)
        {
            return processFile(filePath);
        }

        #region Private
        private IEnumerable<SearchEngineMatch> processFile(string path)
        {
            List<SearchEngineMatch> matches = new List<SearchEngineMatch>();
            string text = File.ReadAllText(path);

            int startSearchIndex = 0;
            int startSearchPatterIndex = text.IndexOf(config.StartSearchPattern, startSearchIndex);
            while (startSearchPatterIndex > -1)
            {
                int endSearchPatterIndex = text.IndexOf(config.EndSearchPattern, startSearchPatterIndex);
                if (endSearchPatterIndex < 0 || startSearchPatterIndex > endSearchPatterIndex)
                {
                    break;
                }
                else
                {
                    int fullFoundSetenceStartIndex = startSearchPatterIndex;
                    int fullFoundSetenceEndIndex = endSearchPatterIndex + config.EndSearchPattern.Length;
                    int substringLength = fullFoundSetenceEndIndex - fullFoundSetenceStartIndex;
                    string fullFoundText = text.Substring(fullFoundSetenceStartIndex, substringLength);


                    int foundSetenceStartIndex = startSearchPatterIndex + config.StartSearchPattern.Length;
                    int foundSetenceEndIndex = fullFoundSetenceEndIndex - config.EndSearchPattern.Length;
                    substringLength = foundSetenceEndIndex - foundSetenceStartIndex;
                    string foundSetence = text.Substring(foundSetenceStartIndex, substringLength);

                    matches.Add(new SearchEngineMatch(foundSetence, foundSetenceStartIndex, foundSetenceEndIndex, fullFoundText, fullFoundSetenceStartIndex, fullFoundSetenceEndIndex, path));
                    startSearchPatterIndex = text.IndexOf(config.StartSearchPattern, startSearchPatterIndex + 1);
                }

            }
            return matches;
        }
        private IEnumerable<SearchEngineMatch> processDirectory(string path, SearchEngineDirectoryConfig directoryConfig)
        {
            List<SearchEngineMatch> matches = new List<SearchEngineMatch>();

            if (Directory.Exists(path))
            {
                if (directoryConfig.SearchInSubDirectories)
                    foreach (string dir in Directory.GetDirectories(path))
                        matches.AddRange(processDirectory(dir, directoryConfig));

                foreach (string file in Directory.GetFiles(path, directoryConfig.FileNamePattern))
                {
                    if (directoryConfig.IgnoreFileNamePatterns.Where(x => file.IndexOf(x) > -1).Count() == 0)
                    {
                        matches.AddRange(processFile(file));
                    }
                }
            }

            return matches;
        }
        #endregion
    }
}
