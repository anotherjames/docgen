#load "nuget:simple-targets-csx, 6.0.0"
#load "scripts/build/process.csx"
using static SimpleTargets;

var targets = new TargetDictionary();

targets.Add("test", () => {
    Process.Run("dotnet test test/DocGen.Core.Tests/");
    Process.Run("dotnet test test/DocGen.Requirements.Tests/");
    Process.Run("dotnet test test/DocGen.Web.Tests/");
    Process.Run("dotnet test test/DocGen.Web.Requirements.Tests/");
});

targets.Add("build", SimpleTargets.DependsOn("test"), () => {
    Process.Run("dotnet build DocGen.sln");
});

Run(Args, targets);