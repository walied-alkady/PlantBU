using System.IO;
using Xamarin.Essentials;

namespace PlantBU.ViewModels
{
    class DrawingViewModel : BaseViewModel
    {

        /// <summary>
        /// The PDF document stream that is loaded into the instance of the PDF viewer. 
        /// </summary>
        public Stream PdfDocumentStream
        {
            get
            {
                if (_pdfDocumentStream != null && _pdfDocumentStream.CanRead)
                    return _pdfDocumentStream;
                else
                    return null;
            }
            set
            {
                _pdfDocumentStream = value;
                OnPropertyChanged("PdfDocumentStream");
            }
        }
        private Stream _pdfDocumentStream;
        public DrawingViewModel()
        {
        }
        public void OpenDrawing(string draw)
        {
            //Local
            //string path = Path.Combine(FileSystem.AppDataDirectory + "\\Drawings\\");
            //PdfDocumentStream = File.OpenRead(path + draw + ".pdf");

            //Cloud
            string path = Path.Combine(@"https://titanbu-tokvg.mongodbstitch.com/Drawings/");
            PdfDocumentStream = File.OpenRead(path + draw + ".pdf");
        }
    }
}
