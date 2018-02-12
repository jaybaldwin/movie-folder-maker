using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MovieFolderMaker
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] aryFolders = { "4k", "1080p", "720p", "DVD-Rip", "BR-Rip", "Screener" };

            foreach (string strFolder in aryFolders)
            {
                Console.WriteLine("");
                Console.WriteLine("");

                string path = Directory.GetCurrentDirectory();
                if (!Directory.Exists($@"{path}\{strFolder}"))
                {
                    Console.WriteLine($@"Skipping. \{strFolder}\ ... doesn't exist.");
                    continue;
                }

                path += $@"\{strFolder}";

                Console.WriteLine($@"Inspecting {path}");
                string[] files = Directory.GetFiles(path);

                foreach (string filename in files)
                {
                    Console.WriteLine("");
                    Console.WriteLine($@" ... parsing {filename}");

                    // if file like (.+\s\(\d{4}\))
                    Regex regex = new Regex(@"([^\\\n]*\s\(\d{4}\))");
                    bool blnMatch = regex.IsMatch(filename);

                    Console.WriteLine($@" ... ... is match? {blnMatch}");

                    if (!blnMatch)
                    {
                        Console.WriteLine(" ... Skipping.");
                        continue;
                    }

                    CaptureCollection groups = regex.Match(filename).Captures;

                    // ReSharper wanted this taken out: groups == null || 
                    if (groups.Count == 0)
                    {
                        Console.WriteLine(" ... No capture group found. Skipping.");
                        continue;
                    }

                    string strFolderToCreate = groups[0].Value;

                    Console.WriteLine($@" ... ... Folder should be ... ""{strFolderToCreate}""");

                    bool blnFolderExists = Directory.Exists($@"{path}\{strFolderToCreate}");

                    Console.WriteLine($@" ... ... ... Exists? ... {blnFolderExists}");

                    if (!blnFolderExists)
                    {
                        Console.WriteLine($@" ... ... ... Creating Directory ""{path}\{strFolderToCreate}""");
                        Directory.CreateDirectory($@"{path}\{strFolderToCreate}");
                    }

                    string strNewFileName = filename.Replace(path, $@"{path}\{strFolderToCreate}");

                    if (File.Exists(strNewFileName))
                    {
                        Console.WriteLine($@" ... ... TARGET FILE ALREADY EXISTS: ""{strNewFileName}"" - SKIPPING");
                        continue;
                    }

                    Console.WriteLine($@" ... ... Renaming file ""{filename}""");
                    Console.WriteLine($@" ... ... ... to ""{strNewFileName}""");

                    File.Move(filename, strNewFileName);

                    Console.WriteLine(@" ... Done.");

                    // Uncomment to stop after one file.
                    //break;
                }

                // Uncomment to stop after one directory.
                //break;

                Console.WriteLine(@"Done.");
            }
            
            Console.WriteLine(@"ALL COMPLETE!");
        }
    }
}
