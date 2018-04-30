#load "nuget:simple-targets-csx, 6.0.0"
#load "process.csx"
#load "path.csx"
#load "runner.csx"

using static SimpleTargets;
using static Runner;

var options = ParseOptions(Args);

var targets = new TargetDictionary();

targets.Add("clean", () => {
    Path.CleanDirectory(Path.Expand("./output"));
});

targets.Add("build", () => {
    Process.Run($"dotnet build DocGen.sln --configuration {options.Configuration}");
});

targets.Add("test", () => {
    Process.Run($"dotnet test test/DocGen.Core.Tests/ --configuration {options.Configuration}");
    Process.Run($"dotnet test test/DocGen.Requirements.Tests/ --configuration {options.Configuration}");
    Process.Run($"dotnet test test/DocGen.Web.Tests/");
    Process.Run($"dotnet test test/DocGen.Web.Requirements.Tests/ --configuration {options.Configuration}");
});

targets.Add("deploy", SimpleTargets.DependsOn("clean"), () => {
    // Deploy the console project in each of our target runtimes.
    Process.Run($"dotnet publish src/DocGen.Cons/ --output {Path.Expand("./output/console/osx-x64")} --runtime osx-x64 --configuration {options.Configuration}");
    Process.Run($"dotnet publish src/DocGen.Cons/ --output {Path.Expand("./output/console/win-x64")} --runtime win-x64 --configuration {options.Configuration}");
    Process.Run($"dotnet publish src/DocGen.Cons/ --output {Path.Expand("./output/console/linux-x64")} --runtime linux-x64 --configuration {options.Configuration}");
    // Deploy nuget packages
    Process.Run($"dotnet pack --output {Path.Expand("./output/nuget")} --configuration {options.Configuration}");   
});

targets.Add("default", SimpleTargets.DependsOn("build"));

Runner.Run(options, targets);