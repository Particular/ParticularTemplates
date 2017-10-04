using System;
using System.IO;
using System.Linq;

public static class DotNetTemplatesHelper
{
    static string dotNetCliPath = Environment.ExpandEnvironmentVariables(@"%PROGRAMFILES%\dotnet\dotnet.exe");

    public static void ExecuteNew(string parameters)
    {
        ProcessRunner.RunProcess(dotNetCliPath, "new " + parameters);
    }

    public static void Uninstall(string templatePackage)
    {
        ExecuteNew(" --uninstall " + templatePackage);
    }

    public static void Install(string packagePath)
    {
        ExecuteNew(" --install " + packagePath);
    }

    public static void Run(string templateName, string targetDirectory)
    {
        ExecuteNew($"{templateName} --output {targetDirectory}");
    }
    public static void Build(string projectDirectory)
    {
        var projectFile = Directory.EnumerateFiles(projectDirectory,"*.csproj").Single();
        ProcessRunner.RunProcess(dotNetCliPath, $" build {projectFile}");
    }
}