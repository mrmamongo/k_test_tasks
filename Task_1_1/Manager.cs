using System.IO;
using System.Threading;

namespace Task_1_1
{
    public class Manager
    {
        private int _tasksStarted;
        private int _tasksHandled;
        public void AddWorker()
        {
            Interlocked.Increment(ref _tasksStarted);
            ThreadPool.QueueUserWorkItem(_ =>
            {
                const short limit = 5;
                short f = 0;
                var inspector = new Inspector();
                Pair<bool, FileSystemInfo> entity = default;
                while (f < limit)
                {
                    if (EntityQueue.GetInstance().Dequeue(ref entity))
                    {
                        ProcessedFilesDict.GetInstance().Add(inspector.Inspect(entity));
                        AddWorker();
                        return;
                    }

                    Thread.Sleep(500);
                    f++;
                }

                Interlocked.Increment(ref _tasksHandled);
            });
        }

        public void WaitForTasks()
        {
            if (_tasksStarted > _tasksHandled)
            {
                Thread.Sleep(500);
            }
        }
    }
}