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
        private static string _lastChangeElement; 

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


            FileSystemWatcher fileSystemWatcherInstance = new FileSystemWatcher(@"C:\Temp")
            {
                IncludeSubdirectories = true,
                NotifyFilter =  NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Attributes
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
            _lastChangeElement = string.Empty;

            if (Directory.Exists(e.FullPath))
            {
                Console.WriteLine("Directory Renamed - {0} {1}", e.FullPath, e.ChangeType.ToString("g"));
            }
            else
            {
                Console.WriteLine("File Renamed - {0} {1}", e.FullPath, e.ChangeType.ToString("g"));
            }
        }

        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!Directory.Exists(e.FullPath))
            {
                if (!e.FullPath.Equals(_lastChangeElement))
                {
                    _lastChangeElement = e.FullPath;
                    Console.WriteLine("File Changed - {0} {1}", e.FullPath, e.ChangeType.ToString("g"));
                }
                else
                {
                    _lastChangeElement = string.Empty;
                    Console.WriteLine("Double File Changed - {0} {1}", e.FullPath, e.ChangeType.ToString("g"));
                }
            }
            else
            {
                Console.WriteLine("Directory Changed - {0} {1}", e.FullPath, e.ChangeType.ToString("g"));
                
            }
        }

        private static void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            _lastChangeElement = string.Empty;

            if (String.IsNullOrEmpty(Path.GetExtension(e.FullPath))) //IMPORTANT!
            {
                Console.WriteLine("Directory Deleted - {0} {1}", e.FullPath, e.ChangeType.ToString("g"));
            }
            else
            {
                Console.WriteLine("File Deleted - {0} {1}", e.FullPath, e.ChangeType.ToString("g"));
            }
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            _lastChangeElement = string.Empty;

            if (Directory.Exists(e.FullPath))
            {
                Console.WriteLine("Directory Created - {0} {1}", e.FullPath, e.ChangeType.ToString("g"));
            }
            else
            {
                Console.WriteLine("File Created - {0} {1}", e.FullPath, e.ChangeType.ToString("g"));
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
