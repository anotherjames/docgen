using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocGen.Web
{
    public interface IWeb : IDisposable
    {
        IReadOnlyCollection<string> Paths { get; }

        void Listen();

        Task ExportTo(string directory);
    }
}