using System;

public static class Path
{
    public static string Expand(string path)
    {
        return System.IO.Path.GetFullPath(path);
    }

    public static void CleanDirectory(string directory)
    {
        var di = new System.IO.DirectoryInfo(directory);

        if(!di.Exists)
        {
            return;
        }

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete(); 
        }

        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true); 
        }
    }
}
