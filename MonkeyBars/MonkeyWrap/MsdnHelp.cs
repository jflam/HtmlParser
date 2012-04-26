using System.Runtime.InteropServices.WindowsRuntime;
using MonkeyLibs;
using Windows.Foundation;

namespace MonkeyWrap
{
    public sealed class MsdnHelp
    {
        public IAsyncOperation<string> GetHelp(string id)
        {
            return AsyncInfo.Run(token => Help.GetHelp(id));
        }
    }
}
