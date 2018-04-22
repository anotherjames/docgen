using System;

namespace DocGen.Core
{
    public class DocGenException : Exception
    {
        public DocGenException(string message) : base(message)
        {
            
        }
    }
}