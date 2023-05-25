using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Particular.Approvals;

[TestFixture]
public class TemplateTests : IDisposable
{
    static TemplateTests()
    {
        // Windows separator char becomes special character in a regex, need to escape it
        var separator = Path.DirectorySeparatorChar == '\\' ? @"\\" : Path.DirectorySeparatorChar.ToString();
        IgnorePathRegex = new Regex($@"^{separator}(bin|obj){separator}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    [OneTimeSetUp]
    public async Task Setup()
    {

        await Uninstall(CreateTimeoutToken()).ConfigureAwait(false);
        await Install(CreateTimeoutToken()).ConfigureAwait(false);
    }

    static async Task Install(CancellationToken cancellationToken)
    {
        //dotnet new --install \nugets\ParticularTemplates.xxx.nupkg
        var nugets = Path.GetFullPath(Path.Combine(ProjectDirectory.ProjectPath, "..", "..", "nugets"));
        var directoryInfo = new DirectoryInfo(nugets);
        var latestNuget = directoryInfo.GetFiles("*.nupkg")
            .OrderByDescending(f => f.LastWriteTimeUtc)
            .First();

        await DotNetTemplatesHelper.Install(latestNuget.FullName, cancellationToken).ConfigureAwait(false);
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
    public async Task NServiceBusEndpoint()
    {
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken()).ConfigureAwait(false);
    }

    [Test]
    [TestCase("LearningTransport")]
    [TestCase("AzureServiceBus")]
    [TestCase("SQS")]
    [TestCase("RabbitMQ")]
    [TestCase("SQL")]
    [TestCase("MSMQ")]
    public async Task NServiceBusEndpointTransports(string transport)
    {
        var parameters = new Dictionary<string, string> { { "transport", transport } };
        if (transport == "MSMQ")
        {
            parameters.Add("framework", "net48");
        }
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken(), parameters).ConfigureAwait(false);
    }

    [Test]
    [TestCase("ConsoleApp")]
    [TestCase("WindowsService")]
    [TestCase("Docker")]
    public async Task NServiceBusEndpointHosting(string hosting)
    {
        var parameters = new Dictionary<string, string> { { "hosting", hosting } };
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken(), parameters).ConfigureAwait(false);
    }

    [Test]
    [TestCase("net7.0")]
    [TestCase("net6.0")]
    [TestCase("net48")]
    [TestCase("net472")]
    public async Task NServiceBusEndpointTargetFramework(string framework)
    {
        var parameters = new Dictionary<string, string> { { "framework", framework } };
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken(), parameters).ConfigureAwait(false);
    }

    [Test]
    [TestCase("LearningPersistence")]
    [TestCase("SQL")]
    [TestCase("CosmosDB")]
    [TestCase("AzureTable")]
    [TestCase("RavenDB")]
    [TestCase("MongoDB")]
    [TestCase("DynamoDB")]
    [TestCase("NonDurable")]
    public async Task NServiceBusEndpointPersistence(string persistence)
    {
        var parameters = new Dictionary<string, string> { { "persistence", persistence } };
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken(), parameters).ConfigureAwait(false);
    }

    static async Task VerifyAndBuild(string templateName, CancellationToken cancellationToken, Dictionary<string, string> parameters = null)
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath();
        await DotNetTemplatesHelper.Run(templateName, targetDirectory, parameters, cancellationToken).ConfigureAwait(false);
        await DotNetTemplatesHelper.Build(targetDirectory, cancellationToken).ConfigureAwait(false);
        VerifyDirectory(targetDirectory);
    }

    static void VerifyDirectory(string targetDirectory)
    {
        var fileText = new StringBuilder();

        foreach (var file in Directory.EnumerateFiles(targetDirectory, "*.*", SearchOption.AllDirectories).OrderBy(file => file, StringComparer.Ordinal))
        {
            var relativePath = file.Substring(targetDirectory.Length);
            if (IgnorePathRegex.IsMatch(relativePath))
            {
                continue;
            }

            const string hr = "---------------------------------------------------------------";

            var filename = Path.GetFileName(file);
            var contents = File.ReadAllText(file);

            if (Path.GetExtension(filename) == ".csproj")
            {
                contents = Regex.Replace(contents, @"Version=""\d+\.\d+\.\d+""", "Version=\"(VERSION)\"");
            }

            fileText.AppendLine(hr);
            fileText.AppendLine(filename);
            fileText.AppendLine(hr);
            fileText.Append(contents);
            fileText.AppendLine();
        }

        Approver.Verify(fileText.ToString(), scenario: ProjectDirectory.GetScenarioFromTestArguments());
    }

    static readonly Regex IgnorePathRegex;

}