using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MovieFolderMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            string strBaseDirectory = Directory.GetCurrentDirectory();

            if (args != null && args.Length > 0)
            {
                switch (args[0])
                {
                    case "-d":
                    case "/d":

                        if (args.Length <= 1 || args[1] == null)
                        {
                            Console.WriteLine("Error. Must specify base directory after -d flag.");
                            return;
                        }

                        strBaseDirectory = args[1];

                        // Remove the trailing ", if it was preceded by \.
                        if (strBaseDirectory.EndsWith("\"")) strBaseDirectory = strBaseDirectory.Substring(0, strBaseDirectory.Length - 1);
                        // Remove the trailing slash, if it's provided.
                        if (strBaseDirectory.EndsWith("\\")) strBaseDirectory = strBaseDirectory.Substring(0, strBaseDirectory.Length - 1);
                        

                        break;

                    //case "--help":
                    //case "?":
                    //case "-?":
                    //case "/?":
                    //case "-help":
                    //case "help":
                    default:

                        Console.WriteLine("/*******************************************************");
                        Console.WriteLine("** MakeFolderMaker v0.1 by Jay Baldwin:");
                        Console.WriteLine("**");
                        Console.WriteLine("** Usage:\tMakeFolderMaker.exe -d {base_directory}");
                        Console.WriteLine("** Example:\tMakeFolderMaker.exe -d \"C:\\Media\\Movies\"");
                        Console.WriteLine("**");
                        Console.WriteLine("** Arguments:");
                        Console.WriteLine("** \t-?\tThis help output.");
                        Console.WriteLine("** \t-d\tBase directory to use. Note: if base directory is not defined, ");
                        Console.WriteLine("** \t\tuse current directory.");
                        Console.WriteLine("*******************************************************/");

                        return;
                        //break;

                }
            }

            if (!Directory.Exists(strBaseDirectory))
            {
                Console.WriteLine($@"Value for argument 1, ""{strBaseDirectory}"" is not a valid directory.  Exiting...");
                return;
            }

            Console.WriteLine($@"BASE DIRECTORY: ""{strBaseDirectory}""");

            string[] aryFolders = { "4k", "1080p", "720p", "DVD-Rip", "BR-Rip", "Screener" };

            foreach (string strFolder in aryFolders)
            {
                Console.WriteLine("");
                Console.WriteLine("");

                string path = strBaseDirectory;
                if (!Directory.Exists($@"{path}\{strFolder}"))
                {
                    Console.WriteLine($@"Skipping - ""{strBaseDirectory}\{strFolder}\""  doesn't exist.");
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
