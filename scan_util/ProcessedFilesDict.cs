using System.Collections.Generic;

namespace scan_util
{
    public class ProcessedFilesDict
    {
        private readonly Dictionary<CorruptedFileTypes, int> _dictionary;
        private readonly object _locker = new();

        private static ProcessedFilesDict _instance;
        
        public static ProcessedFilesDict GetInstance()
        {
            return _instance ??= new ProcessedFilesDict();
        }
        
        private ProcessedFilesDict()
        {
            _dictionary = new Dictionary<CorruptedFileTypes, int>
            {
                { CorruptedFileTypes.Sum, 0},
                { CorruptedFileTypes.rm_rf, 0 },
                { CorruptedFileTypes.JS, 0 },
                { CorruptedFileTypes.Rundll32, 0 },
                { CorruptedFileTypes.Error, 0 },
                { CorruptedFileTypes.Clean, 0}
            };
        }

        public void Add(CorruptedFileTypes fileTypeType)
        {
            lock (_locker)
            {
                if (fileTypeType != CorruptedFileTypes.Sum)
                {
                    _dictionary[fileTypeType]++;
                    _dictionary[CorruptedFileTypes.Sum]++;   
                }
            }
        }

        public int Get(CorruptedFileTypes fileTypeType)
        {
            int output;
            lock (_locker)
            {
                output = _dictionary[fileTypeType];
            }

            return output;
        }
    }
}