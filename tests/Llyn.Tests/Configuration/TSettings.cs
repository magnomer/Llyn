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
}
