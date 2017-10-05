using System;
using System.Collections.Generic;
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
        //  Uninstall();
    }

    [Test]
    public void NServiceBusWindowsService()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(NServiceBusWindowsService));
        VerifyAndBuild("nsbwinservice", targetDirectory);
    }

    [Test]
    public void NServiceBusWindowsServiceDiffFramework()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(NServiceBusWindowsServiceDiffFramework));
        VerifyAndBuild("nsbwinservice", targetDirectory, new Dictionary<string, string>
        {
            {"framework", "net462"}
        });
    }

    [Test]
    public void ScAdapterService()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(ScAdapterService));
        VerifyAndBuild("scadapterwinservice",targetDirectory);
    }

    [Test]
    public void ScAdapterServiceDiffFramework()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(ScAdapterService));
        VerifyAndBuild("scadapterwinservice", targetDirectory, new Dictionary<string, string>
        {
            {"framework", "net462"}
        });
    }

    static void VerifyAndBuild(string templateName, string targetDirectory, Dictionary<string, string> parameters = null)
    {
        DotNetTemplatesHelper.Run(templateName, targetDirectory, parameters);
        VerifyDirectory(targetDirectory);
        DotNetTemplatesHelper.Build(targetDirectory);
    }

    static void VerifyDirectory(string targetDirectory)
    {
        var files = Directory.EnumerateFiles(targetDirectory, "*.*");

        var dictionary = files.ToDictionary(
            keySelector: x => Path.GetFileName(x),
            elementSelector: x => $"\r\n{File.ReadAllText(x)}\r\n\r\n");
        Approvals.VerifyAll(dictionary);
    }
}