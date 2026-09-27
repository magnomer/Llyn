using Llyn.Conduct;
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

        CDiweiPage page = panel.TYunjingDiweiRead();

        Assert.Equal(language, page.CDiweiPageLanguage);
        Assert.Equal("來", page.CDiweiPageKey);
        Assert.False(page.CDiweiPageEmpty);
        CDiweiSection section = Assert.Single(page.CDiweiPageSections);
        Assert.Equal("一", section.CDiweiSectionLabel);
        CDiweiLine line = Assert.Single(section.CDiweiSectionLines);
        Assert.Equal("寒", line.CDiweiLineLabel);
        Assert.Equal(["爛", "蘭"], line.CDiweiLineCharacters);
    }

    [Fact]
    public void DiweiRead_PageHidden_IsBlank()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LYunjing panel = TYunjingLanguage.TYunjingPrepare(engine);

        Assert.False(panel.LYunjingDiweiShown);
        Assert.True(panel.TYunjingDiweiRead().CDiweiPageEmpty);
    }

    [Fact]
    public void PageBuild_BlankPage_CarriesNoSection()
    {
        CDiweiPage page = TInterfaceGate.TYunjingPageBuild(TInterfaceGate.TDiweiPageBlank);

        Assert.Empty(page.CDiweiPageKey);
        Assert.Empty(page.CDiweiPageLanguage);
        Assert.Empty(page.CDiweiPageSections);
        Assert.True(page.CDiweiPageEmpty);
    }

    [Fact]
    public void PageBuild_Section_KeepsLineAndSwitch()
    {
        CDiweiSection section = Assert.Single(
            TInterfaceGate.TYunjingPageBuild(TInterfaceGate.TDiweiPageCreate(false)).CDiweiPageSections);

        Assert.Equal("一", section.CDiweiSectionLabel);
        Assert.True(section.CDiweiSectionSwitched);
        Assert.False(section.CDiweiSectionRespelled);
        CDiweiLine line = Assert.Single(section.CDiweiSectionLines);
        Assert.Equal("/l/", line.CDiweiLineReading);
        Assert.True(line.CDiweiLineRounded);
        Assert.Equal(["爛", "蘭"], line.CDiweiLineCharacters);
    }

    [Theory]
    [InlineData(false, "l", "爛")]
    [InlineData(true, "L", "蘭")]
    public void PageBuild_Tally_TakesTheChosenSet(bool respelled, string text, string character)
    {
        CDiweiSection section = Assert.Single(
            TInterfaceGate.TYunjingPageBuild(TInterfaceGate.TDiweiPageCreate(respelled)).CDiweiPageSections);

        CTally tally = Assert.Single(section.CDiweiSectionTallies);
        Assert.Equal("Cantonese", tally.CTallyLanguage);
        Assert.Equal("literary", tally.CTallyKind);
        CTallyMark mark = Assert.Single(tally.CTallyMarks);
        Assert.Equal(text, mark.CTallyMarkText);
        Assert.Equal(1, mark.CTallyMarkCount);
        Assert.Equal([character], mark.CTallyMarkCharacters);
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
