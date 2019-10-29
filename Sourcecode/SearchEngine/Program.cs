using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static List<String> GUIDLines = new List<string>();
        static void Main(string[] args)
        {
            //SearchDupliacteGUID();
            SeachProductIdInComplicatedFile();

        }

        static void SeachProductIdInComplicatedFile()
        {
            SeachEngineConfig config = new SeachEngineConfig()
            {
                StartSeachPattern = "Referentienummer Coolblue: ",
                EndSeachPattern = ";"
            };

            SeachEngine engine = new SeachEngine(config);
            Console.WriteLine($"Searching...");
            IEnumerable<SeachEngineMatch> result = engine.SearchInFile(@"C:\Users\alisson.resenderubim\Desktop\teste.txt");

            foreach(SeachEngineMatch match in result)
            {
                Console.WriteLine(match.FoundSetence);
            }
            Console.ReadLine();

        }

        static void SearchDupliacteGUID()
        {
            string startPath = @"C:\Dev\Delphi\Vanessa2\Source\";

            SeachEngineConfig config = new SeachEngineConfig()
            {
                EndSeachPattern = "}']",
                StartSeachPattern = "['{"
            };

            SeachEngineDirectoryConfig directoryConfig = new SeachEngineDirectoryConfig()
            {
                FileNamePattern = "*.pas",
                IgnoreFileNamePatterns = new string[] { "MSXML2_TLB.pas", "Finance.Payment.Eft.Adyen.COM.pas" },
                SearchInSubDirectories = true
            };

            SeachEngine engine = new SeachEngine(config);
            Console.WriteLine($"Searching duplicated GUID in {startPath}...");
            IEnumerable<SeachEngineMatch> result = engine.SearchInDirectory(startPath, directoryConfig);

            List<String> duplicateKeys = result.GroupBy(x => x.FoundSetence).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

            if (duplicateKeys.Count == 0)
            {
                Console.WriteLine("No duplicated GUID founded!");
            }
            else
            {
                foreach (string guid in duplicateKeys)
                {
                    Console.WriteLine($"{guid} in:");
                    foreach (SeachEngineMatch match in result)
                    {
                        if (match.FoundSetence == guid)
                            Console.WriteLine($"\t{match.FilePath}");
                    }
                    Console.WriteLine($"\n");
                }

                Console.Write($"[{duplicateKeys.Count}] duplicated GUID founded! Do you want to fix them? [Y/N]:");
                if (Console.ReadLine().ToUpper().Trim() == "Y")
                {
                    int fixCount = 0;
                    foreach (string guid in duplicateKeys)
                    {
                        Console.WriteLine($"Fixing...{guid}:");
                        foreach (SeachEngineMatch match in result)
                        {
                            try
                            {
                                if (match.FoundSetence == guid)
                                {
                                    Console.WriteLine($"\tFixing file " +
                                        $"{match.FilePath}");
                                    string text = File.ReadAllText(match.FilePath);
                                    //text = text.Replace(match.FoundSetence, Guid.NewGuid().ToString(), 1);
                                    text = text.Substring(0, match.FoundSetenceStartIndex) + Guid.NewGuid().ToString() + text.Substring(match.FoundSetenceEndIndex);
                                    File.WriteAllText(match.FilePath, text);
                                    fixCount++;
                                }
                            }
                            catch
                            {
                                Console.WriteLine($"\tError: Was not possible edit the file {match.FilePath}!!");
                            }
                        }
                        Console.WriteLine($"\n");
                    }
                    Console.WriteLine($"[{fixCount}] files was fixed with success!");
                }
            }
            Console.WriteLine("Press Enter to Exit.");
            Console.ReadLine();
        }
    }
}
