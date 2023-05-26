using System.IO;
using System.Linq;
using NUnit.Framework;

public static class ProjectDirectory
{
    static ProjectDirectory()
    {
        ProjectPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", ".."));
        var srcPath = Path.GetFullPath(Path.Combine(ProjectPath, ".."));
        var templatesPath = Path.Combine(srcPath, "Templates");
        var tempStorage = Path.GetFullPath(Path.Combine(ProjectPath, "..", "..", "tempstorage"));
        SandboxPath = Path.Combine(tempStorage, "sandbox");

        // Copy company-standard editorconfig
        Directory.CreateDirectory(SandboxPath);
        File.Copy(Path.Combine(srcPath, ".editorconfig"), Path.Combine(tempStorage, ".editorconfig"), true);

        // Copy editorconfig overrides
        File.Copy(Path.Combine(templatesPath, ".editorconfig"), Path.Combine(SandboxPath, ".editorconfig"), true);
    }

    public static string GetSandboxPath()
    {
        var suffix = TestContext.CurrentContext.Test.MethodName + GetScenarioFromTestArguments();
        var path = Path.Combine(SandboxPath, suffix);

        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        return path;
    }

    public static string GetScenarioFromTestArguments()
    {
        if (TestContext.CurrentContext.Test.Arguments.Any())
        {
            return string.Join("-", TestContext.CurrentContext.Test.Arguments.Select(arg => arg.ToString()));
        }
        return null;
    }

    public static string SandboxPath { get; }

    public static string ProjectPath { get; }
}
