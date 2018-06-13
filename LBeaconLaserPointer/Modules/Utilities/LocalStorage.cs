using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using System;
using System.Threading.Tasks;

namespace LBeaconLaserPointer.Modules.Utilities
{
    public class LocalStorage
    {
        private static readonly string FolderName = 
            ApplicationData.Current.LocalFolder.Path + @"\Storage\";

        public static async Task<List<string>> AllFileNameAsync()
        {
            List<StorageFile> FileNames = new List<StorageFile>();
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("Storage") != null)
            {
                StorageFolder Folder =await StorageFolder.GetFolderFromPathAsync(FolderName);
                FileNames.AddRange(await Folder.GetFilesAsync());
            }
            else
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("Storage");

            return FileNames.Select(c => c.Name).ToList();
        }

        public static async Task<bool> WriteToFileAsync(string FileName, string Data)
        {
            try
            {
                if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("Storage") == null)
                    await ApplicationData.Current.LocalFolder.CreateFolderAsync("Storage");
                StorageFolder Folder = await StorageFolder.GetFolderFromPathAsync(FolderName);
                StorageFile File = await Folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(File, Data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<string> ReadOnFileAsync(string FileName)
        {
            try
            {
                StorageFile File = await StorageFile.GetFileFromPathAsync(FolderName + FileName);
                return await FileIO.ReadTextAsync(File);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static async Task<bool> CleanAllFileAsync()
        {
            try
            {
                if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("Storage") != null)
                {
                    StorageFolder Folder = await StorageFolder.GetFolderFromPathAsync(FolderName);
                    await Folder.DeleteAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
