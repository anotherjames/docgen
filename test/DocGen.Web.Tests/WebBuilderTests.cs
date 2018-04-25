using System;
using System.IO;
using DocGen.Web.Impl;
using Xunit;

namespace DocGen.Web.Tests
{
    public class WebBuilderTests : IDisposable
    {
        string _tempDirectory;
        
        public WebBuilderTests()
        {
            _tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            while (Directory.Exists(_tempDirectory))
                _tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
        }

        [Fact]
        public void Can_export_from_file_provider()
        {
            //var webBuilder = new WebBuilder();
        }

        public void Dispose()
        {
            
        }
    }
}