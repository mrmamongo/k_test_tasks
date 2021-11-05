using System.Collections.Generic;
using System.IO;

namespace Task_1_1
{
    public class EntityQueue
    {
        private static EntityQueue _instance;
        private readonly object _locker = new();
        private readonly Queue<Pair<bool, FileSystemInfo>> _itemQ = new(); // bool: Dir - true, File - false
        
        private EntityQueue() {}
        
        public static EntityQueue GetInstance()
        {
            return _instance ??= new EntityQueue();
        }
        
        public void Enqueue (Pair<bool, FileSystemInfo> item) 
        {
            lock (_locker)
            {
                _itemQ.Enqueue(item);
            }
        }

        public void Enqueue(IEnumerable<Pair<bool, FileSystemInfo>> items)
        {
            lock (_locker)
            {
                foreach (var item in items)
                {
                    _itemQ.Enqueue(item);
                }
            }
        }

        public bool Dequeue(ref Pair<bool, FileSystemInfo> item)
        {
            lock (_locker)
            {
                if (_itemQ.Count == 0)
                {
                    return false;
                }
                item = _itemQ.Dequeue();
            }
            return true;
        }
    }
}