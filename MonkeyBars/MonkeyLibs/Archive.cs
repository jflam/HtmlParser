using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace MonkeyLibs
{
    public sealed class Archive
    {
        // Inefficient copy 
        private static string CopyStreamToString(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        // TODO: figure out what type must be passed into this thing to open a Zip archive correctly
        public static async Task<string> Open(StorageFile file)
        {
            var extension = Path.GetExtension(file.Name);
            if (extension == ".gz")
            {
                // handle gzip files specially by dealing with them directdly
                var gzipStream = new GZipStream(await file.OpenStreamForReadAsync(), CompressionMode.Decompress);
                return CopyStreamToString(gzipStream);
            }
            else if (extension == ".js")
            {
                // An uncompressed file, so we just open and return the contents
                return CopyStreamToString(await file.OpenStreamForReadAsync());
            }
            else if (extension == ".zip")
            {
                // This is an archive file ... need to do the right thing here
                // Remember that in unix you first create a .tar and then gzip it to .gz
                // This means that the archive format ris the .tar and the compression is the .gz
                // This also means that there is a bunch of ambiguity about what a .zip is
                // A .zip is a package of compressed files. So it is the INVERSE of a .tar .gz
                // We use ZipArchive to manage .zips
                // How do we deal with the compression of the individual stream/files in the .zip?
                // Open the local storage and call CreateFolderAsync to create a temporary folder with the name of the archive
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(file.Name, CreationCollisionOption.OpenIfExists);

                try
                {
                    // Copy the input stream to a MemoryStream to workaround bug 669923
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var stream = await file.OpenStreamForReadAsync())
                        {
                            stream.CopyTo(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            using (var archive = new ZipArchive(memoryStream))
                            {
                                // Return the first opened file
                                if (archive.Entries.Count > 0)
                                {
                                    return CopyStreamToString(archive.Entries[0].Open());
                                }
                                else
                                {
                                    return "nothing in the archive";
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return String.Empty;
        }
    }


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
            try
            {
                var response = await proxy.GetContentAsync(request);
                foreach (var doc in response.getContentResponse.primaryDocuments)
                {
                    if (doc.primaryFormat == "Mtps.Xhtml")
                    {
                        return doc.Any.ToString(System.Xml.Linq.SaveOptions.DisableFormatting);
                    }
                }
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }

            return String.Empty;
        }
    }
}
