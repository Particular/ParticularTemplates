using System;
using System.Diagnostics;
using System.Text;

public class ProcessRunner
{
    public static string RunProcess(string fileName, string arguments)
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
            process.WaitForExit(10000);
            if (process.ExitCode != 0)
            {
                throw new Exception(output.ToString());
            }
        }

        return output.ToString();
    }
}