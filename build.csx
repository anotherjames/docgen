#load "nuget:simple-targets-csx, 6.0.0"
#load "scripts/build/process.csx"
#load "scripts/build/directory.csx"
using static SimpleTargets;

var targets = new TargetDictionary();

targets.Add("clean", () => {
    Directory.Clean("");
});

targets.Add("build", () => {
    Process.Run("dotnet build DocGen.sln");
});

targets.Add("test", () => {
    Process.Run("dotnet test test/DocGen.Core.Tests/");
    Process.Run("dotnet test test/DocGen.Requirements.Tests/");
    Process.Run("dotnet test test/DocGen.Web.Tests/");
    Process.Run("dotnet test test/DocGen.Web.Requirements.Tests/");
});

targets.Add("deploy", SimpleTargets.DependsOn("clean"), () => {
    System.Console.WriteLine("Test");
});

targets.Add("default", SimpleTargets.DependsOn("build"));

Run(Args, targets);