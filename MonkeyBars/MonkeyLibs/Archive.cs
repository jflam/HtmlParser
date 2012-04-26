using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MonkeyLibs
{
    /// <summary>
    /// This is the class that is used to communicate with the MSDN documentation service.
    /// It will marshal documents back to the caller who can then render them in the browser.
    /// </summary>
    public sealed class Help
    {
        public static async Task<string> GetHelp(string id)
        {
            var docs = new Msdn.requestedDocument[1];
            docs[0] = new Msdn.requestedDocument();
            docs[0].type = Msdn.documentTypes.primary;
            docs[0].selector = "Mtps.Xhtml";

            var request = new Msdn.getContentRequest();
            request.contentIdentifier = id;
            request.locale = "en-us";
            request.version = "MSDN.10";
            request.requestedDocuments = docs;

            var proxy = new Msdn.ContentServicePortTypeClient();
            var response = await proxy.GetContentAsync(request);
            foreach (var doc in response.getContentResponse.primaryDocuments)
            {
                if (doc.primaryFormat == "Mtps.Xhtml")
                {
                    return doc.Any.ToString(System.Xml.Linq.SaveOptions.DisableFormatting);
                }
            }

            return String.Empty;
        }
    }
}
