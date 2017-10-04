using System;
using System.IO;

public class TempDirHelper : IDisposable
{
    public TempDirHelper()
    {
        Current = Path.GetTempFileName();
        Directory.CreateDirectory(Current);
    }

    public string Current;

    public void Dispose()
    {
        Directory.Delete(Current);
    }
}