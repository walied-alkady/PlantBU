using System.IO;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ISave'
public interface ISave
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ISave'
{
    //Method to save document as a file and view the saved document
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ISave.SaveAndView(string, string, MemoryStream, bool)'
	Task<string> SaveAndView(string filename, string contentType, MemoryStream stream,bool view=false);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ISave.SaveAndView(string, string, MemoryStream, bool)'
}

