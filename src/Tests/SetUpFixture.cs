using System;
using NUnit.Framework;

[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public void SetUp()
    {
        Environment.CurrentDirectory = ProjectDirectory.ProjectPath;
    }
}