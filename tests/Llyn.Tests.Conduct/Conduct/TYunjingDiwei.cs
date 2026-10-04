using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TYunjingDiwei
{
    [Fact]
    public void YunjingDiweiRead_InitialCell_SectionsByDivisionWithSortedLines()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjing.TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "蘭", "來");
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");

        CDiweiPage page = yunjing.CYunjingDiweiRead();

        Assert.Equal((language, "來"), (page.CDiweiPageLanguage, page.CDiweiPageKey));
        Assert.False(page.CDiweiPageEmpty);
        CDiweiSection section = Assert.Single(page.CDiweiPageSections);
        Assert.Equal((false, false), (section.CDiweiSectionSwitched, section.CDiweiSectionRespelled));
        CDiweiLine line = Assert.Single(section.CDiweiSectionLines);
        Assert.Equal(
            ("寒", string.Empty, false), (line.CDiweiLineLabel, line.CDiweiLineReading, line.CDiweiLineRounded));
        Assert.Equal(["爛", "蘭"], line.CDiweiLineCharacters);
    }

    [Fact]
    public void YunjingDiweiRead_BookCell_CarriesTheFontsOfTheCellLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, TYunjing.TYunjingBook, "爛", "來");
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        CDiweiPage blank = yunjing.CYunjingDiweiRead();
        yunjing.TYunjingDiweiOpen(TYunjing.TYunjingBook, LDiwei.LDiweiInitial, "來");

        CDiweiPage page = yunjing.CYunjingDiweiRead();

        Assert.Equal(new CFont(null, null, CFontSlant.CFontSlantTheme), blank.CDiweiPageFont);
        Assert.Equal(new CFont(null, null, CFontSlant.CFontSlantTheme), blank.CDiweiPageGlyph);
        const string family = "Microsoft JhengHei UI, Microsoft YaHei UI, Malgun Gothic";
        Assert.Equal(family, page.CDiweiPageFont.CFontFamily);
        Assert.Equal<double?>(40, page.CDiweiPageFont.CFontSize);
        Assert.Equal(family, page.CDiweiPageGlyph.CFontFamily);
        Assert.Equal<double?>(16, page.CDiweiPageGlyph.CFontSize);
    }

    [Fact]
    public void YunjingDiweiRead_ReflexUnderTheCell_CopiesTheTallyOfTheShownSet()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, TYunjing.TYunjingBook, "爛", "來");
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, TYunjing.TYunjingBook, "蘭", "來");
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(TYunjing.TYunjingBook, LDiwei.LDiweiInitial, "來");

        CDiweiSection section = Assert.Single(yunjing.CYunjingDiweiRead().CDiweiPageSections);

        CTally tally = Assert.Single(section.CDiweiSectionTallies);
        Assert.Equal(("Korean", string.Empty), (tally.CTallyLanguage, tally.CTallyKind));
        CTallyMark mark = Assert.Single(tally.CTallyMarks);
        Assert.Equal(("ㄹ", 2), (mark.CTallyMarkText, mark.CTallyMarkCount));
        Assert.Equal(["爛", "蘭"], mark.CTallyMarkCharacters);
    }

    [Fact]
    public void YunjingTallyToggle_SwitchThenNull_SavesOnceAndRaisesOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        yunjing.CYunjingTallyToggle(true);

        Assert.Equal(1, changed);
        Assert.True(engine.TEngineSettingsRead().LSettingsTally);

        yunjing.CYunjingTallyToggle(null);

        Assert.Equal(1, changed);
    }

    [Fact]
    public void YunjingTallyToggle_RefusedSave_ShowsTheSaveFailureAndKeepsTheState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        Dictionary<string, Func<object?[]?, object?>> refusing = new()
        {
            ["LEngineTallySave"] = _ => throw new InvalidOperationException("The settings file is unreadable."),
        };
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, refusing);
        List<string> notices = [];
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, notices));
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        yunjing.CYunjingTallyToggle(true);

        Assert.Equal(["Settings.SaveFailed"], notices);
        Assert.Equal(1, changed);
        Assert.False(engine.TEngineSettingsRead().LSettingsTally);
    }

    [Fact]
    public void YunjingGlyphSelect_PageShown_OpensTheGlyphEntryInThePageLanguage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjing.TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjing.TYunjingPrepare(atelier);
        List<long> chosen = [];
        atelier.CAtelierNavigation.TNavigationTabAdd(
            "Library", static () => true, static () => 0, static _ => { }, chosen.Add);

        yunjing.CYunjingGlyphSelect("爛");

        Assert.Empty(chosen);

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
        yunjing.CYunjingGlyphSelect(string.Empty);
        yunjing.CYunjingGlyphSelect("爛");

        Assert.Equal([engine.TEngineGlyphResolve("爛", language).LEntryId], chosen);
    }
}
