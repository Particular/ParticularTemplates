using System.IO;
using NUnit.Framework;

public static class ProjectDirectory
{
    static ProjectDirectory()
    {
        ProjectPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..");
        SandboxPath = Path.Combine(ProjectPath, "..", "..", "tempstorage", "sandbox");
    }

    public static string GetSandboxPath(string suffix)
    {
        var path = Path.Combine(SandboxPath, suffix);

        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        return path;
    }

    public static string SandboxPath { get; }

    public static string ProjectPath { get; }
}
