using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scan_util
{
    public class Inspector
    {
        private static readonly string[] CorruptedStrings = {
            "<script>evil_script()</script>", "rm -rf %userprofile%\\Documents",
            "Rundll32 sus.dll SusEntry"
        };

        public async Task<CorruptedFileTypes> Inspect(Pair<bool, FileSystemInfo> entity)
        {
            return entity.Key ? InspectDirectory((DirectoryInfo)entity.Value) : await InspectFile((FileInfo)entity.Value);
        }

        private async Task<CorruptedFileTypes> InspectFile(FileInfo file)
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

                await using var fileStream = file.OpenRead();
                var arr = new byte[fileStream.Length];
                await fileStream.ReadAsync(arr, 0, arr.Length);
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

                return CorruptedFileTypes.Sum;
            }
            catch (Exception)
            {
                return CorruptedFileTypes.Error;
            }
        }
    }
}