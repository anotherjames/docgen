//#load "scripts/nuget/simple-targets-csx/simple-targets-csx/6.0.0/contentFiles/csx/any/simple-targets.csx"
#load "scripts/build/process.csx"

using static SimpleTargets;

var targets = new TargetDictionary();

targets.Add("build", () => {
    Process.Run("dotnet build DocGen.sln");
});

Run(Args, targets);