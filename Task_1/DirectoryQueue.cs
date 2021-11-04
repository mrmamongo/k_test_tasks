using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Task_1
{
    public class DirectoryQueue
    {
        private static DirectoryQueue _instance;
        private readonly object _locker = new();
        private readonly Queue<DirectoryInfo> _itemQ = new();
        
        private DirectoryQueue() {}
        public static DirectoryQueue GetInstance()
        {
            return _instance ??= new DirectoryQueue();
        }
        
        public void Enqueue (DirectoryInfo item) 
        {
            lock (_locker)
            {
                _itemQ.Enqueue(item);
                Monitor.Pulse(_locker);
            }
        }

        public void Enqueue(IEnumerable<DirectoryInfo> items)
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

        public bool Dequeue(ref DirectoryInfo item)
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