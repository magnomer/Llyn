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
}
