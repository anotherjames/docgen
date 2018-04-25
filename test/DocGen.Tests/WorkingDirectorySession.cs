using System;
using System.IO;

namespace DocGen.Tests
{
    public class WorkingDirectorySession : IDisposable
    {
        public WorkingDirectorySession()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            while (System.IO.Directory.Exists(path))
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            Directory = path;
        }

        public string Directory { get; }

        public void Dispose()
        {
            System.IO.Directory.Delete(Directory, true);
        }
    }
}