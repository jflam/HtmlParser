using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Compression;
using Windows.Storage;

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
}
