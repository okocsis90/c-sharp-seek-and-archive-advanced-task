using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SeekAndArchive
{
    class FileSearchEngine
    {
        private List<FileInfo> allPaths;
        public string FileName { get; set; }
        public string Path { get; set; }

        public FileSearchEngine(string fileName, string path)
        {
            FileName = fileName;
            Path = path;
        }

        public List<FileInfo> Search()
        {
            allPaths = new List<FileInfo>();
            SearchEngine(Path);
            return allPaths;
        }

        private void SearchEngine(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            if (!dirInfo.Exists)
            {
                Console.WriteLine("The specified directory does not exist.");
                return;
            }

            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                if (fileInfo.Name == FileName)
                {
                    allPaths.Add(fileInfo);
                }
            }
            foreach (DirectoryInfo currentDirInfo in dirInfo.GetDirectories())
            {
                SearchEngine(currentDirInfo.FullName);
            }
        }
    }
}
