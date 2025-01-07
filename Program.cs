using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LatestModifiedFolders
{
    class Program
    {
        static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        static DateTime FixFolder(string folder)
        {
            DirectoryInfo directory = new DirectoryInfo(folder);
            DateTime latest = new DateTime(epoch.Ticks);

            DirectoryInfo[] directories = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (DirectoryInfo child in directories)
            {
                DateTime stamp = FixFolder(Path.Combine(folder, child.Name));
                if (stamp.CompareTo(latest) > 0)
                    latest = stamp;
            }


            FileInfo[] files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo child in files)
            {
                // skip Thumbs.db
                if (child.Name == "Thumbs.db")
                    continue;
                else if (latest.CompareTo(epoch) <= 0)
                    latest = child.LastWriteTime;
                else if (child.LastWriteTime.CompareTo(latest) > 0)
                    latest = child.LastWriteTime;
            }

            // if empty, make sure modified is same as creation
            if (latest.CompareTo(epoch) <= 0)
                latest = directory.CreationTime;

            if (directory.LastWriteTime.CompareTo(latest) > 0)
            {
                directory.LastWriteTime = latest;
                Console.WriteLine("Setting {0} to {1}", folder, latest.ToString());
            }

            return latest;
        }

        static void Usage()
        {
            Console.WriteLine("Usage: {0} folder", Environment.CommandLine);
            Environment.Exit(1);
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length != 1)
            {
                Usage();
                return;
            }

            string folder = args[0];

            if (!Directory.Exists(folder))
                throw new Exception("Folder '" + folder + "' is invalid!");

            FixFolder(folder);
            Console.WriteLine("Done");
        }
    }
}
