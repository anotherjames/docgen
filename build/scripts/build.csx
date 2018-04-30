#load "nuget:simple-targets-csx, 6.0.0"
#load "process.csx"
#load "path.csx"
#load "runner.csx"
#load "gitversion.csx"
#load "log.csx"

using static SimpleTargets;
using static Runner;

var options = ParseOptions(Args);
var gitversion = GitVersion.Get("./");

Log.Info($"Version: {gitversion.FullVersion}");

var commandBuildArgs = $"--configuration {options.Configuration} --version-suffix \"{gitversion.PreReleaseTag}\"";

var targets = new TargetDictionary();

targets.Add("clean", () => {
    Path.CleanDirectory(Path.Expand("./output"));
});

targets.Add("build", () => {
    Process.Run($"dotnet build DocGen.sln {commandBuildArgs}");
});

targets.Add("test", () => {
    Process.Run($"dotnet test test/DocGen.Core.Tests/");
    Process.Run($"dotnet test test/DocGen.Requirements.Tests/");
    Process.Run($"dotnet test test/DocGen.Web.Tests/");
    Process.Run($"dotnet test test/DocGen.Web.Requirements.Tests/");
});

targets.Add("deploy", SimpleTargets.DependsOn("clean"), () => {
    // Deploy the console project in each of our target runtimes.
    Process.Run($"dotnet publish src/DocGen.Cons/ --output {Path.Expand("./output/console/osx-x64")} --runtime osx-x64 {commandBuildArgs}");
    Process.Run($"dotnet publish src/DocGen.Cons/ --output {Path.Expand("./output/console/win-x64")} --runtime win-x64 {commandBuildArgs}");
    Process.Run($"dotnet publish src/DocGen.Cons/ --output {Path.Expand("./output/console/linux-x64")} --runtime linux-x64 {commandBuildArgs}");
    // Deploy nuget packages
    Process.Run($"dotnet pack --output {Path.Expand("./output/nuget")} {commandBuildArgs}");   
});

targets.Add("update-version", () => {
    if(Path.FileExists("./build/version.props")) Path.DeleteFile("./build/version.props");
    Path.WriteFile("./build/version.props",
$@"<Project>
    <PropertyGroup>
        <VersionPrefix>{gitversion.Version}</VersionPrefix>
    </PropertyGroup>
</Project>"
);
});

targets.Add("default", SimpleTargets.DependsOn("build"));

Runner.Run(options, targets);