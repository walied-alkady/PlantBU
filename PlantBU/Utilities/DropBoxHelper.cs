using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PlantBU.Utilities
{
    class DropBoxHelper
    {
        private const string ClientId = "s7do98tg7bkp56o"; //Appkey
        private const string AppSecret = "55dtktrkjyvg8kl"; //Your App secret key
        private const string RedirectUri = "http://127.0.0.1:52475/"; //Your Redirect Uri

        public string AccessToken { get; set; } = "KzIIiHR7mW4AAAAAAAAAAZHOuoWfY5xuzCWF6iGx1k7fmvB4amOQ8vogmyE50SHu";
#pragma warning disable CS0649 // Field 'DropBoxHelper.OnAuthenticated' is never assigned to, and will always have its default value null
        public Action OnAuthenticated;
#pragma warning restore CS0649 // Field 'DropBoxHelper.OnAuthenticated' is never assigned to, and will always have its default value null
        private string oauth2State;
#pragma warning disable CS0169 // The field 'DropBoxHelper.pdfDocumentStream' is never used
        private Stream pdfDocumentStream;
#pragma warning restore CS0169 // The field 'DropBoxHelper.pdfDocumentStream' is never used
        public bool IsAuthorized { get; set; }
        public DropBoxHelper()
        {
            if (!IsAuthorized)
            {
                Task task = Authorize();
                task.Wait();
            }

        }
        private DropboxClient GetClient()
        {
            return new DropboxClient(AccessToken);
        }
        public async void CheckAuthorization()
        {
            if (AccessToken == null)
            {
                await Authorize();
            }
            else
            {
                IsAuthorized = true;
            }
        }
        public async Task Authorize()
        {
            Application.Current.Properties.Clear();

            if (string.IsNullOrWhiteSpace(AccessToken) == false)
            {
                // Already authorized
                this.OnAuthenticated?.Invoke();
                return;
            }

            // Run Dropbox authentication
            this.oauth2State = Guid.NewGuid().ToString("N");
            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, ClientId, new Uri(RedirectUri), this.oauth2State);
            var webView = new WebView { Source = new UrlWebViewSource { Url = authorizeUri.AbsoluteUri } };
            webView.Navigating += this.WebViewOnNavigating;
            var contentPage = new ContentPage { Content = webView };
            await Application.Current.MainPage.Navigation.PushModalAsync(contentPage);
        }
        public async Task<IList<Metadata>> ListFiles()
        {
            try
            {
                using (var client = this.GetClient())
                {
                    var list = await client.Files.ListFolderAsync(string.Empty, true);
                    return list?.Entries;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<string>> GetPdfFiles()
        {
            List<string> dropboxPdfFiles = new List<string>();
            using (var dbx = GetClient())
            {

                var list = await dbx.Files.ListFolderAsync("", true);
                IList<Metadata> lst = list?.Entries;
                foreach (var item in lst.Where(i => i.IsFile))
                {
                    if (item.Name.EndsWith(".pdf"))
                        dropboxPdfFiles.Add(item.PathDisplay);
                }
                return dropboxPdfFiles;
            }

        }
        public async Task<Stream> DownloadPdfStream(string fileName)
        {
            var byteArray = await ReadFile(fileName);
            return new MemoryStream(byteArray);
        }
        public async void UploadPdfStream(MemoryStream stream, string fileName)
        {
            await WriteFile(stream, fileName);
        }
        public async Task<byte[]> ReadFile(string filePath)
        {
            try
            {
                using (var client = GetClient())
                {
                    var response = await client.Files.DownloadAsync(filePath);
                    var bytes = response?.GetContentAsByteArrayAsync();
                    return bytes?.Result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<FileMetadata> WriteFile(MemoryStream fileContent, string fileName)
        {
            try
            {
                var commitInfo = new CommitInfo(fileName, WriteMode.Overwrite.Instance, false, DateTime.Now);

                using (var client = this.GetClient())
                {
                    var metadata = await client.Files.UploadAsync(commitInfo, fileContent);
                    return metadata;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        private async void WebViewOnNavigating(object sender, WebNavigatingEventArgs e)
        {
            // was if (!e.Url.StartsWith(RedirectUri, StringComparison.OrdinalIgnoreCase))
            if (!e.Url.StartsWith(@"https://www.dropbox.com/oauth2/", StringComparison.OrdinalIgnoreCase))
            {
                // we need to ignore all navigation that isn't to the redirect uri.
                return;
            }
            var result = DropboxOAuth2Helper.ParseTokenFragment(new Uri(e.Url));

            if (result.State != this.oauth2State)
            {
                return;
            }

            this.AccessToken = result.AccessToken;
            this.OnAuthenticated?.Invoke();

            try
            {

            }
            catch (Exception)
            {

            }
            finally
            {

                e.Cancel = true;
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }
    }
}
