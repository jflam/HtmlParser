using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace ZipHelper
{
    public sealed class Zip
    {
        // Inefficient copy 
        private static string CopyStreamToString(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static async Task<string> OpenInternal(StorageFile file)
        {
            var extension = Path.GetExtension(file.Name);
            if (extension == ".gz")
            {
                // handle gzip files specially by dealing with them directdly
                var gzipStream = new GZipStream(await file.OpenStreamForReadAsync(), CompressionMode.Decompress);
                return CopyStreamToString(gzipStream);
            }
            else if (extension == ".htm")
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

        // Public exposed function that maps an IAsyncOperation<string> to Task<string>
        public static IAsyncOperation<string> Open(StorageFile file)
        {
            return AsyncInfo.Run(token => Zip.OpenInternal(file));
        }
    }
}
