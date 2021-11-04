using System.Collections.Generic;

namespace Task_1
{
    public class ProcessedFilesDict
    {
        private readonly Dictionary<CorruptedFiles, int> _dictionary;
        private readonly object _locker = new();

        private static ProcessedFilesDict _instance;
        
        public static ProcessedFilesDict GetInstance()
        {
            return _instance ??= new ProcessedFilesDict();
        }
        
        private ProcessedFilesDict()
        {
            _dictionary = new Dictionary<CorruptedFiles, int>
            {
                { CorruptedFiles.Sum, 0},
                { CorruptedFiles.rm_rf, 0 },
                { CorruptedFiles.JS, 0 },
                { CorruptedFiles.Rundll32, 0 },
                { CorruptedFiles.Error, 0 },
                { CorruptedFiles.Clean, 0 }
            };
        }

        public void Add(CorruptedFiles fileType)
        {
            lock (_locker)
            {
                _dictionary[fileType]++;
                _dictionary[CorruptedFiles.Sum]++;
            }
        }

        public int Get(CorruptedFiles fileType)
        {
            int output;
            lock (_locker)
            {
                output = _dictionary[fileType];
            }

            return output;
        }
    }
}