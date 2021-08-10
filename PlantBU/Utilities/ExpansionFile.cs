using System.IO;
using System.IO.Compression;
using Xamarin.Essentials;

namespace PlantBU.Utilities
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpansionFile'
    public class ExpansionFile
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpansionFile'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpansionFile.Extract()'
        public void Extract()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpansionFile.Extract()'
        {
            ZipFile.ExtractToDirectory(
    Path.Combine(FileSystem.AppDataDirectory, "Drawings.zip"),//zipPath 
                 FileSystem.AppDataDirectory + @"\Drawings\"//extractPath
);
        }
    }
}
