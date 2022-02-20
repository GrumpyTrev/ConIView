using System;
using System.IO;

namespace ConIView
{
    public static class Constants
    {
        public const string StorageFilename = "PictureLists/Directory.xml";
        public const string StorageFolder = "PictureLists";

        public static string TodoStorePath => Path.Combine( App.FileProvider?.DatabaseDirectory ??
            Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), StorageFilename );

        public static string StoreFolder => Path.Combine( App.FileProvider?.DatabaseDirectory ??
            Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), StorageFolder );
    }
}
