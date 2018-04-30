#load "nuget:simple-targets-csx, 6.0.0"
#load "process.csx"
#load "path.csx"
using static SimpleTargets;

var targets = new TargetDictionary();

targets.Add("clean", () => {
    Path.CleanDirectory(Path.Expand("./output"));
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
    Process.Run($"dotnet publish --output {Path.Expand("./output")}");
});

targets.Add("default", SimpleTargets.DependsOn("build"));

Run(Args, targets);