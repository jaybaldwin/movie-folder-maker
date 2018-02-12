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

            foreach (string str in aryFolders)
            {
                Console.WriteLine("");
                Console.WriteLine("");

                string path = Directory.GetCurrentDirectory();
                if (!Directory.Exists(String.Format(@"{0}\{1}", path, str)))
                {
                    Console.WriteLine(String.Format(@"Skipping. \{0}\ ... doesn't exist.", str));
                    continue;
                }

                path += String.Format(@"\{0}", str);

                Console.WriteLine(String.Format(@"Inspecting {0}", path));
                string[] files = Directory.GetFiles(path);

                foreach (string filename in files)
                {
                    Console.WriteLine("");
                    Console.WriteLine(String.Format(@" ... parsing {0}", filename));

                    // if file like (.+\s\(\d{4}\))
                    Regex regex = new Regex(@"([^\\\n]*\s\(\d{4}\))");
                    bool blnMatch = regex.IsMatch(filename);

                    Console.WriteLine(String.Format(@" ... ... is match? {0}", blnMatch));

                    if (!blnMatch)
                    {
                        Console.WriteLine(" ... Skipping.");
                        continue;
                    }

                    CaptureCollection groups = regex.Match(filename).Captures;

                    if (groups == null || groups.Count == 0)
                    {
                        Console.WriteLine(" ... No capture group found. Skipping.");
                        continue;
                    }

                    string strFolderToCreate = groups[0].Value;

                    Console.WriteLine(String.Format(@" ... ... Folder should be ... ""{0}""", strFolderToCreate));

                    bool blnFolderExists = Directory.Exists(String.Format(@"{0}\{1}", path, strFolderToCreate));

                    Console.WriteLine(String.Format(@" ... ... ... Exists? ... {0}", blnFolderExists));

                    if (!blnFolderExists)
                    {
                        Console.WriteLine(String.Format(@" ... ... ... Creating Directory ""{0}\{1}""", path, strFolderToCreate));
                        Directory.CreateDirectory(String.Format(@"{0}\{1}", path, strFolderToCreate));
                    }

                    string strNewFileName = filename.Replace(path, String.Format(@"{0}\{1}", path, strFolderToCreate));

                    if (File.Exists(strNewFileName))
                    {
                        Console.WriteLine(String.Format(@" ... ... TARGET FILE ALREADY EXISTS: ""{0}"" - SKIPPING", strNewFileName));
                        continue;
                    }

                    Console.WriteLine(String.Format(@" ... ... Renaming file ""{0}""", filename));
                    Console.WriteLine(String.Format(@" ... ... ... to ""{0}""", strNewFileName));

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
