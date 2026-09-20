using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSettings
{
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
    public void SettingsLoad_LegacyPostureKeys_LoadsEngineFields()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(
            Path.Combine(workspace.TWorkspaceFolder, "settings.json"),
            "{ \"localization\": \"ko\", \"respelling\": true, \"window\": { \"left\": 1 }, "
            + "\"layout\": {}, \"mode\": \"Library\" }");

        LSettings settings = TInterface.TSettingsLoad(workspace.TWorkspaceFolder);

        Assert.Equal("ko", settings.LSettingsLocalization);
        Assert.True(settings.LSettingsRespelled);
    }

    [Fact]
    public void SettingsChange_Echo_RaisesNoBulletin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSettings settings = engine.TEngineSettingsRead();
        int count = 0;
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectSettings)
            {
                count++;
            }
        });

        engine.TEngineLocalizationSave(settings.LSettingsLocalization);
        engine.TEngineRespellingSave(settings.LSettingsRespelled);
        engine.TEngineEpithetSave(settings.LSettingsEpithet);
        engine.TEngineFrequencySave(settings.LSettingsFrequency);
        engine.TEngineMorphologySave(settings.LSettingsMorphology);

        Assert.Equal(0, count);
    }

    [Fact]
    public void SettingsChange_Changed_RaisesBulletinOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSettings settings = engine.TEngineSettingsRead();
        int count = 0;
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectSettings)
            {
                count++;
            }
        });

        engine.TEngineEpithetSave(!settings.LSettingsEpithet);
        engine.TEngineEpithetSave(!settings.LSettingsEpithet);

        Assert.Equal(1, count);
    }
}
