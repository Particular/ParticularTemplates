using System;

static class ServiceHelper
{
    public static bool IsService()
    {
        return !Environment.UserInteractive;
    }
}