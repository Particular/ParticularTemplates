using System.IO;
using System.Linq;
using NUnit.Framework;

public static class ProjectDirectory
{
    static ProjectDirectory()
    {
        ProjectPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..");
        SandboxPath = Path.Combine(ProjectPath, "..", "..", "tempstorage", "sandbox");
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
