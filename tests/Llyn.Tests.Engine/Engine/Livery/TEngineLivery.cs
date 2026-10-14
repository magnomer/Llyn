using System.Text.RegularExpressions;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineLivery
{
    [Fact]
    public void LiveryFormat_StoredEntry_OmitsExportMarkup()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", "A plain note", [TInterface.TCardCreate("a liquid", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(water.LEntryId));

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Contains("<div class=\"llyn\">\n\n", body, StringComparison.Ordinal);
        Assert.DoesNotContain("class=\"portrait\"", body, StringComparison.Ordinal);
        Assert.DoesNotContain("class=\"crest\"", body, StringComparison.Ordinal);
        Assert.DoesNotContain("class=\"band\"", body, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_FullHeader_RendersEveryPart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break",
            "English",
            "brejk",
            string.Empty,
            [TInterface.TCardCreate("to come apart", 1)],
            [],
            speeches: ["Verb"]));
        engine.TEngineFavoriteSave(entry.LEntryId);
        engine.TEngineGraspSave(entry.LEntryId, 3);
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Contains("# break <span class=\"llyn-language\">English</span>", body, StringComparison.Ordinal);
        Assert.Contains("<span class=\"llyn-heart llyn-on\">", body, StringComparison.Ordinal);
        Assert.Contains("<span class=\"llyn-star llyn-half\">", body, StringComparison.Ordinal);
        Assert.Contains("<span class=\"llyn-rating\">Grasp.Level3</span>", body, StringComparison.Ordinal);
        Assert.Contains("<span class=\"llyn-speech\">Verb</span>", body, StringComparison.Ordinal);
        Assert.Contains("brejk", body, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_MarkedWithoutChip_WritesNoChipLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break", "English", "brejk", string.Empty, [TInterface.TCardCreate("to come apart", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LLiveryPage marked = page with
        {
            LLiveryPageDraft = page.LLiveryPageDraft with
            {
                LEntryDraftSpeeches = [TInterface.TSpeechDraftCreate(1, string.Empty)],
            },
        };

        string body = TInterface.TLiveryFormat(marked, static key => key).LLiveryNoteBody;

        Assert.True(marked.LLiveryPageDraft.LEntryDraftMarked);
        Assert.Equal(TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody, body);
    }

    [Fact]
    public void LiveryFormat_TonedContour_DrawsChartPerSyllable()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "弄坏", "Mandarin", "nuŋ51-53 huai51", string.Empty, [TInterface.TCardCreate("to break", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        page = page with
        {
            LLiveryPageAccent = page.LLiveryPageAccent with
            {
                LAccentSheetContour =
                [TInterface.TContourCreate("nuŋ51-53", [5, 3]), TInterface.TContourCreate("huai51", [5, 1])],
            },
        };

        LLiveryNote note = TInterface.TLiveryFormat(page, static key => key);
        string body = note.LLiveryNoteBody;
        LTheme theme = TInterface.TThemeLoad();

        int contour = body.IndexOf("<span class=\"llyn-contour\">", StringComparison.Ordinal);
        Assert.True(contour > body.IndexOf("<span class=\"llyn-accent\">", StringComparison.Ordinal));
        Assert.DoesNotContain("<svg", body, StringComparison.Ordinal);
        Assert.Equal(2, body.Split("\" class=\"llyn-contour-chart\">").Length - 1);
        string[] charts = [.. note.LLiveryNoteParcel.Where(static parcel => parcel.LParcelMime == "image/svg+xml")
            .Select(static parcel => System.Text.Encoding.UTF8.GetString(parcel.LParcelBytes))];
        Assert.Equal(2, charts.Length);
        string last = Assert.Single(
            charts, static chart => chart.Contains(">huai51</text></svg>", StringComparison.Ordinal));
        string stop = "<stop stop-color=\"" + theme.TThemeRead("contourBottom") + "\" offset=\"1\"/>";
        Assert.Contains(stop, last, StringComparison.Ordinal);
        Assert.Contains(
            "<circle fill=\"" + theme.TThemeRead("contourMid") + "\"",
            Assert.Single(charts, chart => chart != last),
            StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_TonedContour_CentresPlotUnderReading()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "好", "Mandarin", "xau214", string.Empty, [TInterface.TCardCreate("good", 1)], []));
        string path = Path.Combine(workspace.TWorkspaceFolder, "hao.mp3");
        File.WriteAllBytes(path, [1, 2, 3, 4]);
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LEntryDraft draft = page.LLiveryPageDraft;
        page = page with
        {
            LLiveryPageDraft = draft with
            {
                LEntryDraftPronunciations =
                    [draft.LEntryDraftPronunciations[0] with { LPronunciationDraftAudio = path }],
            },
            LLiveryPageAccent = page.LLiveryPageAccent with
            {
                LAccentSheetContour = [TInterface.TContourCreate("xau214", [2, 1, 4])],
            },
        };

        LLiveryNote note = TInterface.TLiveryFormat(page, static key => key);
        string body = note.LLiveryNoteBody;

        LParcel audio = Assert.Single(note.LLiveryNoteParcel, static parcel => parcel.LParcelMime == "audio/mpeg");
        string head = "<span class=\"llyn-axis\"><span><span class=\"llyn-accent\">";
        int axis = body.IndexOf(head, StringComparison.Ordinal);
        int player = body.IndexOf("[" + audio.LParcelTitle + "](:/" + audio.LParcelId + ")", StringComparison.Ordinal);
        Assert.True(axis >= 0 && player > axis);
        string block = body[axis..player];
        Assert.Contains("</span><span class=\"llyn-contour\">", block, StringComparison.Ordinal);
        Assert.EndsWith("\" class=\"llyn-contour-chart\"></span></span> ", block, StringComparison.Ordinal);
        Assert.Contains(
            ".llyn .llyn-axis { display: inline-flex; flex-direction: column; align-items: center; }",
            TInterface.TLiveryStyleFormat(TInterface.TThemeLoad()),
            StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_StoredRecording_AddsAudioParcel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break", "English", "brejk", string.Empty, [TInterface.TCardCreate("to come apart", 1)], []));
        string path = Path.Combine(workspace.TWorkspaceFolder, "break.mp3");
        File.WriteAllBytes(path, [1, 2, 3, 4]);
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LEntryDraft draft = page.LLiveryPageDraft;
        page = page with
        {
            LLiveryPageDraft = draft with
            {
                LEntryDraftPronunciations =
                    [draft.LEntryDraftPronunciations[0] with { LPronunciationDraftAudio = path }],
            },
        };

        LLiveryNote note = TInterface.TLiveryFormat(page, static key => key);

        LParcel parcel = Assert.Single(note.LLiveryNoteParcel);
        Assert.Equal(("audio/mpeg", "audio.mp3"), (parcel.LParcelMime, parcel.LParcelTitle));
        Assert.Contains(
            "[" + parcel.LParcelTitle + "](:/" + parcel.LParcelId + ")",
            note.LLiveryNoteBody,
            StringComparison.Ordinal);
        Assert.DoesNotContain("<audio", note.LLiveryNoteBody, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_MissingRecording_OmitsPlayer()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break", "English", "brejk", string.Empty, [TInterface.TCardCreate("to come apart", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LEntryDraft draft = page.LLiveryPageDraft;
        string path = Path.Combine(workspace.TWorkspaceFolder, "absent.mp3");
        page = page with
        {
            LLiveryPageDraft = draft with
            {
                LEntryDraftPronunciations =
                    [draft.LEntryDraftPronunciations[0] with { LPronunciationDraftAudio = path }],
            },
        };

        LLiveryNote note = TInterface.TLiveryFormat(page, static key => key);

        Assert.Empty(note.LLiveryNoteParcel);
        Assert.DoesNotContain("<audio", note.LLiveryNoteBody, StringComparison.Ordinal);
        Assert.Contains("brejk", note.LLiveryNoteBody, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_RepeatedRimeBook_OmitsBookChip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "爛", "Chinese", string.Empty, string.Empty, [TInterface.TCardCreate("rotten", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LFanqieRow rime = TInterface.TFanqieRowCreate("爛", "Broad", "郎旰", "來", "寒");
        IReadOnlyList<LFanqieGroup> groups = TInterface.TFanqieGroupScan(
            [rime with { LFanqieRowSource = "Kaom" }, rime with { LFanqieRowSource = "Wiktionary" }],
            [TInterface.TFanqieBookCreate("Broad", "Kaom"), TInterface.TFanqieBookCreate("Broad", "Wiktionary")]);
        page = page with { LLiveryPageFanqie = groups };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Single(body.Split("<span class=\"llyn-book\">Broad</span>").Skip(1));
        Assert.Contains("<span class=\"llyn-source\">Kaom</span>", body, StringComparison.Ordinal);
        Assert.Contains("<span class=\"llyn-source\">Wiktionary</span>", body, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_FullPage_KeepsViewOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break", "English", string.Empty, "A **plain** note", [TInterface.TCardCreate("to come apart", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LParadigmSlot slot = TInterface.TParadigmSlotCreate(
            TInterface.TSpeechValueCreate(1, "Verb"),
            TInterfaceInflection.TMorphologyCreate(1, 1, "past", 0),
            TInterfaceInflection.TInflectionCreate(entry.LEntryId, 0, "broke", null, 1, [1]),
            LState.LStateSpecified);
        LFanqieRow row = TInterface.TFanqieRowCreate("字", "Broad", "郎旰", "來", "寒", "翰", "一", "去");
        page = page with
        {
            LLiveryPageParadigm = TInterface.TParadigmRowScan([slot]),
            LLiveryPageFanqie = [TInterface.TFanqieGroupCreate(string.Empty, "Broad", "Kaom", [row], ["朿"])],
        };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        int[] places =
        [
            body.IndexOf("<div class=\"llyn\">", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-label\">past</span> | broke |", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-series\">Display.FanqieShengfu</span>", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-book\">Broad</span>", StringComparison.Ordinal),
            body.IndexOf("## Display.Note\n\nA **plain** note", StringComparison.Ordinal),
            body.IndexOf("---\n\n<span class=\"llyn-stamp\">Display.Created</span>", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-stamp\">Display.Updated</span>", StringComparison.Ordinal),
        ];
        Assert.All(places, static place => Assert.True(place >= 0));
        Assert.Equal(places.Order(), places);
    }

    [Fact]
    public void CourierNoteBuild_OutsideEntry_ReturnsEmpty()
    {
        LEntry dog = TInterface.TEntryCreate(7, "dog", "English", 0, null, null);

        Func<long, string> note = TInterface.TCourierNoteBuild("realm", [dog]);

        Assert.Equal(32, note(7).Length);
        Assert.Equal(string.Empty, note(8));
    }

    [Fact]
    public void LiveryStyleFormat_TokenTheme_ScopesRulesWithoutColour()
    {
        string css = TThemeStyleFormat();

        string[] selectors =
        [
            .. css.Split('}').Where(static rule => rule.Contains('{', StringComparison.Ordinal))
                .SelectMany(static rule => rule[..rule.IndexOf('{', StringComparison.Ordinal)].Split(','))
                .Select(static selector => selector.Trim()),
        ];
        Assert.NotEmpty(selectors);
        Assert.All(selectors, static selector => Assert.True(
            selector == ".llyn" || selector.StartsWith(".llyn ", StringComparison.Ordinal), selector));
        Assert.DoesNotContain("#", css, StringComparison.Ordinal);
        Assert.DoesNotMatch(new Regex(@"(?<![\w-])(rgba?|hsla?|white|black|transparent)(?![\w-])"), css);
    }

    [Fact]
    public void LiveryStyleFormat_CardSummary_DrawsFoldChevronAtRightEnd()
    {
        string css = TThemeStyleFormat();

        Assert.Contains(".llyn-card > summary::after", css, StringComparison.Ordinal);
        Assert.Contains(".llyn-card[open] > summary::after", css, StringComparison.Ordinal);
        Assert.Contains(".llyn-card:not([open]) > summary::after", css, StringComparison.Ordinal);
        Assert.Contains(".llyn-card > summary::-webkit-details-marker", css, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryStyleFormat_OpenFullDetails_HidesShortTable()
    {
        string css = TThemeStyleFormat();

        Assert.Contains(
            ".llyn .llyn-inflection-full[open] + .llyn-inflection-short { display: none; }",
            css,
            StringComparison.Ordinal);
        Assert.Matches(
            new Regex(@"\.llyn \.llyn-inflection-full > summary \{[^}]*margin: 0 0 10px auto;[^}]*text-align: right;"),
            css);
    }

    [Fact]
    public void LiveryStyleFormat_ReflexTableInCard_KeepsReadingSizes()
    {
        const string reflex = ":has(> thead > tr > th:nth-child(7):last-child:empty)";

        string css = TThemeStyleFormat();

        Assert.Contains(".llyn table" + reflex + " td:nth-child(-n+2) {", css, StringComparison.Ordinal);
        Assert.Contains(".llyn table" + reflex + " td:nth-child(3) { font-size: 20px;", css, StringComparison.Ordinal);
        Assert.Contains(".llyn table" + reflex + " td:nth-child(n+4) {", css, StringComparison.Ordinal);
        Assert.DoesNotMatch(new Regex(@"\.llyn \.llyn-card (td|\.llyn-tone)"), css);
        string card = Regex.Escape(".llyn .llyn-card table:where(:not(" + reflex + "))");
        Assert.Matches(new Regex(card + @" td \{[^}]*font-size: 14px;"), css);
    }

    [Fact]
    public void LiveryStyleFormat_CollocationHeading_LeavesHeadingPillAlone()
    {
        string css = TThemeStyleFormat();

        string[] pills =
        [
            .. css.Split('}').Where(static rule => rule.Contains('{', StringComparison.Ordinal))
                .Where(static rule =>
                    rule[..rule.IndexOf('{', StringComparison.Ordinal)].Trim() == ".llyn .llyn-heading"),
        ];
        string pill = Assert.Single(pills);
        Assert.Contains("background: var(--accentSoft);", pill, StringComparison.Ordinal);
        Assert.Contains("color: var(--accent);", pill, StringComparison.Ordinal);
        Assert.Matches(
            new Regex(@"\.llyn \.llyn-collocations \.llyn-heading \{[^}]*background: none;[^}]*color: var\(--muted\);"),
            css);
    }

    private static string TThemeStyleFormat()
    {
        string[] roles =
        [
            "ink", "muted", "line", "canvas", "surface", "accent", "accentSoft",
            "situation", "situationSoft", "situationEdge", "surfaceRaised", "favorite",
            "helper", "helperSoft", "helperEdge", "frequencyCore", "frequencyEveryday", "frequencyAdvanced",
            "frequencyRare", "warning",
        ];
        LTheme theme = TInterface.TThemeCreate(
            roles.ToDictionary(static role => role, static role => "var(--" + role + ")"));
        return TInterface.TLiveryStyleFormat(theme);
    }
}
