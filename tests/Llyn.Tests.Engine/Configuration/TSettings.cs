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

    [Fact]
    public void SettingsSave_AnalysisOff_RoundTripsFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", analysis: false));

        Assert.False(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsAnalysis);
        Assert.Contains(
            "\"analysis\": false",
            File.ReadAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json")));
        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en"));
        Assert.True(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsAnalysis);
    }

    [Theory]
    [InlineData("{ \"localization\": \"en\" }")]
    [InlineData("{ \"localization\": \"en\", \"analysis\": true }")]
    [InlineData("{ \"localization\": \"en\", \"analysis\": \"no\" }")]
    public void SettingsLoad_AnalysisAbsentOrNotFalse_LoadsTrue(string json)
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), json);

        Assert.True(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsAnalysis);
    }

    [Fact]
    public void AnalysisSave_Off_RaisesInflectionAndKeepsValue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        Assert.True(engine.TEngineAnalysisCheck());
        List<LBulletin> heard = [];
        engine.TEngineObserverAttach(heard.Add);

        engine.TEngineAnalysisSave(false);
        engine.TEngineAnalysisSave(false);

        Assert.False(engine.TEngineAnalysisCheck());
        Assert.False(TInterface.TSettingsLoad(workspace.TWorkspaceFolder).LSettingsAnalysis);
        LBulletin inflection = Assert.Single(
            heard, bulletin => bulletin.LBulletinSubject == LSubject.LSubjectInflection);
        Assert.Equal(0, inflection.LBulletinId);
        Assert.Single(heard, bulletin => bulletin.LBulletinSubject == LSubject.LSubjectSettings);
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
    public void SettingsLoad_LegacyBoxKeys_LoadsTheOtherFieldsAndDropsThemOnSave()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string path = Path.Combine(workspace.TWorkspaceFolder, "settings.json");
        File.WriteAllText(
            path, "{ \"localization\": \"ko\", \"tally\": true, \"fanqie\": true, \"script\": true }");

        LSettings settings = TInterface.TSettingsLoad(workspace.TWorkspaceFolder);
        TInterface.TSettingsSave(workspace.TWorkspaceFolder, settings);

        Assert.Equal("ko", settings.LSettingsLocalization);
        Assert.True(settings.LSettingsTally);
        Assert.DoesNotContain("\"fanqie\"", File.ReadAllText(path));
        Assert.DoesNotContain("\"script\"", File.ReadAllText(path));
        Assert.Equal(settings, TInterface.TSettingsLoad(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void LocalizationRead_UnlistedLanguageSaved_FallsToDefault()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineLocalizationSave("xx-Unlisted");

        Assert.Equal("xx-Unlisted", engine.TEngineSettingsRead().LSettingsLocalization);
        Assert.Equal(TInterface.TLocalizationNormalize(null), engine.TEngineLocalizationRead());
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
