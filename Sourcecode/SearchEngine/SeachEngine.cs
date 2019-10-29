using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public class SeachEngineConfig
    {
        public string StartSeachPattern { get; set; }
        public string EndSeachPattern { get; set; }
    }

    public class SeachEngineDirectoryConfig
    {
        public string FileNamePattern { get; set; }
        public string[] IgnoreFileNamePatterns { get; set; } = new string[] { };
        public bool SearchInSubDirectories { get; set; }
    }

    public class SeachEngineMatch
    {
        public string FoundSetence { get; set; }
        public int FoundSetenceStartIndex { get; set; }

        public int FoundSetenceEndIndex { get; set; }

        public string FoundFullSetence { get; set; }
        public int FoundFullSetenceStartIndex { get; set; }
        public int FoundFullSetenceEndIndex { get; set; }

        public string FilePath { get; set; }

        

        public SeachEngineMatch(string FoundSetence, int FoundSetenceStartIndex, int FoundSetenceEndIndex, string FoundFullSetence, int FoundFullSetenceStartIndex, int FoundFullSetenceEndIndex, string FilePath)
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

    public class SeachEngine
    {
        private SeachEngineConfig config;
        public SeachEngineConfig Config { get
            {
                return Config;
            }
        }

        public SeachEngine(SeachEngineConfig config)
        {
            this.config = config;
        }

        public IEnumerable<SeachEngineMatch> SearchInDirectory(string startPath, SeachEngineDirectoryConfig directoryConfig)
        {
            return processDirectory(startPath, directoryConfig);
        }

        public IEnumerable<SeachEngineMatch> SearchInFile(string filePath)
        {
            return processFile(filePath);
        }

        #region Private
        private IEnumerable<SeachEngineMatch> processFile(string path)
        {
            List<SeachEngineMatch> matches = new List<SeachEngineMatch>();
            string text = File.ReadAllText(path);

            int startSearchIndex = 0;
            int startSearchPatterIndex = text.IndexOf(config.StartSeachPattern, startSearchIndex);
            while (startSearchPatterIndex > -1)
            {
                int endSearchPatterIndex = text.IndexOf(config.EndSeachPattern, startSearchPatterIndex);
                if (endSearchPatterIndex < 0 || startSearchPatterIndex > endSearchPatterIndex)
                {
                    break;
                }
                else
                {
                    int fullFoundSetenceStartIndex = startSearchPatterIndex;
                    int fullFoundSetenceEndIndex = endSearchPatterIndex + config.EndSeachPattern.Length;
                    int substringLength = fullFoundSetenceEndIndex - fullFoundSetenceStartIndex;
                    string fullFoundText = text.Substring(fullFoundSetenceStartIndex, substringLength);


                    int foundSetenceStartIndex = startSearchPatterIndex + config.StartSeachPattern.Length;
                    int foundSetenceEndIndex = fullFoundSetenceEndIndex - config.EndSeachPattern.Length;
                    substringLength = foundSetenceEndIndex - foundSetenceStartIndex;
                    string foundSetence = text.Substring(foundSetenceStartIndex, substringLength);

                    matches.Add(new SeachEngineMatch(foundSetence, foundSetenceStartIndex, foundSetenceEndIndex, fullFoundText, fullFoundSetenceStartIndex, fullFoundSetenceEndIndex, path));
                    startSearchPatterIndex = text.IndexOf(config.StartSeachPattern, startSearchPatterIndex + 1);
                }

            }
            return matches;
        }
        private IEnumerable<SeachEngineMatch> processDirectory(string path, SeachEngineDirectoryConfig directoryConfig)
        {
            List<SeachEngineMatch> matches = new List<SeachEngineMatch>();

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
