using System;
using System.Collections.Generic;

namespace DocGen.Web
{
    public interface IWeb : IDisposable
    {
        IReadOnlyCollection<string> Paths { get; }

        void Listen();
    }
}