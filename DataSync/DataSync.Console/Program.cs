using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSync
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<string> output = new List<string>(); 

            //Console.SetBufferSize(200, 50);
            //Console.SetWindowSize(200, 50);

            //foreach (string dir in GetDirectories(@"C:\Music"))
            //{
            //    Console.Clear();

            //    DrawChrome();
            //    if (dir.Length >= 200)
            //    {
            //        output.Add("..." + dir.Substring(50));
            //    }
            //    else
            //    {
            //        output.Add(dir);
            //    }


            //    if (output.Count >= 50)
            //    {
            //        output.RemoveAt(0);
            //    }

            //    output.ForEach(Console.WriteLine);
            //}

            //System.Console.ReadLine();

            foreach (var item in GetDirectories(@"C:\Temp"))
            {
                Console.WriteLine(item);
            }
            foreach (var item in GetFiles(@"C:\Temp"))
            {
                Console.WriteLine(item);
            }


            FileSystemWatcher fileSystemWatcherInstance = new FileSystemWatcher(@"C:\Temp")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.DirectoryName |
                               NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };

            fileSystemWatcherInstance.Changed += FileSystemWatcher_Changed;
            fileSystemWatcherInstance.Created += FileSystemWatcher_Created;
            fileSystemWatcherInstance.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcherInstance.Renamed += FileSystemWatcher_Renamed;

            fileSystemWatcherInstance.EnableRaisingEvents = true;

            Console.ReadLine();
        }

        private static void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                Console.WriteLine("Directory Renamed");
            }
            else
            {
                Console.WriteLine("File Renamed");
            }
        }

        private static void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                Console.WriteLine("Directory Deleted");
            }
            else
            {
                Console.WriteLine("File Deleted");
            }
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                Console.WriteLine("Directory Created");
            }
            else
            {
                Console.WriteLine("File Created");
            }
        }

        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                Console.WriteLine("Directory Changed");
            }
            else
            {
                Console.WriteLine("File Changed");
            }
        }

        public static List<string> GetDirectories(string path)
        {
            List<string> directories = new List<string>();

            foreach (var info in new DirectoryInfo(path).EnumerateDirectories())
            {
                directories.Add(info.FullName);
                directories.AddRange(GetDirectories(info.FullName));
            }

            return directories;
        }

        public static List<string> GetFiles(string path)
        {
            List<string> files = new List<string>();

            DirectoryInfo currentDirInfo = new DirectoryInfo(path);
            files.AddRange(currentDirInfo.EnumerateFiles().Select(fi => fi.FullName));

            foreach (var dirinfo in currentDirInfo.EnumerateDirectories())
            {
                //Go To Sub Directories
                files.AddRange(GetFiles(dirinfo.FullName));
            }

            return files;
        }


        private static void DrawChrome()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write('>');
            Console.WriteLine();

            for (int i = 0; i < 200; i++)
            {
                Console.Write('-');
            }

            Console.WriteLine();

            Console.SetCursorPosition(0, 3);
        }

    }
}
