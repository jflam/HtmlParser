using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Windows.Storage;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MonkeyLibs
{
    public sealed class Archive
    {
        private StorageFile _file;

        public double Add(double x, double y)
        {
            return x + y;
        }

        public Archive(StorageFile file)
        {
            _file = file;
        }

        public async void DoSomething()
        {
            var result = await _file.OpenAsync(FileAccessMode.ReadWrite);
            
        }
    }

    /// <summary>
    /// This is the class that is used to communicate with the MSDN documentation service.
    /// It will marshal documents back to the caller who can then render them in the browser.
    /// </summary>
    public sealed class Help
    {
        private async Task<string> InternalGetHelp(string id)
        {
            var docs = new Msdn.requestedDocument[1];
            docs[0] = new Msdn.requestedDocument();
            docs[0].type = Msdn.documentTypes.primary;
            docs[0].selector = "Mtps.Xhtml";

            var request = new Msdn.getContentRequest();
            request.contentIdentifier = id;
            request.locale = "en-us";
            request.version = String.Empty;
            request.requestedDocuments = docs;

            var proxy = new Msdn.ContentServicePortTypeClient();
            var response = await proxy.GetContentAsync(request);
            foreach (var doc in response.getContentResponse.primaryDocuments)
            {
                if (doc.primaryFormat == "Htps.Xhtml")
                {
                    return doc.Any.ToString(System.Xml.Linq.SaveOptions.DisableFormatting);
                }
            }

            return String.Empty;
        }

        public IAsyncOperation<string> GetHelp(string id)
        {
            return AsyncInfo.Run(token => InternalGetHelp(id));
        }
    }
}
