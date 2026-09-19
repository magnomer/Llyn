using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TDiweiPage
{
    [Fact]
    public void DiweiRead_InitialCell_SectionsByDivisionWithSortedLines()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        TYunjingLanguage.TYunjingDiweiPlace(engine, workspace, language, "爛", "來");
        TYunjingLanguage.TYunjingDiweiPlace(engine, workspace, language, "蘭", "來");
        LYunjing panel = TYunjingLanguage.TYunjingPrepare(engine);
        panel.TYunjingDiweiShow(language, LDiwei.LDiweiInitial, "來");

        LDiweiPage page = panel.TYunjingDiweiRead();

        Assert.Equal(language, page.LDiweiPageLanguage);
        Assert.Equal("來", page.LDiweiPageKey);
        Assert.False(page.LDiweiPageFinal);
        Assert.False(page.LDiweiPageEmpty);
        LDiweiSection section = Assert.Single(page.LDiweiPageSections);
        Assert.Equal("一", section.LDiweiSectionHeading);
        Assert.Equal("一", section.LDiweiSectionLabel);
        LDiweiLine line = Assert.Single(section.LDiweiSectionLines);
        Assert.Equal("寒", line.LDiweiLineLabel);
        Assert.Equal(["爛", "蘭"], line.LDiweiLineCharacters);
    }

    [Fact]
    public void DiweiRead_PageHidden_IsBlank()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LYunjing panel = TYunjingLanguage.TYunjingPrepare(engine);

        Assert.False(panel.LYunjingDiweiShown);
        Assert.True(panel.TYunjingDiweiRead().LDiweiPageEmpty);
    }

    [Fact]
    public void TallySet_Switch_SavesAndReReads()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LYunjing panel = TYunjingLanguage.TYunjingPrepare(engine);
        int changed = 0;
        panel.LYunjingChanged += () => changed++;

        panel.TYunjingTallySet(true);

        Assert.Equal(1, changed);
        Assert.True(engine.TEngineSettingsRead().LSettingsTally);

        panel.TYunjingTallySet(null);

        Assert.Equal(1, changed);
    }
}
