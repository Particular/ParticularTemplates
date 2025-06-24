using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        await Uninstall(CreateTimeoutToken());
        await Install(CreateTimeoutToken());
    }

    static async Task Install(CancellationToken cancellationToken)
    {
        //dotnet new --install \nugets\ParticularTemplates.xxx.nupkg
        var nugets = Path.GetFullPath(Path.Combine(ProjectDirectory.ProjectPath, "..", "..", "nugets"));
        var directoryInfo = new DirectoryInfo(nugets);
        var latestNuget = directoryInfo.GetFiles("*.nupkg")
            .OrderByDescending(f => f.LastWriteTimeUtc)
            .First();

        await DotNetTemplatesHelper.Install(latestNuget.FullName, cancellationToken);
    }

    async Task Uninstall(CancellationToken cancellationToken)
    {
        // dotnet new --uninstall ParticularTemplates
        await DotNetTemplatesHelper.Uninstall("ParticularTemplates", cancellationToken);
    }

    public void Dispose()
    {
        //  Uninstall();
    }

    CancellationToken CreateTimeoutToken() => new CancellationTokenSource(60_000).Token;

    [Test]
    public async Task NServiceBusEndpoint()
    {
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken());
    }

    [Test]
    [TestCase("LearningTransport")]
    [TestCase("AzureServiceBus")]
    [TestCase("AzureStorageQueues")]
    [TestCase("SQS")]
    [TestCase("RabbitMQ")]
    [TestCase("SQL")]
    [TestCase("PostgreSQL")]
    public async Task NServiceBusEndpointTransports(string transport)
    {
        var parameters = new Dictionary<string, string> { { "transport", transport } };
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken(), parameters);
    }

    [Test]
    [TestCase("ConsoleApp")]
    [TestCase("WindowsService")]
    [TestCase("Docker")]
    public async Task NServiceBusEndpointHosting(string hosting)
    {
        var parameters = new Dictionary<string, string> { { "hosting", hosting } };
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken(), parameters);
    }

    [Test]
    [TestCase("net10.0")]
    public async Task NServiceBusEndpointTargetFramework(string framework)
    {
        var parameters = new Dictionary<string, string> { { "framework", framework } };
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken(), parameters);
    }

    [Test]
    [TestCase("LearningPersistence")]
    [TestCase("MSSQL")]
    [TestCase("MySQL")]
    [TestCase("PostgreSQL")]
    [TestCase("Oracle")]
    [TestCase("CosmosDB")]
    [TestCase("AzureTable")]
    [TestCase("RavenDB")]
    [TestCase("MongoDB")]
    [TestCase("DynamoDB")]
    public async Task NServiceBusEndpointPersistence(string persistence)
    {
        var parameters = new Dictionary<string, string> { { "persistence", persistence } };
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken(), parameters);
    }

    [Test]
    [TestCase("SQL", "MSSQL")]
    [TestCase("PostgreSQL", "PostgreSQL")]
    public async Task TransportPersistenceCombinations(string transport, string persistence)
    {
        var parameters = new Dictionary<string, string> { { "transport", transport }, { "persistence", persistence } };
        await VerifyAndBuild("nsbendpoint", CreateTimeoutToken(), parameters);
    }

    [Test]
    public async Task Handler()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath();
        var token = CreateTimeoutToken();
        var messageClass = @"
using NServiceBus;

public class TestMessage : ICommand { }";

        await DotNetTemplatesHelper.Run("nsbendpoint", targetDirectory, cancellationToken: token);

        await DotNetTemplatesHelper.Run("nsbhandler", targetDirectory, new() { { "messagetype", "TestMessage" } }, token);
        await File.WriteAllTextAsync(Path.Combine(targetDirectory, "Messages.cs"), messageClass, token);
        await DotNetTemplatesHelper.Build(targetDirectory, token);
        VerifyDirectory(targetDirectory, ["Program.cs"]);
    }

    [Test]
    public async Task Saga()
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath();
        var token = CreateTimeoutToken();
        var messagesClass = @"
using NServiceBus;

public class OrderPlaced : IEvent
{
    public string CorrelationId { get; set; }
}

public class OrderBilled : IEvent
{
    public string CorrelationId { get; set; }
}";

        await DotNetTemplatesHelper.Run("nsbendpoint", targetDirectory, cancellationToken: token);

        await DotNetTemplatesHelper.Run("nsbsaga", targetDirectory, new() { { "name", "ShippingPolicy" }, { "messagetype1", "OrderPlaced" }, { "messagetype2", "OrderBilled" } }, token);
        await File.WriteAllTextAsync(Path.Combine(targetDirectory, "Messages.cs"), messagesClass, token);
        await DotNetTemplatesHelper.Build(targetDirectory, token);
        VerifyDirectory(targetDirectory, ["Program.cs"]);
    }

    static async Task VerifyAndBuild(string templateName, CancellationToken cancellationToken, Dictionary<string, string> parameters = null, [CallerMemberName] string callerMemberName = null)
    {
        var targetDirectory = ProjectDirectory.GetSandboxPath();
        await DotNetTemplatesHelper.Run(templateName, targetDirectory, parameters, cancellationToken);
        await DotNetTemplatesHelper.Build(targetDirectory, cancellationToken);
        VerifyDirectory(targetDirectory, callerMemberName: callerMemberName);
    }

    static void VerifyDirectory(string targetDirectory, string[] fileNamesToIgnore = null, [CallerMemberName] string callerMemberName = null)
    {
        fileNamesToIgnore ??= [];

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
            fileText.AppendLine(hr);
            fileText.AppendLine(filename);
            fileText.AppendLine(hr);

            if (fileNamesToIgnore.Contains(filename))
            {
                fileText.AppendLine("Contents ignored by test");
            }
            else
            {
                var contents = File.ReadAllText(file);

                if (Path.GetExtension(filename) == ".csproj")
                {
                    contents = Regex.Replace(contents, "[Vv]ersion=\"[^\"]+\"", "Version=\"(VERSION)\"");
                }

                fileText.Append(contents);
                fileText.AppendLine();
            }
        }

        Approver.Verify(fileText.ToString(), scenario: ProjectDirectory.GetScenarioFromTestArguments(), callerMemberName: callerMemberName);
    }

    static readonly Regex IgnorePathRegex;

}