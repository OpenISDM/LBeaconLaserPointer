using System.Collections.Generic;
using System.Linq;
using System.IO;
using Windows.Storage;

namespace LBeaconLaserPointer.Modules.Utilities
{
    public class LocalStorage
    {
        private static readonly string FolderName = 
            ApplicationData.Current.LocalFolder + @"\Storage\";

        public static List<string> AllFileName()
        {
            List<string> FileNames = new List<string>();
            if (Directory.Exists(FolderName))
                FileNames.AddRange(Directory.EnumerateFiles(FolderName).ToList());
            else
                Directory.CreateDirectory(FolderName);

            return FileNames;
        }

        public static bool WriteToFile(string FileName, string Data)
        {
            try
            {
                if (!Directory.Exists(FolderName))
                    Directory.CreateDirectory(FolderName);

                using (StreamWriter sw = new StreamWriter(FolderName+FileName))
                {
                    sw.WriteLine(Data);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ReadOnFile(string FileName)
        {
            try
            {
                using (StreamReader SR = new StreamReader(FolderName + FileName))
                    return SR.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool CleanAllFile()
        {
            try
            {
                if (!Directory.Exists(FolderName))
                    Directory.CreateDirectory(FolderName);

                File.Delete(FolderName + "*");

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
