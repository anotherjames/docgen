#load "log.csx"
using System;

public static class Process
{
    public static void Run(string command)
    {
        var escapedArgs = command.Replace("\"", "\\\"");
        
        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "/usr/bin/env",
            Arguments = $"bash -c \"{escapedArgs}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = System.Diagnostics.Process.Start(processStartInfo))
        {
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception(string.Format("Exit code {0} from {1}", process.ExitCode, command));
        }
    }

    public static string Capture(string command)
    {
        var escapedArgs = command.Replace("\"", "\\\"");
        
        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "/usr/bin/env",
            Arguments = $"bash -c \"{escapedArgs}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (var process = System.Diagnostics.Process.Start(processStartInfo))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Log.Failure($"Standard output: {output}");
                Log.Failure($"Error output: {error}");
                throw new Exception(string.Format("Exit code {0} from {1}", process.ExitCode, command));
            }
            
            return output;
        }
    }
}
