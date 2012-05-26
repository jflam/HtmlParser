using System.Runtime.InteropServices.WindowsRuntime;
using MonkeyLibs;
using Windows.Foundation;
using Windows.Storage;

namespace MonkeyWrap
{
    public sealed class MsdnHelp
    {
        public IAsyncOperation<string> GetHelp(string id)
        {
            return AsyncInfo.Run(token => Help.GetHelp(id));
        }

        public IAsyncOperation<string> Open(StorageFile file)
        {
            return AsyncInfo.Run(token => Archive.Open(file));
        }
    }
}
