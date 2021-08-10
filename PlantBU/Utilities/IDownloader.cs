using System;

namespace PlantBU.Utilities
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IDownloader'
    public interface IDownloader
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IDownloader'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IDownloader.DownloadFile(string, string)'
        void DownloadFile(string url, string folder);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IDownloader.DownloadFile(string, string)'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IDownloader.OnFileDownloaded'
        event EventHandler<DownloadEventArgs> OnFileDownloaded;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IDownloader.OnFileDownloaded'
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DownloadEventArgs'
    public class DownloadEventArgs : EventArgs
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DownloadEventArgs'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DownloadEventArgs.FileSaved'
        public bool FileSaved = false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DownloadEventArgs.FileSaved'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DownloadEventArgs.DownloadEventArgs(bool)'
        public DownloadEventArgs(bool fileSaved)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DownloadEventArgs.DownloadEventArgs(bool)'
        {
            FileSaved = fileSaved;
        }
    }
}
