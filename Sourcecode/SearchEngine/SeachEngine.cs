using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public class SeachEngineConfig
    {
        public string FileNamePattern { get; set; }
        public string StartSeachPattern { get; set; }
        public string EndSeachPattern { get; set; }
        public string[] IgnoreFilesPattern { get; set; } = new string[] { };
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

        public IEnumerable<SeachEngineMatch> Search(string startPath)
        {
            return process(startPath);
        }

        #region Private
        private IEnumerable<SeachEngineMatch> process(string path)
        {
            List<SeachEngineMatch> matches = new List<SeachEngineMatch>();

            if (config.SearchInSubDirectories)
                foreach (string dir in Directory.GetDirectories(path))
                    matches.AddRange(process(dir));

            foreach (string file in Directory.GetFiles(path, config.FileNamePattern))
            {
                if (config.IgnoreFilesPattern.Where(x => file.IndexOf(x) > -1).Count() == 0)
                {
                    string text = File.ReadAllText(file);

                    int startSearchIndex = 0;
                    int startSearchPatterIndex = text.IndexOf(config.StartSeachPattern, startSearchIndex);
                    while (startSearchPatterIndex > -1)
                    {
                        int endSearchPatterIndex = text.IndexOf(config.EndSeachPattern, startSearchPatterIndex);
                        if (endSearchPatterIndex < 0)
                        {
                            break;
                        }
                        else
                        {
                            int fullFoundSetenceStartIndex = startSearchPatterIndex;
                            int fullFoundSetenceEndIndex = (endSearchPatterIndex - startSearchPatterIndex) + config.EndSeachPattern.Length;
                            string fullFoundText = text.Substring(fullFoundSetenceStartIndex, fullFoundSetenceEndIndex);


                            int foundSetenceStartIndex = startSearchPatterIndex + config.StartSeachPattern.Length;
                            int foundSetenceEndIndex = (endSearchPatterIndex - startSearchPatterIndex - config.StartSeachPattern.Length);
                            string foundSetence = text.Substring(foundSetenceStartIndex, foundSetenceEndIndex);
                            matches.Add(new SeachEngineMatch(foundSetence, foundSetenceStartIndex, foundSetenceEndIndex, fullFoundText, fullFoundSetenceStartIndex, fullFoundSetenceEndIndex, file));
                            startSearchPatterIndex = text.IndexOf(config.StartSeachPattern, startSearchPatterIndex + 1);
                        }
                    }
                }
            }

            return matches;
        }
        #endregion
    }
}
