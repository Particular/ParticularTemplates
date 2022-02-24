using System;
using System.Diagnostics;
using System.Text;

public class ProcessRunner
{
    public static void RunProcess(string fileName, string arguments)
    {
        var output = new StringBuilder();
        using (var process = new Process
        {
            StartInfo =
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            },
            EnableRaisingEvents = true,
        })
        {
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    output.AppendLine(args.Data);
                }
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    output.AppendLine(args.Data);
                }
            };
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            var hasExited = process.WaitForExit(10000);
            if (!hasExited || process.ExitCode != 0)
            {
                if (!hasExited)
                {
                    output.AppendLine("The process failed to exit before the timeout period.");
                }

                throw new Exception(output.ToString());
            }
        }
    }
}