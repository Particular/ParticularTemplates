using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public static class DotNetTemplatesHelper
{
    static string dotNetCli = "dotnet";

    public static string ExecuteNew(string parameters)
    {
        return ProcessRunner.RunProcess(dotNetCli, "new " + parameters);
    }

    public static string Uninstall(string templatePackage)
    {
        // Check if installed first
        var output = ExecuteNew(" --uninstall");

        if (output.IndexOf(templatePackage, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return ExecuteNew(" --uninstall " + templatePackage);
        }

        return output;
    }

    public static string Install(string packagePath)
    {
        return ExecuteNew(" --install " + packagePath);
    }

    public static string Run(string templateName, string targetDirectory, Dictionary<string, string> parameters = null)
    {
        return ExecuteNew($"{templateName} --output {targetDirectory}" + FormatParameters(parameters));
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

    public static void Build(string projectDirectory)
    {
        var projectFile = Directory.EnumerateFiles(projectDirectory, "*.csproj").Single();
        ProcessRunner.RunProcess(dotNetCli, $" build {projectFile}");
    }
}