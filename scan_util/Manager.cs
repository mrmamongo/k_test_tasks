using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace scan_util
{
    public class Manager
    {
        private readonly List<Task<CorruptedFileTypes>> _tasks = new ();
        private int _tasksStarted;
        private int _tasksHandled;

        public async Task AddWorker()
        {
            Interlocked.Increment(ref _tasksStarted);
            await Task.Run(async () =>
            {
                const short limit = 5;
                short f = 0;
                var inspector = new Inspector();
                Pair<bool, FileSystemInfo> entity = default;
                while (f < limit)
                {
                    if (EntityQueue.GetInstance().Dequeue(ref entity))
                    {
                        var inspection = inspector.Inspect(entity);
                        _tasks.Add(inspection);
                        ProcessedFilesDict.GetInstance().Add(await inspection);
                        await AddWorker();
                        return;
                    }

                    Thread.Sleep(50);
                    f++;
                }

                Interlocked.Increment(ref _tasksHandled);
            });
        }

        public void WaitForTasks()
        {
            if (_tasks.Count > _tasksHandled)
            {
                Thread.Sleep(50);
            }
        }
    }
}