using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSheetEntry
{
    private const string TSheetEntryFlag = "data:image/png;base64,iVBORw0KGgo=";

    [Fact]
    public void SheetEntryFormat_StoredEntry_PaintsTheSurfaceWithThePaneInsets()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);

        string sheet = TSheetTextFormat(engine, entry);

        Assert.Contains("<main class=\"entry\">", sheet, StringComparison.Ordinal);
        Assert.Contains("body{background:var(--surface);color:var(--ink);", sheet, StringComparison.Ordinal);
        Assert.Contains(
            ".entry{display:block;margin:0;padding:18px 10px 0 30px;text-align:left;}",
            sheet,
            StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_VarietyWithFlag_ShowsFlagInsteadOfLabel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        var flag = new Dictionary<string, string> { ["Brazil"] = TSheetEntryFlag };
        LLiveryPage page = TLiveryVarietyCreate(engine, entry, flag);

        string sheet = TSheetTextFormat(engine, entry, page);

        Assert.Contains(
            "<tr class=\"accent primary\"><th><span class=\"flag ensign-0\" title=\"Brazil\"></span></th>",
            sheet,
            StringComparison.Ordinal);
        Assert.DoesNotContain("<th>Brazil</th>", sheet, StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_VarietyWithoutFlag_ShowsLabel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        LLiveryPage page = TLiveryVarietyCreate(engine, entry, new Dictionary<string, string>());

        string sheet = TSheetTextFormat(engine, entry, page);

        Assert.Contains("<tr class=\"accent primary\"><th>Brazil</th>", sheet, StringComparison.Ordinal);
        Assert.DoesNotContain("title=\"Brazil\"", sheet, StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_LanguageWithoutFlag_ShowsGlobe()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        LLiveryPage page = TSheetEntryRead(engine, entry) with { LLiveryPageBanner = new Dictionary<string, string>() };

        string sheet = TSheetTextFormat(engine, entry, page);

        Assert.Contains(
            "<span class=\"language\"><span class=\"globe\"></span><span class=\"name\">English</span></span>",
            sheet,
            StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_GraspThree_FillsOneFullAndOneHalfStar()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        engine.TEngineGraspSave(entry.LEntryId, 3);

        string sheet = TSheetTextFormat(engine, entry);

        Assert.Contains(
            "<span class=\"grasp\"><span class=\"star full\"></span><span class=\"star half\"></span>"
            + "<span class=\"star\"></span><span class=\"star\"></span><span class=\"star\"></span></span>",
            sheet,
            StringComparison.Ordinal);
        Assert.Contains("<span class=\"rating\">Grasp.Level3</span>", sheet, StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_GraspAboveStep_ClampsToTopLabel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        LLiveryPage page = TSheetEntryRead(engine, entry) with { LLiveryPageGrasp = 12 };

        string sheet = TSheetTextFormat(engine, entry, page);

        Assert.Contains("<span class=\"rating\">Grasp.Level10</span>", sheet, StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_Unrated_ShowsNotRatedLabel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);

        string sheet = TSheetTextFormat(engine, entry);

        Assert.Contains("<span class=\"rating\">Grasp.Level0</span>", sheet, StringComparison.Ordinal);
        Assert.DoesNotContain("class=\"star full\"", sheet, StringComparison.Ordinal);
        Assert.DoesNotContain("class=\"star half\"", sheet, StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_RankedFrequency_WritesOnePillWithSourceTitle()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        LLiveryPage page = TSheetEntryRead(engine, entry) with
        {
            LLiveryPageFrequency = [TInterface.TFrequencyCreate("Subtlex", "12", "Everyday")],
        };

        string sheet = TSheetTextFormat(engine, entry, page);

        Assert.Equal(2, sheet.Split("class=\"pill frequency-").Length);
        Assert.Contains(
            "<div class=\"frequency\"><span class=\"pill frequency-everyday\" title=\"Subtlex: 12\">"
            + "<span class=\"tier\">Frequency.Everyday</span><span class=\"band\">"
            + "<span class=\"pip on\">✦</span><span class=\"pip on\">✦</span>"
            + "<span class=\"pip on\">✦</span><span class=\"pip\">✦</span></span></span></div>\n",
            sheet,
            StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_BreakRows_WritesPartNameTextGrid()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        LParadigmSlot slot = TInterface.TParadigmSlotCreate(
            TInterface.TSpeechValueCreate(1, "Verb"),
            TInterfaceInflection.TMorphologyCreate(1, 1, "past", 0),
            TInterfaceInflection.TInflectionCreate(entry.LEntryId, 0, "broke", null, 1, [1]),
            LState.LStateSpecified);
        IReadOnlyList<LParadigmRow> rows = TInterface.TParadigmRowScan([slot]);
        LLiveryPage page = TSheetEntryRead(engine, entry) with
        {
            LLiveryPageParadigm = rows,
            LLiveryPageShown = [TInterface.TParadigmShownCreate("broke", null)],
        };

        string sheet = TSheetTextFormat(engine, entry, page);

        Assert.Contains(
            "<table class=\"paradigm-rows\">\n<tr><td class=\"part\">"
            + TInterface.TSheetNormalize(rows[0].LParadigmRowPart)
            + "</td><td class=\"name\">" + TInterface.TSheetNormalize(rows[0].LParadigmRowName)
            + "</td><td class=\"text\">broke</td></tr>\n</table>\n",
            sheet,
            StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_HaberView_ChecksShortAndMarksIrregularRun()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        LParadigmTable collapsed = TInterfaceInflection.TParadigmTableCreate(
            [],
            [TInterfaceInflection.TParadigmLineCreate(
                string.Empty, "Inflection.Short", [TParadigmFormBuild("he", 0)])]);
        LParadigmTable expanded = TInterfaceInflection.TParadigmTableCreate(
            [],
            [TInterfaceInflection.TParadigmLineCreate(
                string.Empty, "Inflection.Present", [TParadigmFormBuild("hemos", 2)])]);
        LLiveryPage page = TSheetEntryRead(engine, entry) with
        {
            LLiveryPageParadigm = [],
            LLiveryPageShown = [],
            LLiveryPageInflection = TInterfaceInflection.TParadigmViewCreate(collapsed, expanded),
        };

        string sheet = TSheetTextFormat(engine, entry, page);

        Assert.Contains(
            "<input type=\"radio\" name=\"paradigm-fold\" id=\"paradigm-short\" checked>"
            + "<input type=\"radio\" name=\"paradigm-fold\" id=\"paradigm-full\">",
            sheet,
            StringComparison.Ordinal);
        Assert.Contains(
            "<table class=\"inflection inflection-short\">\n<tr><td class=\"llyn-group\"></td>"
            + "<td class=\"llyn-label\">Inflection.Short</td><td>he</td></tr>\n</table>\n",
            sheet,
            StringComparison.Ordinal);
        Assert.Contains("<td><span class=\"llyn-marked\">he</span>mos</td>", sheet, StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_FoldedCard_WritesClosedDetails()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("to come apart", 1), TInterface.TCardCreate("to stop", 2)],
            []));
        LEntryDraft held = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        engine.TEngineFoldSave(entry.LEntryId, LCardKind.LCardKindMeaning, held.LEntryDraftMeanings[0].LCardDraftId);

        string sheet = TSheetTextFormat(engine, entry);

        Assert.Contains(
            "<details class=\"card\">\n<summary><span class=\"rank\">1</span>", sheet, StringComparison.Ordinal);
        Assert.Contains(
            "<details class=\"card\" open>\n<summary><span class=\"rank\">2</span>", sheet, StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_UnlinkedCollocation_WritesOtherRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LCardDraft collocation = TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnspecified,
            TInterfaceState.TStateValueCreate("set off"),
            TInterfaceState.TStateValueCreate("to start a journey"),
            [], [], [], [], [], 1);
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "set", "English", string.Empty, string.Empty, [TInterface.TCardCreate("to place", 1)], [collocation]));

        string sheet = TSheetTextFormat(engine, entry);

        Assert.Contains("<section class=\"band\">\n<h2>Other collocations</h2>\n", sheet, StringComparison.Ordinal);
        Assert.Contains(
            "<div class=\"collocations\">\n<details class=\"collocation\" open>\n<summary><span class=\"phrase\">"
            + "set off</span><span class=\"sense\">to start a journey</span></summary>",
            sheet,
            StringComparison.Ordinal);
        Assert.DoesNotContain(
            "<section class=\"band deck\">\n<h2>Other collocations</h2>", sheet, StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_OpenedFanqie_WritesOpenDetails()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        engine.TEngineBoxSpread(entry.LEntryId, LFoldBox.LFoldBoxFanqie, true);
        LPortraitPage portrait = engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead());
        portrait = portrait with
        {
            LPortraitPageSection =
            [
                .. portrait.LPortraitPageSection,
                TInterface.TPortraitSectionCreate(
                    TInterface.TPortraitLabelRead().LPortraitLabelFanqie,
                    [TInterface.TPortraitLineCreate("Broad", "x")],
                    string.Empty,
                    [],
                    []),
            ],
        };

        string sheet = TInterface.TSheetEntryFormat(TSheetEntryRead(engine, entry), portrait, static key => key);

        Assert.Contains(
            "<details class=\"box fanqie\" open>\n<summary>"
            + TInterface.TSheetNormalize(TInterface.TPortraitLabelRead().LPortraitLabelFanqie)
            + "</summary>\n",
            sheet,
            StringComparison.Ordinal);
        Assert.DoesNotContain("<details class=\"box fanqie\">", sheet, StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_Stamps_WritesCreatedAndUpdatedFooter()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        LLiveryPage page = TSheetEntryRead(engine, entry);

        string sheet = TSheetTextFormat(engine, entry, page);

        Assert.Contains(
            "<footer class=\"stamp\">\n<hr>\n<div class=\"grid\">"
            + "<span class=\"label\">Display.Created</span><span class=\"time\">"
            + TInterface.TSheetNormalize(page.LLiveryPageCreated)
            + "</span><span class=\"label\">Display.Updated</span><span class=\"time\">"
            + TInterface.TSheetNormalize(page.LLiveryPageUpdated)
            + "</span></div>\n</footer>\n",
            sheet,
            StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_FlagRepeated_EmbedsItOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);
        var flag = new Dictionary<string, string> { ["Brazil"] = TSheetEntryFlag };
        LLiveryPage page = TLiveryVarietyCreate(engine, entry, flag);
        page = page with { LLiveryPageBanner = new Dictionary<string, string> { ["English"] = TSheetEntryFlag } };

        string sheet = TSheetTextFormat(engine, entry, page);

        Assert.Equal(2, sheet.Split(".ensign-0{").Length);
        Assert.DoesNotContain(".ensign-1{", sheet, StringComparison.Ordinal);
        Assert.Contains(
            "<span class=\"language\"><span class=\"flag ensign-0\"></span>", sheet, StringComparison.Ordinal);
        Assert.Contains("<span class=\"flag ensign-0\" title=\"Brazil\"></span>", sheet, StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_PrintStyle_KeepsTheScreenSurface()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TSheetEntrySave(engine);

        string sheet = TSheetTextFormat(engine, entry);

        int start = sheet.IndexOf("@media print", StringComparison.Ordinal);
        Assert.True(start >= 0);
        string print = sheet[start..(sheet.IndexOf("}}", start, StringComparison.Ordinal) + 2)];
        Assert.Equal("@media print{.entry{padding:0;}}", print);
        Assert.Contains("@page{margin:12mm;}", sheet, StringComparison.Ordinal);
        Assert.Contains("print-color-adjust:exact;", sheet, StringComparison.Ordinal);
        Assert.Contains("body{background:var(--surface);", sheet, StringComparison.Ordinal);
    }

    private static LEntry TSheetEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, "A plain note", [TInterface.TCardCreate("a liquid", 1)], []));
    }

    private static LLiveryPage TSheetEntryRead(LEngine engine, LEntry entry)
    {
        return Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
    }

    private static LLiveryPage TLiveryVarietyCreate(
        LEngine engine, LEntry entry, IReadOnlyDictionary<string, string> ensign)
    {
        LLiveryPage page = TSheetEntryRead(engine, entry);
        LAccentRow primary = page.LLiveryPageAccent.LAccentSheetPrimary with
        {
            LAccentRowVariety = "Brazil",
            LAccentRowText = "agwa",
        };
        return page with
        {
            LLiveryPageAccent = page.LLiveryPageAccent with { LAccentSheetPrimary = primary, LAccentSheetRows = [] },
            LLiveryPageBanner = new Dictionary<string, string>(),
            LLiveryPageEnsign = ensign,
        };
    }

    private static string TSheetTextFormat(LEngine engine, LEntry entry)
    {
        return TSheetTextFormat(engine, entry, TSheetEntryRead(engine, entry));
    }

    private static string TSheetTextFormat(LEngine engine, LEntry entry, LLiveryPage page)
    {
        return TInterface.TSheetEntryFormat(
            page, engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead()), static key => key);
    }

    private static LParadigmForm TParadigmFormBuild(string text, int marked)
    {
        return TInterfaceInflection.TParadigmFormCreate(
            text, marked > 0 ? [TInterfaceInflection.TInflectionMarkCreate(0, marked)] : [], null);
    }
}
