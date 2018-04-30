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
    // Deploy our nuget packages.
    Process.Run($"dotnet pack --output {Path.Expand("./output/nuget")} {commandBuildArgs}"); 
    // Deploy our node scripts/configuration for later publish to npm. 
    Process.Run("cp -r ./src/DocGen.Cons/node-launcher/. ./output/console/");
    foreach(var file in new string[] {
        "./output/console/package.json",
        "./output/console/osx-x64/package.json",
        "./output/console/win-x64/package.json",
        "./output/console/linux-x64/package.json"
    }) {
        Path.ReplaceInFile(file, "VERSION", gitversion.FullVersion);
    }
});

targets.Add("update-version", () => {
    if(Path.FileExists("./build/version.props")) Path.DeleteFile("./build/version.props");
    Path.WriteFile("./build/version.props",
$@"<Project>
    <PropertyGroup>
        <VersionPrefix>{gitversion.Version}</VersionPrefix>
    </PropertyGroup>
</Project>");
});

targets.Add("publish", () => {
    // For now, we are only deploying npm packages.
    Process.Run("cd ./output/console/ && npm publish");
    Process.Run("cd ./output/console/osx-x64/ && npm publish");
    Process.Run("cd ./output/console/win-x64/ && npm publish");
    Process.Run("cd ./output/console/linux-x64/ && npm publish");
});

targets.Add("ci", DependsOn("update-version", "test", "deploy"), () => {

});

targets.Add("default", SimpleTargets.DependsOn("build"));

Runner.Run(options, targets);