using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Task_1_1
{
    public class Inspector
    {
        private static readonly string[] CorruptedStrings = {
            "<script>evil_script()</script>", "rm -rf %userprofile%\\Documents",
            "Rundll32 sus.dll SusEntry"
        };

        public CorruptedFileTypes Inspect(in Pair<bool, FileSystemInfo> entity)
        {
            return entity.Key ? InspectDirectory((DirectoryInfo)entity.Value) : InspectFile((FileInfo)entity.Value);
        }

        private CorruptedFileTypes InspectFile(FileInfo file)
        {
            try
            {
                switch (file)
                {
                    case { Exists: false, IsReadOnly: true }:
                        return CorruptedFileTypes.Error;
                    case { Length: 0 }:
                        return CorruptedFileTypes.Clean;
                }

                using var fileStream = file.OpenRead();
                var arr = new byte[fileStream.Length];
                fileStream.Read(arr, 0, arr.Length);
                var textFromFile = Encoding.Default.GetString(arr);
                if (file.Extension == ".js" && textFromFile.Contains(CorruptedStrings[0]))
                {
                    return CorruptedFileTypes.JS;
                }
                return (CheckFile(textFromFile));
            }
            catch (Exception)
            {
                return CorruptedFileTypes.Error;
            }
        }
        
        private static CorruptedFileTypes CheckFile(string fileString)
        {
            return (fileString.Contains(CorruptedStrings[1]) ? CorruptedFileTypes.rm_rf : 
                    (fileString.Contains(CorruptedStrings[2]) ? CorruptedFileTypes.Rundll32 : 
                        CorruptedFileTypes.Clean
                    )
                );
        }

        private CorruptedFileTypes InspectDirectory(DirectoryInfo dir)
        {
            try
            {
                if (!dir.Exists) return CorruptedFileTypes.Error;
                
                EntityQueue.GetInstance().Enqueue(
                    dir.EnumerateDirectories().ToList()
                        .ConvertAll(d => new Pair<bool, FileSystemInfo>(true, d)));
                EntityQueue.GetInstance().Enqueue(
                    dir.EnumerateFiles().ToList()
                        .ConvertAll(f => new Pair<bool, FileSystemInfo>(false, f))
                    );

                return CorruptedFileTypes.Clean;
            }
            catch (Exception)
            {
                return CorruptedFileTypes.Error;
            }
        }
    }
}