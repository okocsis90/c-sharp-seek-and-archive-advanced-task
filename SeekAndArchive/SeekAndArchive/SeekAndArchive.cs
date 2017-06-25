using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekAndArchive
{
    class SeekAndArchive
    {
        static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        static List<DirectoryInfo> archiveDirs;
        static List<FileInfo> resultFiles;

        static void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            FileSystemWatcher senderWatcher = (FileSystemWatcher)sender;
            int index = watchers.IndexOf(senderWatcher, 0);
            ArchiveFile(archiveDirs[index], resultFiles[index]);
        }

        static void WatcherRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        static void ArchiveFile(DirectoryInfo archiveDir, FileInfo fileToArchive)
        {
            FileStream input = File.Open(fileToArchive.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            FileStream output = File.Create(archiveDir.FullName + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileToArchive.Name +".gz");
            GZipStream Compressor = new GZipStream(output, CompressionMode.Compress);
            int b = input.ReadByte();
            while (b != -1)
            {
                Compressor.WriteByte((byte)b);
                b = input.ReadByte();
            }
            Compressor.Close();
            input.Close();
            output.Close();
        }

        static void CreateWatchers()
        {
            foreach (FileInfo fileInfo in resultFiles)
            {
                FileSystemWatcher newWatcher = new FileSystemWatcher(fileInfo.DirectoryName, fileInfo.Name);
                newWatcher.Changed += new FileSystemEventHandler(WatcherChanged);
                newWatcher.Renamed += new RenamedEventHandler(WatcherRenamed);
                newWatcher.Deleted += new FileSystemEventHandler(WatcherChanged);
                newWatcher.EnableRaisingEvents = true;
                newWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
                watchers.Add(newWatcher);
            }
        }

        static void CreateArchiveDirs()
        {
            archiveDirs = new List<DirectoryInfo>();
            for (int i = 0; i < resultFiles.Count; i++)
            {
                archiveDirs.Add(Directory.CreateDirectory("archive" + i));
            }
        }

        static void Main(string[] args)
        {
            FileSearchEngine fileSearchEngine = new FileSearchEngine(args[0], args[1]);
            resultFiles = fileSearchEngine.Search();

            foreach (FileInfo fileInfo in resultFiles)
            {
                Console.WriteLine(fileInfo.FullName);
            }

            CreateWatchers();
            CreateArchiveDirs();

            Console.ReadKey();
        }
    }
}
