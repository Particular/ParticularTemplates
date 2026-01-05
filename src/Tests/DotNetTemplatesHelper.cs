using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleExec;

public static class DotNetTemplatesHelper
{
    static string dotNetCli = "dotnet";

    public static async Task<(string StandardOutput, string StandardError)> ExecuteNew(string parameters, CancellationToken cancellationToken = default)
    {
        var (stdout, stderr) = await Command.ReadAsync(dotNetCli, "new " + parameters, ct: cancellationToken).ConfigureAwait(false);

        Console.WriteLine(stdout);
        if (!string.IsNullOrWhiteSpace(stderr))
        {
            Console.Error.WriteLine(stderr);
            Assert.Fail("Information received on StandardError stream.");
        }

        return (stdout, stderr);
    }

    public static async Task Uninstall(string templatePackage, CancellationToken cancellationToken = default)
    {
        var (output, _) = await ExecuteNew(" uninstall ", cancellationToken).ConfigureAwait(false);

        if (output.Contains(templatePackage, StringComparison.OrdinalIgnoreCase))
        {
            _ = ExecuteNew("uninstall " + templatePackage, cancellationToken).ConfigureAwait(false);
        }
    }

    public static async Task Install(string packagePath, CancellationToken cancellationToken = default)
    {
        _ = await ExecuteNew("install " + packagePath, cancellationToken).ConfigureAwait(false);
    }

    public static async Task Run(string templateName, string targetDirectory, Dictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
    {
        _ = await ExecuteNew($"{templateName} --output {targetDirectory}" + FormatParameters(parameters), cancellationToken).ConfigureAwait(false);
    }

    static string FormatParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null)
        {
            return "";
        }

        var builder = new StringBuilder();
        foreach (var parameter in parameters)
        {
            builder.Append($" --{parameter.Key} {parameter.Value}");
        }

        return builder.ToString();
    }

    public static async Task Build(string projectDirectory, CancellationToken cancellationToken = default)
    {
        var projectFile = Directory.EnumerateFiles(projectDirectory, "*.csproj").Single();
        _ = await Command.ReadAsync(dotNetCli, $" build {projectFile}", ct: cancellationToken).ConfigureAwait(false);
    }

    public static async Task Restore(string projectDirectory, CancellationToken cancellationToken = default)
    {
        var projectFile = Directory.EnumerateFiles(projectDirectory, "*.csproj").Single();
        _ = await Command.ReadAsync(dotNetCli, $" restore {projectFile}", ct: cancellationToken).ConfigureAwait(false);
    }
}