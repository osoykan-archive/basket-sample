#tool "nuget:?package=xunit.runner.console"

#addin "nuget:?package=NuGet.Core"
#addin "nuget:?package=Cake.ExtendedNuGet"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var projectName = "BasketContext";
var solution = "./" + projectName + ".sln";

var target = Argument("target", "Default");
var configuration = Argument("cfg", "Release");
var toolpath = Argument("toolpath", @"tools");
var branch = Argument("branch", "develop");
var buildNumber = Argument("buildNumber", new Random().Next());
var nugetApiKey = EnvironmentVariable("nugetApiKey");

var testProjects = new List<Tuple<string, string[]>>
                {
                    new Tuple<string, string[]>("BasketContext.Api.IntegrationTests", new[] { "netcoreapp2.0" }),  
                    new Tuple<string, string[]>("BasketContext.Domain.IntegrationTests", new[] { "netcoreapp2.0" }),  
                    new Tuple<string, string[]>("BasketContext.Domain.Tests", new[] { "netcoreapp2.0" }),  
                };
                      
var nupkgPath = "nupkg";
var nupkgRegex = $"**/{projectName}*.nupkg";
var nugetPath = toolpath + "/nuget.exe";
var nugetQueryUrl = "https://www.nuget.org/api/v2/";
var nugetPushUrl = "https://www.nuget.org/api/v2/package";
var NUGET_PUSH_SETTINGS = new NuGetPushSettings
                          {
                              ToolPath = File(nugetPath),
                              Source = nugetPushUrl,
                              ApiKey = nugetApiKey
                          };

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectories("./src/**/bin");
        CleanDirectories("./src/**/obj");
        CleanDirectory(nupkgPath);
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore(solution);
    });

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
       var settings = new DotNetCoreBuildSettings
       {
           Framework = "netcoreapp2.0",
           Configuration = configuration
       };

       DotNetCoreBuild(solution, settings);

    });

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        foreach (Tuple<string, string[]> testProject in testProjects)
        {
            foreach (string targetFramework in testProject.Item2)
            {
                var testProj = GetFiles($"./test/**/*{testProject.Item1}.csproj").First();
                DotNetCoreTest(testProj.FullPath, new DotNetCoreTestSettings { Configuration = "Release", Framework = targetFramework });              
            }
        }
    });

Task("ChangeVersion")
    .IsDependentOn("Run-Unit-Tests")
    .Does(()=>
    {
        if (IsRunningOnWindows())
        {
            StartProcess("powershell", $"./changeversion.ps1 -buildNumber {buildNumber}");
        }

        if (IsRunningOnUnix())
        {
            StartProcess("./changeversion.sh", new ProcessSettings()
            {
                Arguments = $"{buildNumber} Directory.build.props",
                WorkingDirectory = "./"
            });
        }
    });
    
Task("Pack")
    .IsDependentOn("ChangeVersion")
    .Does(() =>
    {
        DotNetCorePack("./src/BasketContext.Client/BasketContext.Client.csproj");
        var nupkgFiles = GetFiles(nupkgRegex);
        MoveFiles(nupkgFiles, nupkgPath);
    });

Task("NugetPublish")
    .IsDependentOn("Pack")
    .WithCriteria(() => branch == "master")
    .Does(()=>
    {
        foreach(var nupkgFile in GetFiles(nupkgRegex))
        {
          if(!IsNuGetPublished(nupkgFile, nugetQueryUrl))
          {
             Information("Publishing... " + nupkgFile);
             NuGetPush(nupkgFile, NUGET_PUSH_SETTINGS);
          }
          else
          {
             Information("Already published, skipping... " + nupkgFile);
          }
        }
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("NugetPublish");
    
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
