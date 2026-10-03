using System.Collections.Generic;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineGloss
{
    [Fact]
    public void GlossRead_DefaultSettingsWithEnglishLoaded_PicksEnglish()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Contains("English", engine.TEngineLanguageRead());
        Assert.Equal("English", engine.TEngineGlossRead());
    }

    [Fact]
    public void GlossRead_SettingsNameUnloadedLanguage_PicksFirstLoaded()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TInterface.TSettingsSave(workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", gloss: "Klingon"));
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<string> languages = engine.TEngineLanguageRead();

        Assert.DoesNotContain("Klingon", languages);
        Assert.Equal(languages[0], engine.TEngineGlossRead());
    }
}
