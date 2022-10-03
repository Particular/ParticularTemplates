using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Particular.Approvals;

[TestFixture]
public class TemplateTests : IDisposable
{
    [SetUp]
    public async Task Setup()
    {
        await Uninstall(CreateTimeoutToken()).ConfigureAwait(false);
        await Install(CreateTimeoutToken()).ConfigureAwait(false);
    }

    static async Task Install(CancellationToken cancellationToken)
    {
        //dotnet new --install \nugets\ParticularTemplates.xxx.nupkg
        var nugets = Path.GetFullPath(Path.Combine(ProjectDirectory.ProjectPath, "..", "..", "nugets"));
        var nugetPath = Directory.EnumerateFiles(nugets, "*.nupkg").Single();
        await DotNetTemplatesHelper.Install(nugetPath, cancellationToken).ConfigureAwait(false);
    }

    async Task Uninstall(CancellationToken cancellationToken)
    {
        // dotnet new --uninstall ParticularTemplates
        await DotNetTemplatesHelper.Uninstall("ParticularTemplates", cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        //  Uninstall();
    }

    CancellationToken CreateTimeoutToken() => new CancellationTokenSource(60_000).Token;

    [Test]
    public async Task NServiceBusWindowsService()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(NServiceBusWindowsService));
        await VerifyAndBuild("nsbwinservice", targetDirectory, CreateTimeoutToken()).ConfigureAwait(false);
    }

    [Test]
    public async Task NServiceBusWindowsServiceDiffFramework()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(NServiceBusWindowsServiceDiffFramework));
        await VerifyAndBuild("nsbwinservice", targetDirectory, CreateTimeoutToken(), new Dictionary<string, string> { { "framework", "net48" } }).ConfigureAwait(false);
    }

    [Test]
    public async Task NServiceBusWindowsServiceDotNetCore()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(NServiceBusWindowsServiceDotNetCore));
        await VerifyAndBuild("nsbwinservice", targetDirectory, CreateTimeoutToken(), new Dictionary<string, string> { { "framework", "net6.0" } }).ConfigureAwait(false);
    }

    [Test]
    public async Task NServiceBusDockerContainer()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(NServiceBusDockerContainer));
        await VerifyAndBuild("nsbdockercontainer", targetDirectory, CreateTimeoutToken()).ConfigureAwait(false);
    }

    [Test]
    public async Task ScAdapterService()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(ScAdapterService));
        await VerifyAndBuild("scadapterwinservice", targetDirectory, CreateTimeoutToken()).ConfigureAwait(false);
    }

    [Test]
    public async Task ScAdapterServiceDiffFramework()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(ScAdapterServiceDiffFramework));
        await VerifyAndBuild("scadapterwinservice", targetDirectory, CreateTimeoutToken(), new Dictionary<string, string> { { "framework", "net48" } }).ConfigureAwait(false);
    }

    [Test]
    public async Task ScAdapterServiceDotNetCore()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath(nameof(ScAdapterServiceDotNetCore));
        await VerifyAndBuild("scadapterwinservice", targetDirectory, new CancellationTokenSource(60_000).Token, new Dictionary<string, string> { { "framework", "net6.0" } }).ConfigureAwait(false);
    }

    static async Task VerifyAndBuild(string templateName, string targetDirectory, CancellationToken cancellationToken, Dictionary<string, string> parameters = null)
    {
        await DotNetTemplatesHelper.Run(templateName, targetDirectory, parameters, cancellationToken).ConfigureAwait(false);
        VerifyDirectory(targetDirectory);
        await DotNetTemplatesHelper.Build(targetDirectory, cancellationToken).ConfigureAwait(false);
    }

    static void VerifyDirectory(string targetDirectory)
    {
        var fileText = new StringBuilder();

        foreach (var file in Directory.EnumerateFiles(targetDirectory, "*.*").OrderBy(file => file, StringComparer.Ordinal))
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