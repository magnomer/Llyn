using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineReflex
{
    private const string TEngineReflexPack =
        """
        { "reflex": [
            { "language": "Korean", "url": "https://example.test/wiki/{word}",
              "match": "eumhun: (?<sense>[^ ]+) (?<sound>[^ <]+)(?: initial (?<initial>[^ <]+))?",
              "format": "{sound}[[({initial})]] | {sense}", "epithet": "{text}", "clip": "\\(.*\\)",
              "first": true },
            { "language": "Japanese", "url": "https://example.test/wiki/{word}",
              "match": "<b>(?<kind>Go-on|Kan-on)</b>: (?<main><i class=\"jouyou\">)?<span>(?<text>[^<]+)</span>",
              "every": true, "first": true },
            { "language": "Mandarin", "url": "https://example.test/ipa/{word}",
              "match": "IPA: (?<text>/[^ <]+/(?:, /[^ <]+/)*)(?: (?<note>[^<]+))?", "busy": "Too fast",
              "rewrite": [["/([^/]+)/", "[$1]"]] } ] }
        """;

    private const string TEngineReflexNong =
        """
        <p>eumhun: 희롱할 롱 initial 농</p>
        <ul><li><b>Go-on</b>: <span>る</span></li>
        <li><b>Kan-on</b>: <i class="jouyou"><span>ろう</span></i></li></ul>
        """;

    private const string TEngineReflexZhang =
        """
        <p>eumhun: 홀 장</p>
        <ul><li><b>Kan-on</b>: <span>しょう</span></li></ul>
        """;

    private const string TEngineReflexAn =
        """
        <p>eumhun: 편안 안</p>
        <ul><li><b>Go-on</b>: <span>あん</span></li>
        <li><b>Kan-on</b>: <i class="jouyou"><span>あん</span></i></li></ul>
        """;

    private static readonly TimeSpan TEngineReflexPatience = TimeSpan.FromSeconds(5);

    private static readonly Dictionary<string, string> TEngineReflexPages = new()
    {
        ["https://example.test/wiki/%E5%BC%84"] = TEngineReflexNong,
        ["https://example.test/wiki/%E7%92%8B"] = TEngineReflexZhang,
        ["https://example.test/wiki/%E5%AE%89"] = TEngineReflexAn,
        ["https://example.test/ipa/%E5%AE%89"] = "<p>IPA: /än⁵⁵/ ān</p>",
        ["https://example.test/ipa/%E5%BC%84"] = "<p>IPA: /nʊŋ⁵¹/, /lʊŋ⁵¹/ nòng</p>",
        ["https://example.test/ipa/%E7%92%8B"] = "<p>IPA: /ʈ͡ʂɑŋ⁵⁵/ zhāng</p>",
    };

    [Fact]
    public void ReflexRuleRead_FixturePack_ReadsRulesInPackOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LReflexRule> rules = engine.TEngineReflexRead(pack.TLanguageFixtureName);

        Assert.Equal(["Korean", "Japanese", "Mandarin"], rules.Select(rule => rule.LReflexRuleLanguage));
        Assert.Equal("{sound}[[({initial})]] | {sense}", rules[0].LReflexRuleTemplate);
        Assert.Equal("{text}", rules[1].LReflexRuleTemplate);
        Assert.True(rules[1].LReflexRuleEvery);
        Assert.Equal("Too fast", rules[2].LReflexRuleBusy);
        Assert.Empty(engine.TEngineReflexRead("English"));
    }

    [Fact]
    public async Task ReflexFind_ThreeRules_ReadsRowsKindsNotesAndMark()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "弄", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Korean", "", "롱(농) | 희롱할", "", true),
                ("Japanese", "Go-on", "る", "", false),
                ("Japanese", "Kan-on", "ろう", "", true),
                ("Mandarin", "", "[nʊŋ⁵¹], [lʊŋ⁵¹]", "nòng", false),
            ],
            found.Select(TReflexRowRead));
    }

    [Fact]
    public async Task ReflexFind_OptionalSegmentUncaptured_DropsTheSegment()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "璋", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal("장 | 홀", found[0].LReflexDraftText);
    }

    [Fact]
    public async Task ReflexFind_NoMainCaptured_MarksFirstRow()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "璋", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Korean", "", "장 | 홀", "", true),
                ("Japanese", "Kan-on", "しょう", "", true),
                ("Mandarin", "", "[ʈ͡ʂɑŋ⁵⁵]", "zhāng", false),
            ],
            found.Select(TReflexRowRead));
    }

    [Fact]
    public async Task ReflexFind_KoreanPairOfSeveralWords_ReadsLastWordAsReading()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "reflex": [ { "language": "Korean", "url": "https://example.test/api?query={word}",
              "match": "(?<=\"items\":\\[\\{[^\\[\\]]*\"expKoreanPron\":\"[^\"]*)
            """.TrimEnd()
            + """(?<note>[^\" ,/][^\",/]*?) (?<text>[^\" ,/]+)(?=[\",/])", "every": true, "first": true } ] }""");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/api?query=%E6%83%A1"] =
                    """{"items":[{"expEntry":"惡","expKoreanPron":"악할 악, 미워할 오, 강 이름 호","x":1}]}""",
            }));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "惡", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Korean", "", "악", "악할", true),
                ("Korean", "", "오", "미워할", false),
                ("Korean", "", "호", "강 이름", false),
            ],
            found.Select(TReflexRowRead));
    }

    [Fact]
    public async Task ReflexFind_OneTextAcrossKinds_MarksEveryRow()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "安", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Korean", "", "안 | 편안", "", true),
                ("Japanese", "Go-on", "あん", "", true),
                ("Japanese", "Kan-on", "あん", "", true),
                ("Mandarin", "", "[än⁵⁵]", "ān", false),
            ],
            found.Select(TReflexRowRead));
    }

    [Fact]
    public async Task ReflexFind_BorrowerPack_FillsRespellingBesideText()
    {
        using TLanguageFixture borrower = TLanguageFixture.TLanguageFixtureCreate(
            """{ "phonemic": true, "respelling": [ { "name": "Plain", "rules": [["ʊ", "u"], ["⁵¹", "4"]] } ] }""");
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            TEngineReflexPack.Replace("\"Mandarin\"", "\"" + borrower.TLanguageFixtureName + "\""));
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "弄", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal("[nʊŋ⁵¹], [lʊŋ⁵¹]", found[3].LReflexDraftText);
        Assert.Equal("/nuŋ4/, /luŋ4/", found[3].LReflexDraftRespelling);
        Assert.Equal("nòng", found[3].LReflexDraftNote);
        Assert.Equal("ろう", found[2].LReflexDraftText);
        Assert.Equal(string.Empty, found[2].LReflexDraftRespelling);
    }

    [Fact]
    public async Task ReflexFind_TwoCharacters_MergesRowsOfOneKindAndNote()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineReflexPages));

        IReadOnlyList<LReflexDraft> found = await engine.TEngineReflexFind(
            "弄璋", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [
                ("Korean", "", "롱(농) | 희롱할 장 | 홀", "", true),
                ("Japanese", "Go-on", "る", "", false),
                ("Japanese", "Kan-on", "ろう しょう", "", true),
                ("Mandarin", "", "[nʊŋ⁵¹], [lʊŋ⁵¹]", "nòng", false),
                ("Mandarin", "", "[ʈ͡ʂɑŋ⁵⁵]", "zhāng", false),
            ],
            found.Select(TReflexRowRead));
    }

    [Fact]
    public async Task ReflexStart_NothingStored_FetchesStoresAndRaises()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineReflexPages));
        TReflexObserver observer = new();
        engine.TEngineObserverAttach(observer);
        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate("弄", pack.TLanguageFixtureName));

        Assert.Empty(engine.TEngineReflexRead(entry.LEntryId));
        engine.TEngineReflexStart(entry.LEntryId);

        LBulletin raised = await observer.TReflexObserverRaised.WaitAsync(TEngineReflexPatience);
        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        await TReflexSettle(engine, entry.LEntryId);

        IReadOnlyList<LReflex> read = engine.TEngineReflexRead(entry.LEntryId);
        Assert.Equal(4, read.Count);
        Assert.Equal(("Japanese", "Kan-on", "ろう", "", true), TReflexRowRead(read[2]));
        Assert.Equal(("Mandarin", "", "[nʊŋ⁵¹], [lʊŋ⁵¹]", "nòng", false), TReflexRowRead(read[3]));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reflex;"));
    }

    [Fact]
    public async Task ReflexStart_RowsStored_FetchesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(TEngineReflexNong, HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate("弄", pack.TLanguageFixtureName) with
        {
            LEntryDraftReflexes = [TInterface.TReflexDraftCreate("Korean", "", "롱")],
        });

        engine.TEngineReflexStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(0, handler.TSourceHandlerCount);
        Assert.Equal("롱", Assert.Single(engine.TEngineReflexRead(entry.LEntryId)).LReflexText);
    }

    [Fact]
    public async Task ReflexRebuild_RowsStored_DropsThemAndFetchesAgain()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineReflexPages));
        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate("弄", pack.TLanguageFixtureName) with
        {
            LEntryDraftReflexes = [TInterface.TReflexDraftCreate("Korean", "", "롱")],
        });
        TReflexObserver observer = new();
        engine.TEngineObserverAttach(observer);

        engine.TEngineReflexRebuild(entry.LEntryId);

        LBulletin raised = await observer.TReflexObserverRaised.WaitAsync(TEngineReflexPatience);
        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        await TReflexSettle(engine, entry.LEntryId);

        IReadOnlyList<LReflex> read = engine.TEngineReflexRead(entry.LEntryId);
        Assert.Equal(4, read.Count);
        Assert.Equal(("Korean", "", "롱(농) | 희롱할", "", true), TReflexRowRead(read[0]));
    }

    [Fact]
    public void EpithetRead_KoreanRowsStored_JoinsClippedPieces()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate("惡", pack.TLanguageFixtureName) with
        {
            LEntryDraftReflexes =
            [
                TInterface.TReflexDraftCreate("Korean", "", "악할 악(악)"),
                TInterface.TReflexDraftCreate("Korean", "", "미워할 오"),
                TInterface.TReflexDraftCreate("Japanese", "Go-on", "あく"),
            ],
        });

        Assert.Equal("악할 악, 미워할 오", engine.TEngineEpithetRead(entry.LEntryId));

        engine.TEngineEpithetSave(false);

        Assert.Equal(string.Empty, engine.TEngineEpithetRead(entry.LEntryId));
    }

    [Fact]
    public async Task ReflexStart_PagesNotFound_AsksOncePerSession()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("gone", HttpStatusCode.NotFound);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate("弄", pack.TLanguageFixtureName));

        engine.TEngineReflexStart(entry.LEntryId);
        await TReflexSettle(engine, entry.LEntryId);
        engine.TEngineReflexStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(3, handler.TSourceHandlerCount);
        Assert.Empty(engine.TEngineReflexRead(entry.LEntryId));
    }

    [Fact]
    public async Task ReflexStart_HeldDraftWithoutRows_FillsTheDraft()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineReflexPages));
        LEntry entry = engine.TEngineEntrySave(TReflexDraftCreate("弄", pack.TLanguageFixtureName));
        LDraft started = engine.TEngineDraftStart("Input", entry.LEntryId);
        TReflexObserver observer = new(started.LDraftId);
        engine.TEngineObserverAttach(observer);

        engine.TEngineReflexStart(entry.LEntryId);
        await observer.TReflexObserverRaised.WaitAsync(TEngineReflexPatience);
        await TReflexSettle(engine, entry.LEntryId);

        LDraft? held = engine.TEngineDraftRead(started.LDraftId);
        Assert.NotNull(held);
        Assert.Equal(4, held.LDraftContent.LEntryDraftReflexes.Count);
        Assert.All(held.LDraftContent.LEntryDraftReflexes, row => Assert.True(row.LReflexDraftId > 0));
        Assert.False(engine.TEngineDraftCheck(started.LDraftId));
    }

    private static async Task TReflexSettle(LEngine engine, long entryId)
    {
        DateTime deadline = DateTime.UtcNow + TEngineReflexPatience;
        while (engine.TEngineReflexCheck(entryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the reflex fetch to settle.");
            await Task.Delay(20);
        }
    }

    private static (string, string, string, string, bool) TReflexRowRead(LReflexDraft row) =>
        (row.LReflexDraftLanguage,
         row.LReflexDraftKind,
         row.LReflexDraftText,
         row.LReflexDraftNote,
         row.LReflexDraftMain);

    private static (string, string, string, string, bool) TReflexRowRead(LReflex row) =>
        (row.LReflexLanguage, row.LReflexKind, row.LReflexText, row.LReflexNote, row.LReflexMain);

    private static LEntryDraft TReflexDraftCreate(string headword, string language)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            []);
    }

    private sealed class TReflexObserver : LObserver
    {
        private readonly long _tReflexObserverDraft;

        private readonly TaskCompletionSource<LBulletin> _tReflexObserverRaised =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal TReflexObserver(long draft = 0)
        {
            _tReflexObserverDraft = draft;
        }

        internal Task<LBulletin> TReflexObserverRaised => _tReflexObserverRaised.Task;

        public void LObserverBulletinHandle(LBulletin bulletin)
        {
            bool wanted = _tReflexObserverDraft == 0
                ? bulletin.LBulletinSubject == LSubject.LSubjectReflex
                : bulletin.LBulletinSubject == LSubject.LSubjectDraft && bulletin.LBulletinId == _tReflexObserverDraft;
            if (wanted)
            {
                _tReflexObserverRaised.TrySetResult(bulletin);
            }
        }
    }
}
