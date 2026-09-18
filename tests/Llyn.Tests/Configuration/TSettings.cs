using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSettings
{
    [Fact]
    public void LocalizationSave_AfterWindowSave_KeepsBothChanges()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineWindowSave(TInterface.TWindowStateCreate(10, 20, 800, 600, false));
        engine.TEngineLocalizationSave("ko");

        LSettings settings = engine.TEngineSettingsRead();

        Assert.Equal("ko", settings.LSettingsLocalization);
        Assert.Equal(800, settings.LSettingsWindow?.LWindowStateWidth);
    }

    [Fact]
    public void WindowSave_AfterLocalizationSave_KeepsBothChanges()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineLocalizationSave("ko");
        engine.TEngineWindowSave(TInterface.TWindowStateCreate(10, 20, 640, 480, true));

        LSettings settings = engine.TEngineSettingsRead();

        Assert.Equal("ko", settings.LSettingsLocalization);
        Assert.Equal(640, settings.LSettingsWindow?.LWindowStateWidth);
        Assert.True(settings.LSettingsWindow?.LWindowStateMaximized);
    }

    [Fact]
    public void LocalizationSave_WorkspaceReopened_KeepsBothChanges()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            engine.TEngineLocalizationSave("ko");
            engine.TEngineWindowSave(TInterface.TWindowStateCreate(10, 20, 640, 480, false));
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        LSettings settings = reopened.TEngineSettingsRead();

        Assert.Equal("ko", settings.LSettingsLocalization);
        Assert.Equal(640, settings.LSettingsWindow?.LWindowStateWidth);
    }

    [Fact]
    public void SettingsSave_Respelled_LoadsTrue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", respelled: true));

        Assert.True(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsRespelled);
    }

    [Theory]
    [InlineData("{ \"localization\": \"en\" }")]
    [InlineData("{ \"localization\": \"en\", \"respelling\": \"yes\" }")]
    [InlineData("{ \"localization\": \"en\", \"respelling\": 1 }")]
    public void SettingsLoad_KeyAbsentOrNotBoolean_LoadsFalse(string json)
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), json);

        Assert.False(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsRespelled);
    }

    [Fact]
    public void SettingsSave_FrequencyOff_RoundTripsFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", frequency: false));

        Assert.False(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsFrequency);
        Assert.Contains(
            "\"frequency\": false",
            File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json")));
    }

    [Theory]
    [InlineData("{ \"localization\": \"en\" }")]
    [InlineData("{ \"localization\": \"en\", \"frequency\": true }")]
    [InlineData("{ \"localization\": \"en\", \"frequency\": \"no\" }")]
    public void SettingsLoad_FrequencyAbsentOrNotFalse_LoadsTrue(string json)
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), json);

        Assert.True(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsFrequency);
    }

    [Fact]
    public void SettingsSave_MorphologyOff_RoundTripsFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", morphology: false));

        Assert.False(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsMorphology);
        Assert.Contains(
            "\"morphology\": false",
            File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json")));
    }

    [Theory]
    [InlineData("{ \"localization\": \"en\" }")]
    [InlineData("{ \"localization\": \"en\", \"morphology\": true }")]
    [InlineData("{ \"localization\": \"en\", \"morphology\": \"no\" }")]
    public void SettingsLoad_MorphologyAbsentOrNotFalse_LoadsTrue(string json)
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), json);

        Assert.True(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsMorphology);
    }

    [Fact]
    public void SettingsSave_EpithetOff_RoundTripsFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", epithet: false));

        Assert.False(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsEpithet);
        Assert.Contains(
            "\"epithet\": false",
            File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json")));
    }

    [Fact]
    public void RespellingSave_True_ReadsBackAndWritesKey()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineRespellingSave(true);

        Assert.True(engine.TEngineSettingsRead().LSettingsRespelled);
        Assert.Contains(
            "\"respelling\": true",
            File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json")));
    }

    [Fact]
    public void SettingsSave_TallyOn_RoundTripsTrue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", tally: true));

        Assert.True(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsTally);
        Assert.Contains(
            "\"tally\": true",
            File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json")));
    }

    [Theory]
    [InlineData("{ \"localization\": \"en\" }")]
    [InlineData("{ \"localization\": \"en\", \"tally\": \"yes\" }")]
    public void SettingsLoad_TallyAbsentOrNotTrue_LoadsFalse(string json)
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), json);

        Assert.False(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsTally);
    }

    [Fact]
    public void TallySave_True_ReadsBackAndWritesKey()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineTallySave(true);

        Assert.True(engine.TEngineSettingsRead().LSettingsTally);
        Assert.Contains(
            "\"tally\": true",
            File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json")));
    }

    [Fact]
    public void SettingsSave_Layout_RoundTripsPerTab()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        LLayout[] layout =
        [
            TInterface.TLayoutCreate("library", 420),
            TInterface.TLayoutCreate("taxonomy", 310, 280)
        ];

        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", layout: layout));

        LSettings loaded = TInterface.TSettingsLoad(workspace.TWorkspaceFolder);

        Assert.NotNull(loaded.LSettingsLayout);
        Assert.Equal(2, loaded.LSettingsLayout.Count);
        Assert.Contains(
            loaded.LSettingsLayout,
            tab => tab.LLayoutTab == "library" && tab.LLayoutLeft == 420 && tab.LLayoutMiddle is null);
        Assert.Contains(
            loaded.LSettingsLayout,
            tab => tab.LLayoutTab == "taxonomy" && tab.LLayoutLeft == 310 && tab.LLayoutMiddle == 280);
    }

    [Fact]
    public void SettingsSave_LinkedOff_RoundTripsFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", linked: false));

        Assert.False(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsLinked);
        Assert.Contains(
            "\"linked\": false",
            File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json")));
    }

    [Theory]
    [InlineData("{ \"localization\": \"en\" }")]
    [InlineData("{ \"localization\": \"en\", \"linked\": true }")]
    [InlineData("{ \"localization\": \"en\", \"linked\": \"no\" }")]
    public void SettingsLoad_LinkedAbsentOrNotFalse_LoadsTrue(string json)
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), json);

        Assert.True(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsLinked);
    }

    [Theory]
    [InlineData("{ \"localization\": \"en\" }")]
    [InlineData("{ \"localization\": \"en\", \"layout\": 5 }")]
    [InlineData("{ \"localization\": \"en\", \"layout\": { \"library\": { \"left\": -1 }, \"tenor\": \"wide\" } }")]
    public void SettingsLoad_LayoutAbsentOrUnusable_LoadsNoTab(string json)
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), json);

        Assert.Empty(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsLayout ?? []);
    }

    [Fact]
    public void LayoutSave_SecondTab_KeepsFirstTab()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineLayoutSave(TInterface.TLayoutCreate("library", 400));
        engine.TEngineLayoutSave(TInterface.TLayoutCreate("corpus", 390), TInterface.TLayoutCreate("library", 410));

        IReadOnlyList<LLayout> layout = engine.TEngineSettingsRead().LSettingsLayout ?? [];

        Assert.Equal(2, layout.Count);
        Assert.Contains(layout, tab => tab.LLayoutTab == "library" && tab.LLayoutLeft == 410);
        Assert.Contains(layout, tab => tab.LLayoutTab == "corpus" && tab.LLayoutLeft == 390);
    }

    [Fact]
    public void SettingsLoad_BrokenJson_LoadsDefaultsAndKeepsCopy()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), "{ not json");

        LSettings settings = TInterface.TSettingsLoad(workspace.TWorkspaceFolder);

        Assert.Equal("en", settings.LSettingsLocalization);
        Assert.Equal("{ not json", File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.broken.json")));
    }

    [Fact]
    public void SettingsSave_Written_LeavesNoPendingFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("ko"));

        Assert.True(TInterface.TSettingsExist(workspace.TWorkspaceFolder));
        Assert.False(File.Exists(Path.Combine(workspace.TWorkspaceFolder, "settings.json.tmp")));
        Assert.Equal("ko", TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsLocalization);
    }

    [Fact]
    public void SettingsExist_NoFile_ReportsFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        Assert.False(TInterface.TSettingsExist(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void WindowSave_SameGeometryTwice_WritesFileOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string path = Path.Combine(workspace.TWorkspaceFolder, "settings.json");

        engine.TEngineWindowSave(TInterface.TWindowStateCreate(10, 20, 800, 600, false));
        DateTime written = File.GetLastWriteTimeUtc(path);
        File.SetLastWriteTimeUtc(path, written.AddMinutes(-5));

        engine.TEngineWindowSave(TInterface.TWindowStateCreate(10, 20, 800, 600, false));

        Assert.Equal(written.AddMinutes(-5), File.GetLastWriteTimeUtc(path));
    }

    [Fact]
    public void LinkedSave_False_KeepsLayout()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineLayoutSave(TInterface.TLayoutCreate("tenor", 320, 300));
        engine.TEngineLinkedSave(false);

        LSettings settings = engine.TEngineSettingsRead();

        Assert.False(settings.LSettingsLinked);
        Assert.Single(settings.LSettingsLayout ?? []);
    }

    [Fact]
    public void SettingsChange_Echo_RaisesNoBulletin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSettings settings = engine.TEngineSettingsRead();
        TSettingsObserver observer = new();
        engine.TEngineObserverAttach(observer);

        engine.TEngineLocalizationSave(settings.LSettingsLocalization);
        engine.TEngineRespellingSave(settings.LSettingsRespelled);
        engine.TEngineEpithetSave(settings.LSettingsEpithet);
        engine.TEngineFrequencySave(settings.LSettingsFrequency);
        engine.TEngineMorphologySave(settings.LSettingsMorphology);
        engine.TEngineLinkedSave(settings.LSettingsLinked);

        Assert.Equal(0, observer.TSettingsObserverCount);
    }

    [Fact]
    public void SettingsChange_Changed_RaisesBulletinOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSettings settings = engine.TEngineSettingsRead();
        TSettingsObserver observer = new();
        engine.TEngineObserverAttach(observer);

        engine.TEngineEpithetSave(!settings.LSettingsEpithet);
        engine.TEngineEpithetSave(!settings.LSettingsEpithet);

        Assert.Equal(1, observer.TSettingsObserverCount);
    }

    private sealed class TSettingsObserver : LObserver
    {
        internal int TSettingsObserverCount { get; private set; }

        public void LObserverBulletinHandle(LBulletin bulletin)
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectSettings)
            {
                TSettingsObserverCount++;
            }
        }
    }
}
