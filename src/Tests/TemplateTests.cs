using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Particular.Approvals;

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
        //dotnet new --install \nugets\ParticularTemplates.xxx.nupkg
        var nugets = Path.GetFullPath(Path.Combine(ProjectDirectory.ProjectPath, "..", "..", "nugets"));
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
            {"framework", "net472"}
        });
    }

    [Test]
    public void NServiceBusWindowsServiceDotNetCore()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(NServiceBusWindowsServiceDotNetCore));
        VerifyAndBuild("nsbwinservice", targetDirectory, new Dictionary<string, string>
        {
            {"framework", "netcoreapp3.1"}
        });
    }

    [Test]
    public void NServiceBusDockerContainer()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(NServiceBusDockerContainer));
        VerifyAndBuild("nsbdockercontainer", targetDirectory);
    }

    [Test]
    public void ScAdapterService()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(ScAdapterService));
        VerifyAndBuild("scadapterwinservice", targetDirectory);
    }

    [Test]
    public void ScAdapterServiceDiffFramework()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(ScAdapterServiceDiffFramework));
        VerifyAndBuild("scadapterwinservice", targetDirectory, new Dictionary<string, string>
        {
            {"framework", "net472"}
        });
    }

    [Test]
    public void ScAdapterServiceDotNetCore()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(ScAdapterServiceDotNetCore));
        VerifyAndBuild("scadapterwinservice", targetDirectory, new Dictionary<string, string>
        {
            {"framework", "netcoreapp3.1"}
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
        var fileText = new StringBuilder();

        foreach (var file in Directory.EnumerateFiles(targetDirectory, "*.*"))
        {
            fileText.AppendLine($"{Path.GetFileName(file)} =>");
            fileText.Append(File.ReadAllText(file));
            fileText.AppendLine();
            fileText.AppendLine();
            fileText.AppendLine();
        }

        Approver.Verify(fileText.ToString());
    }
}