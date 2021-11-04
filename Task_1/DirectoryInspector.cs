using System;
using System.IO;

namespace Task_1
{
    public static class DirectoryInspector
    {
        public static void ProcessEntity(DirectoryInfo item)
        {
            try
            {
                
                if (!item.Exists)
                {
                    throw new Exception($"There's no directory {item.FullName}");
                }

                var directories = item.GetDirectories();
                if (directories.Length != 0)
                {
                    DirectoryQueue.GetInstance().Enqueue(directories);   
                }
                var fileSystemInfos = item.GetFiles();
                if (fileSystemInfos.Length != 0)
                {
                    FileQueue.GetInstance().Enqueue(fileSystemInfos);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}