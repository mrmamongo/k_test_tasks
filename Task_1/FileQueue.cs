using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Task_1
{
    public class FileQueue
    {
        private static FileQueue _instance;
        private readonly object _locker = new();
        private readonly Queue<FileInfo> _itemQ = new();

        private FileQueue(){}

        public static FileQueue GetInstance()
        {
            return _instance ??= new FileQueue();
        }

        public void Enqueue(IEnumerable<FileInfo> items)
        {
            lock (_locker)
            {
                foreach (var item in items)
                {
                    _itemQ.Enqueue(item);
                    Monitor.Pulse(_locker);
                }
            }
        }

        public bool Dequeue(ref FileInfo item)
        {
            lock (_locker)
            {
                if (_itemQ.Count == 0)
                {
                    return false;
                }
                item = _itemQ.Dequeue();
                Monitor.Pulse(_locker);
            }

            return true;
        }
    }
}