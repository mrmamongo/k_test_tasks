using System;
using System.Diagnostics;
using System.IO;
using static Task_1_1.CorruptedFileTypes;
using static Task_1_1.ProcessedFilesDict;

namespace Task_1_1
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Incorrect usage");
                Console.WriteLine("Usage: scan_util </path/to/dir>");
                return;
            }

            var firstEntry = new DirectoryInfo(args[0]);

            if (!firstEntry.Exists)
            {
                if (new FileInfo(firstEntry.FullName).Extension != string.Empty)
                {
                    Console.WriteLine($"Error: {firstEntry.FullName} is a file");
                    return;
                }
                Console.WriteLine($"Error: Directory {firstEntry.FullName} not found");
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            EntityQueue.GetInstance().Enqueue(new Pair<bool, FileSystemInfo>(true, firstEntry));
            var manager = new Manager();
            manager.AddWorker();
            manager.WaitForTasks();

            PrintSummary(stopwatch);
        }

        private static void PrintSummary(Stopwatch stopwatch)
        {
            Console.WriteLine("===== Scan Result =====");

            Console.WriteLine("Processed Files: {0}",
                GetInstance().Get(Sum));
            Console.WriteLine("{0} detects: {1}",
                JS, GetInstance().Get(JS));
            Console.WriteLine("{0} detects: {1}",
                rm_rf, GetInstance().Get(rm_rf));
            Console.WriteLine("{0} detects: {1}",
                Rundll32, GetInstance().Get(Rundll32));
            Console.WriteLine("{0}: {1}",
                Error, GetInstance().Get(Error));
            Console.WriteLine("Execution time: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            Console.WriteLine("========================");
        }
    }
}