using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public static class DotNetTemplatesHelper
{
    static string dotNetCli = "dotnet";

    public static void ExecuteNew(string parameters)
    {
        ProcessRunner.RunProcess(dotNetCli, "new " + parameters);
    }

    public static void Uninstall(string templatePackage)
    {
        ExecuteNew(" --uninstall " + templatePackage);
    }

    public static void Install(string packagePath)
    {
        ExecuteNew(" --install " + packagePath);
    }

    public static void Run(string templateName, string targetDirectory, Dictionary<string, string> parameters = null)
    {
        ExecuteNew($"{templateName} --output {targetDirectory}" + FormatParameters(parameters));
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