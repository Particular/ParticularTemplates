using System.IO;
using System.Runtime.CompilerServices;

public static class ProjectDirectory
{
    static ProjectDirectory()
    {
        ProjectPath = GetCurrentDirectory();
        SandboxPath = Path.Combine(ProjectPath, "../../tempstorage/sandbox");
    }

    public static string GetSandboxPath(string suffix)
    {
        var path = Path.Combine(SandboxPath, suffix);
        if (Directory.Exists(path))
        {
            Directory.Delete(path,true);
        }
        return path;
    }

    public static string SandboxPath;

    public static string ProjectPath;

    static string GetCurrentDirectory([CallerFilePath] string callerFilePath = "")
    {
        return Directory.GetParent(callerFilePath).FullName;
    }
}
