using Llyn.UIDemeanor.Capsule;
using Xunit;

namespace Llyn.Tests;

public sealed class TCapsulePathfinder
{
    [Fact]
    public void CapsulePathfinder_NoFolder_StartsInStored()
    {
        string stored = Directory.CreateTempSubdirectory("llyn-capsule-").FullName;
        try
        {
            TConsoleFake console = new() { TConsoleFakeTerminal = true };
            console.TConsoleFakeKeys.Add(TCapsule.TCapsuleEscape);
            console.TConsoleFakeKeys.Add(TCapsule.TCapsuleEnter);
            console.TConsoleFakeKeys.Add(TCapsule.TCapsuleExit);
            List<string> asked = [];
            string? theme = null;

            object? answer = TEnvoy.TEnvoyRun(
                console,
                envoy =>
                {
                    _ = envoy.CEnvoyFolderRead(string.Empty);
                    LCapsuleConsole capsule = new();
                    capsule.LCapsuleConsoleSave(asked[0], new LCapsuleSetting("dusk", LCapsuleSettingFolder: stored));
                    return envoy.CEnvoyFolderRead(string.Empty);
                },
                settled: engine => theme = new LCapsuleConsole()
                    .LCapsuleConsoleRead(engine.LEngineWorkspace.LEngineWorkspaceRead()).LCapsuleSettingTheme,
                folder: path =>
                {
                    asked.Add(path);
                    return ([], []);
                });

            Assert.Equal(stored.TrimEnd('\\'), asked[^1].TrimEnd('\\'));
            Assert.Equal(stored.TrimEnd('\\'), (answer as string)?.TrimEnd('\\'));
            Assert.Equal("dusk", theme);
        }
        finally
        {
            Directory.Delete(stored, true);
        }
    }

    [Fact]
    public void CapsulePathfinder_FileAccepted_NextStartsInItsFolder()
    {
        string parent = Directory.CreateTempSubdirectory("llyn-capsule-").FullName;
        try
        {
            string file = Path.Combine(parent, "notes.llx");
            File.WriteAllText(file, string.Empty);
            TConsoleFake console = new() { TConsoleFakeTerminal = true };
            foreach (ConsoleKeyInfo key in TPathfinder.TPathfinderTextBuild(file))
            {
                console.TConsoleFakeKeys.Add(key);
            }

            console.TConsoleFakeKeys.Add(TCapsule.TCapsuleEnter);
            console.TConsoleFakeKeys.Add(TCapsule.TCapsuleEscape);
            console.TConsoleFakeKeys.Add(TCapsule.TCapsuleExit);
            List<string> asked = [];

            object? answer = TEnvoy.TEnvoyRun(
                console,
                envoy =>
                {
                    string? opened = envoy.CEnvoyMarkupRead("Markup.Title", [".llx"]);
                    _ = envoy.CEnvoyFolderRead(string.Empty);
                    return opened;
                },
                folder: path =>
                {
                    asked.Add(path);
                    return ([], []);
                });

            Assert.Equal(file, answer);
            Assert.Equal(parent.TrimEnd('\\'), asked[^1].TrimEnd('\\'));
        }
        finally
        {
            Directory.Delete(parent, true);
        }
    }

    [Fact]
    public void CapsulePathfinder_Accepted_StoresFolder()
    {
        string given = Directory.CreateTempSubdirectory("llyn-capsule-").FullName;
        try
        {
            TConsoleFake console = new() { TConsoleFakeTerminal = true };
            console.TConsoleFakeKeys.Add(TCapsule.TCapsuleEnter);
            console.TConsoleFakeKeys.Add(TCapsule.TCapsuleExit);
            LCapsuleSetting? state = null;

            object? answer = TEnvoy.TEnvoyRun(
                console,
                envoy => envoy.CEnvoyFolderRead(given),
                settled: engine => state = new LCapsuleConsole()
                    .LCapsuleConsoleRead(engine.LEngineWorkspace.LEngineWorkspaceRead()),
                folder: static _ => ([], []));

            Assert.NotNull(answer);
            Assert.Equal(answer, state?.LCapsuleSettingFolder);
        }
        finally
        {
            Directory.Delete(given, true);
        }
    }
}
