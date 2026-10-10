using System.Globalization;
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
    public void LiveryFormat_StoredEtymon_RendersNoteLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry dog = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "dog", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a hound", 1)], []));
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "doggo", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a dog", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        page = page with { LLiveryPageEtymon = [TInterface.TTranslationTargetCreate(dog.LEntryId, "dog", "English")] };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Contains("## Display.Etymology", body, StringComparison.Ordinal);
        Assert.Contains(
            "- [dog](:/" + dog.LEntryId.ToString(CultureInfo.InvariantCulture) + ")",
            body,
            StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_OutsideEtymon_KeepsPlainText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "doggo", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a dog", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        page = page with { LLiveryPageEtymon = [TInterface.TTranslationTargetCreate(987654, "cat", "English")] };

        string body = TInterface.TLiveryFormat(page, static key => key, static _ => string.Empty).LLiveryNoteBody;

        Assert.Contains("- cat <span class=\"llyn-language\">English</span>", body, StringComparison.Ordinal);
        Assert.DoesNotContain("[cat]", body, StringComparison.Ordinal);
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
        LEntry dog = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "dog", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a hound", 1)], []));
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
            LLiveryPageDraft = page.LLiveryPageDraft with
            {
                LEntryDraftEtymology = TInterface.TEtymologyDraftCreate(
                    "from dog", [TInterfaceMentionSpan.TMentionDraftCreate(0, 5, 3, dog.LEntryId)]),
            },
            LLiveryPageParadigm = TInterface.TParadigmRowScan([slot]),
            LLiveryPageFanqie = [TInterface.TFanqieGroupCreate(string.Empty, "Broad", "Kaom", [row], ["朿"])],
        };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        string link = "from [dog](:/" + dog.LEntryId.ToString(CultureInfo.InvariantCulture) + ")";
        int[] places =
        [
            body.IndexOf("<div class=\"llyn\">", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-label\">past</span> | broke |", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-series\">Display.FanqieShengfu</span>", StringComparison.Ordinal),
            body.IndexOf("<span class=\"llyn-book\">Broad</span>", StringComparison.Ordinal),
            body.IndexOf("## Display.Etymology", StringComparison.Ordinal),
            body.IndexOf(link, StringComparison.Ordinal),
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
        string[] roles =
        [
            "ink", "muted", "line", "canvas", "surface", "accent", "accentSoft",
            "situation", "situationSoft", "situationEdge", "surfaceRaised", "favorite",
            "helper", "helperSoft", "helperEdge", "frequencyCore", "frequencyEveryday", "frequencyAdvanced",
            "frequencyRare", "warning",
        ];
        LTheme theme = TInterface.TThemeCreate(
            roles.ToDictionary(static role => role, static role => "var(--" + role + ")"));

        string css = TInterface.TLiveryStyleFormat(theme);

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
        string[] roles =
        [
            "ink", "muted", "line", "canvas", "surface", "accent", "accentSoft",
            "situation", "situationSoft", "situationEdge", "surfaceRaised", "favorite",
            "helper", "helperSoft", "helperEdge", "frequencyCore", "frequencyEveryday", "frequencyAdvanced",
            "frequencyRare", "warning",
        ];
        LTheme theme = TInterface.TThemeCreate(
            roles.ToDictionary(static role => role, static role => "var(--" + role + ")"));

        string css = TInterface.TLiveryStyleFormat(theme);

        Assert.Contains(".llyn-card > summary::after", css, StringComparison.Ordinal);
        Assert.Contains(".llyn-card[open] > summary::after", css, StringComparison.Ordinal);
        Assert.Contains(".llyn-card:not([open]) > summary::after", css, StringComparison.Ordinal);
        Assert.Contains(".llyn-card > summary::-webkit-details-marker", css, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryStyleFormat_OpenFullDetails_HidesShortTable()
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

        string css = TInterface.TLiveryStyleFormat(theme);

        Assert.Contains(
            ".llyn .llyn-inflection-full[open] + .llyn-inflection-short { display: none; }",
            css,
            StringComparison.Ordinal);
        Assert.Matches(
            new Regex(@"\.llyn \.llyn-inflection-full > summary \{[^}]*margin: 0 0 10px auto;[^}]*text-align: right;"),
            css);
    }
}
