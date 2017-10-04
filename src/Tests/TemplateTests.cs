using System;
using System.IO;
using System.Linq;
using ApprovalTests;
using NUnit.Framework;

[TestFixture]
public class TemplateTests : IDisposable
{
    public TemplateTests()
    {
        Uninstall();
        Install();
    }

    static void Install()
    {
        //dotnet new --install \nugets\NServiceBus.Template.WindowsService.xxx.nupkg
        var nugets = Path.GetFullPath(Path.Combine(ProjectDirectory.ProjectPath, "../../nugets"));
        var nugetPath = Directory.EnumerateFiles(nugets, "*.nupkg").Single();
        DotNetTemplatesHelper.Install(nugetPath);
    }

    void Uninstall()
    {
        // dotnet new --uninstall ParticularTemplates
        DotNetTemplatesHelper.Uninstall("ParticularTemplates");
    }

    public void Dispose()
    {
        Uninstall();
    }

    [Test]
    public void WindowsService()
    {
        //dotnet new nsbservice
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(WindowsService));
        DotNetTemplatesHelper.Run("nsbservice", targetDirectory);
        VerifyDirectory(targetDirectory);
    }

    [Test]
    public void TransportAdapter()
    {
        //dotnet new scadapterservice
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(WindowsService));
        DotNetTemplatesHelper.Run("scadapterservice", targetDirectory);
        VerifyDirectory(targetDirectory);
    }

    static void VerifyDirectory(string targetDirectory)
    {
        var files = Directory.EnumerateFiles(targetDirectory, "*.*")
            .OrderByDescending(x=>Path.GetExtension(x))
            .ThenBy(x=>x);

        var dictionary = files.ToDictionary(
            keySelector: x => Path.GetFileName(x),
            elementSelector: x => $"\r\n{File.ReadAllText(x)}");
        Approvals.VerifyAll(dictionary);
    }
}