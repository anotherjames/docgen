using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Collections;

namespace DocGen.Web
{
    public class EmbeddedFileProvider : IFileProvider
    {   
        Assembly _assembly;
        List<EmbeddedFile> _files;
        const string PREFIX = "DocGen.Web.Resources";

        public EmbeddedFileProvider()
        {
            _assembly = typeof(EmbeddedFileProvider).GetTypeInfo().Assembly;
            _files = _assembly.GetManifestResourceNames()
                .Select(x => {
                    var parts = x.Substring(PREFIX.Length).Split('.');
                    var directory = parts.Take(parts.Length - 2);
                    var fileName = parts.Skip(parts.Length - 2);
                    var path = Path.DirectorySeparatorChar + Path.Combine(Path.Combine(directory.ToArray()), string.Join(".", fileName));
                    return new EmbeddedFile(path, x);
                })
                .ToList();
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new DirectoryContents(subpath, _files);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var file = _files.SingleOrDefault(x => x.Path.Equals(subpath, StringComparison.InvariantCultureIgnoreCase));
            if(file == null) {
                return null;
            }
            return new FileInfo(file, _assembly);
        }

        public IChangeToken Watch(string filter)
        {
            return null;
        }

        class DirectoryContents : IDirectoryContents
        {
            string _path;
            List<EmbeddedFile> _files;

            public DirectoryContents(string path, List<EmbeddedFile> files)
            {
                _path = path;
                _files = files;   
            }

            public bool Exists 
            {
                get
                {
                    return _files.Any(x => x.Path.StartsWith(_path, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            public IEnumerator<IFileInfo> GetEnumerator()
            {
                return Enumerable.Empty<IFileInfo>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return Enumerable.Empty<IFileInfo>().GetEnumerator();
            }
        }

        class FileInfo : IFileInfo
        {
            EmbeddedFile _file;
            Assembly _assembly;

            public FileInfo(EmbeddedFile file, Assembly assembly)
            {
                _file = file;
                _assembly = assembly;
            }

            public bool Exists => true;

            public long Length
            {
                get
                {
                    EnsureExists();

                    // TODO:
                    return 0;
                }
            }

            public string PhysicalPath
            {
                get
                {
                    EnsureExists();

                    return _file.ResourceName;
                }
            }

            public string Name
            {
                get
                {
                    EnsureExists();

                    return Path.GetFileName(_file.Path);
                }
            }

            public DateTimeOffset LastModified
            {
                get
                {
                    EnsureExists();

                    // TODO:
                    return DateTime.Now;
                }
            }

            public bool IsDirectory => false;

            public Stream CreateReadStream()
            {
                EnsureExists();

                return _assembly.GetManifestResourceStream(_file.ResourceName);
            }

            private void EnsureExists()
            {
                if(!Exists) throw new InvalidOperationException("File doesn't exist.");
            }
        }

        class EmbeddedFile
        {
            public EmbeddedFile(string path, string resourceName)
            {
                Path = path;
                ResourceName = resourceName;
            }

            public string Path { get; }

            public string ResourceName { get; }
        }
    }
}