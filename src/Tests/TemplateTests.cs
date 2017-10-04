using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ApprovalTests;
using NUnit.Framework;

[TestFixture]
public class TemplateTests:IDisposable
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
        //  DotNetTemplatesHelper.Uninstall("ParticularTemplates");
    }

    public void Dispose()
    {
        Uninstall();
    }

    [Test]
    public void WindowsService()
    {
        //dotnet new nsbservice
        using (var tempDirHelper = new TempDirHelper())
        {
            DotNetTemplatesHelper.Run("nsbservice", ProjectDirectory.Current);
            Debug.WriteLine(tempDirHelper.Current);
            //Approvals.VerifyFile();
        }
    }

}