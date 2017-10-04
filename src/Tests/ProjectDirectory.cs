using System.IO;
using System.Runtime.CompilerServices;

public static class ProjectDirectory
{
    static ProjectDirectory()
    {
        ProjectPath = GetCurrentDirectory();
        SandboxPath = Path.Combine(ProjectPath, "../../sandbox");
    }

    public static string SandboxPath;

    public static string ProjectPath;

    static string GetCurrentDirectory([CallerFilePath] string callerFilePath = "")
    {
        return Directory.GetParent(callerFilePath).FullName;
    }
}