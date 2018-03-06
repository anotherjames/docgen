package hugo

import (
	"os"
	"time"

	"github.com/spf13/afero"
)

type fakeFs struct {
	afero.Fs
	realFs afero.Fs
}

func createFakeFs(realFs afero.Fs) afero.Fs {
	return &fakeFs{realFs: realFs}
}

// Create creates a file in the filesystem, returning the file and an
// error, if any happens.
func (f *fakeFs) Create(name string) (afero.File, error) {
	return f.realFs.Create(name)
}

// Mkdir creates a directory in the filesystem, return an error if any
// happens.
func (f *fakeFs) Mkdir(name string, perm os.FileMode) error {
	return f.realFs.Mkdir(name, perm)
}

// MkdirAll creates a directory path and all parents that does not exist
// yet.
func (f *fakeFs) MkdirAll(path string, perm os.FileMode) error {
	return f.realFs.MkdirAll(path, perm)
}

// Open opens a file, returning it or an error, if any happens.
func (f *fakeFs) Open(name string) (afero.File, error) {
	return f.realFs.Open(name)
}

// OpenFile opens a file using the given flags and the given mode.
func (f *fakeFs) OpenFile(name string, flag int, perm os.FileMode) (afero.File, error) {
	return f.realFs.OpenFile(name, flag, perm)
}

// Remove removes a file identified by name, returning an error, if any
// happens.
func (f *fakeFs) Remove(name string) error {
	return f.realFs.Remove(name)
}

// RemoveAll removes a directory path and any children it contains. It
// does not fail if the path does not exist (return nil).
func (f *fakeFs) RemoveAll(path string) error {
	return f.realFs.RemoveAll(path)
}

// Rename renames a file.
func (f *fakeFs) Rename(oldname, newname string) error {
	return f.realFs.Rename(oldname, newname)
}

// Stat returns a FileInfo describing the named file, or an error, if any
// happens.
func (f *fakeFs) Stat(name string) (os.FileInfo, error) {
	return f.realFs.Stat(name)
}

// The name of this FileSystem
func (f *fakeFs) Name() string {
	return f.realFs.Name()
}

//Chmod changes the mode of the named file to mode.
func (f *fakeFs) Chmod(name string, mode os.FileMode) error {
	return f.realFs.Chmod(name, mode)
}

//Chtimes changes the access and modification times of the named file
func (f *fakeFs) Chtimes(name string, atime time.Time, mtime time.Time) error {
	return f.realFs.Chtimes(name, atime, mtime)
}
