using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Task_1.CorruptedFiles;
using static Task_1.ProcessedFilesDict;

namespace Task_1
{
    internal static class Program
    {
        private static void DirectoryReaderFunc()
        {
            DirectoryInfo item = null;
            const ushort failLimit = 5;
            ushort fails = 0;

            while (fails < failLimit)
            {

                if (DirectoryQueue.GetInstance().Dequeue(ref item))
                {
                    fails = 0;
                    DirectoryInspector.ProcessEntity(item);
                }
                else
                {
                    fails++;
                    Thread.Sleep(500);
                }
            }
        }

        private static void FileReaderFunc()
        {
            FileInfo item = null;
            const ushort failLimit = 5;
            ushort fails = 0;
             
            while (fails < failLimit)
            {
                if (FileQueue.GetInstance().Dequeue(ref item))
                {
                    fails = 0;
                    FileInspector.ProcessEntity(item);
                }
                else
                {
                    fails++;
                    Thread.Sleep(500);
                }
            }
        }


        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Incorrect usage");
                Console.WriteLine("Usage: scan_util </path/to/dir>");
            }


            try
            {
                // Timer init
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                
                var info = new DirectoryInfo(args[0]);
                DirectoryQueue.GetInstance().Enqueue(info);

                var tasks = new []
                {
                    Task.Factory.StartNew(DirectoryReaderFunc, TaskCreationOptions.LongRunning),
                    Task.Factory.StartNew(FileReaderFunc, TaskCreationOptions.LongRunning)
                };

                Task.WaitAll(tasks);
                
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
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }
    }
}