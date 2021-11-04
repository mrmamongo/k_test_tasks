using System;
using System.IO;
using System.Text;

namespace Task_1
{
    public static class FileInspector 
    {
        private static readonly string[] CorruptedStrings;

        static FileInspector()
        {
            CorruptedStrings = new[] {
                "<script>evil_script()</script>", "rm -rf %userprofile%\\Documents",
                "Rundll32 sus.dll SusEntry"
            };
        }

        public static void ProcessEntity(FileInfo item)
        {
            try
            {
                if (!item.Exists && item.IsReadOnly)
                {
                    ProcessedFilesDict.GetInstance().Add(CorruptedFiles.Error);
                    return;
                }

                if (item.Length == 0)
                {
                    ProcessedFilesDict.GetInstance().Add(CorruptedFiles.Clean);
                    return;
                }

                using var fileStream = item.OpenRead();
                var arr = new byte[fileStream.Length];
                fileStream.Read(arr, 0, arr.Length);
                var textFromFile = Encoding.Default.GetString(arr);
                if (item.Extension == ".js" && textFromFile.Contains(CorruptedStrings[0]))
                {
                    ProcessedFilesDict.GetInstance().Add(CorruptedFiles.JS);
                    return;
                }
                ProcessedFilesDict.GetInstance().Add(CheckFile(textFromFile));
            }
            catch (Exception)
            {
                ProcessedFilesDict.GetInstance().Add(CorruptedFiles.Error);
            }
        }

        private static CorruptedFiles CheckFile(string fileString)
        {
            return (fileString.Contains(CorruptedStrings[1]) ? CorruptedFiles.rm_rf : 
                    (fileString.Contains(CorruptedStrings[2]) ? CorruptedFiles.Rundll32 : 
                        CorruptedFiles.Clean
                        )
                );
        }
    }
}