using System;
using System.Threading.Tasks;
using Build.Buildary;
using static Bullseye.Targets;
using static Build.Buildary.Directory;
using static Build.Buildary.Path;
using static Build.Buildary.Shell;
using static Build.Buildary.Runner;
using static Build.Buildary.GitVersion;
using static Build.Buildary.File;

namespace Build
{
    static class Program
    {
        static Task<int> Main(string[] args)
        {
            var options = ParseOptions<Options>(args);
            var gitversion = GetGitVersion("./");

            var commandBuildArgs = $"--configuration {options.Configuration}";
            if (!string.IsNullOrEmpty(gitversion.PreReleaseTag))
            {
                commandBuildArgs += $" --version-suffix \"{gitversion.PreReleaseTag}\"";
            }
            
            Add("clean", () =>
            {
                CleanDirectory(ExpandPath("./output"));
            });
            
            Add("build", () =>
            {
                RunShell($"dotnet build DocGen.sln {commandBuildArgs}");
            });
            
            Add("test", () =>
            {
                RunShell("dotnet test test/DocGen.Core.Tests/");
                RunShell("dotnet test test/DocGen.Requirements.Tests/");
                RunShell("dotnet test test/DocGen.Web.Requirements.Tests/");
            });
            
            Add("deploy", DependsOn("clean"), () => {
                // Deploy the console project in each of our target runtimes.
                RunShell($"dotnet publish src/DocGen.Cons/ --output {ExpandPath("./output/console/osx-x64")} --runtime osx-x64 {commandBuildArgs}");
                RunShell($"dotnet publish src/DocGen.Cons/ --output {ExpandPath("./output/console/win-x64")} --runtime win-x64 {commandBuildArgs}");
                RunShell($"dotnet publish src/DocGen.Cons/ --output {ExpandPath("./output/console/linux-x64")} --runtime linux-x64 {commandBuildArgs}");
                // The dotnet cli doesn't publish the entry points with executable permissions. No idea why.
                RunShell("chmod +x ./output/console/osx-x64/DocGen.Cons");
                RunShell("chmod +x ./output/console/linux-x64/DocGen.Cons");
                // Deploy our nuget packages.
                RunShell($"dotnet pack --output {ExpandPath("./output/nuget")} {commandBuildArgs}"); 
                // Deploy our node scripts/configuration for later publish to npm. 
                RunShell("cp -r ./src/DocGen.Cons/node-launcher/. ./output/console/");
                foreach(var file in new[] {
                    "./output/console/package.json",
                    "./output/console/osx-x64/package.json",
                    "./output/console/win-x64/package.json",
                    "./output/console/linux-x64/package.json"
                }) {
                    ReplaceInFile(file, "VERSION", gitversion.FullVersion);
                }
            });
            
            Add("update-version", () =>
            {
                if (FileExists("./build/version.props"))
                {
                    DeleteFile("./build/version.props");
                }
                
                WriteFile("./build/version.props",
                    $@"<Project>
    <PropertyGroup>
        <VersionPrefix>{gitversion.Version}</VersionPrefix>
    </PropertyGroup>
</Project>");
            });
            
            Add("publish", () =>
            {
                if(Travis.IsTravis)
                {
                    // If we are on travis, we only want to deploy if this is a release tag.
                    if(Travis.EventType != Travis.EventTypeEnum.Push)
                    {
                        // Only pushes (no cron jobs/api/pull rqeuests) can deploy.
                        Log.Warning("Not a push build, skipping publish...");
                        return;
                    }

                    if(Travis.Branch != "master")
                    {
                        // We aren't on master.
                        Log.Warning("Not on master, skipping publish...");
                        return;
                    }
                }
                
                Add("ci", DependsOn("update-version", "test", "deploy", "publish"));

                // For now, we are only deploying npm packages.
                RunShell("cd ./output/console/ && npm publish");
                RunShell("cd ./output/console/osx-x64/ && npm publish");
                RunShell("cd ./output/console/win-x64/ && npm publish");
                RunShell("cd ./output/console/linux-x64/ && npm publish");
            });

            
            Add("default", DependsOn("build"));

            return Run(options);
        }

        // ReSharper disable ClassNeverInstantiated.Local
        class Options : RunnerOptions
        // ReSharper restore ClassNeverInstantiated.Local
        {
            [PowerArgs.ArgShortcut("config"), PowerArgs.ArgDefaultValue("Release")]
            public string Configuration { get; set; }
        }
    }
}