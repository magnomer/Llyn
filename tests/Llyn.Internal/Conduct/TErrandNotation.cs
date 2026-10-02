using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TErrandNotation
{
    private const string TErrandNotationPack =
        """
        { "varieties": { "list": [ { "name": "British" } ] },
          "pronunciation": [
            { "name": "Stub", "attempts": [ { "urls": ["https://example.test/{word}"],
                "readings": [ { "variety": "British", "strategy": "regex",
                    "match": "uk=(\\S+)", "group": 1 } ] } ] } ] }
        """;

    [Fact]
    public async Task TranscriptionStart_PaddedHeadword_StreamsEveryStepThroughTheDeskMarshal()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, " hill ");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        editor.CEditorDesk.TDeskRead();
        Channel<Action> marshalled = Channel.CreateUnbounded<Action>();
        editor.CEditorDesk.CDeskObserverAttach(run => marshalled.Writer.TryWrite(run));
        List<string> notices = [];
        CNotationRoll? ended = null;
        errand.CErrandNotationChanged += roll =>
        {
            notices.Add(TErrandNotationRead(roll));
            ended = roll;
        };

        errand.CErrandTranscriptionStart(0, string.Empty);
        (await TErrandStepRead(marshalled))();
        Assert.Equal(["Stub:"], notices);
        gate.SetResult();
        (await TErrandStepRead(marshalled))();
        (await TErrandStepRead(marshalled))();

        Assert.Equal(["Stub:", "Stub:hɪl", "Stub:hɪl"], notices);
        Assert.False(marshalled.Reader.TryRead(out _));
        Assert.NotNull(ended);
        Assert.False(ended.CNotationRollSearching);
        CNotationReading reading = Assert.Single(Assert.Single(ended.CNotationRollRows).CNotationItemReading);
        Assert.Equal("hɪl", reading.CNotationReadingPhonetic);
        Assert.Equal(
            CSounding.CSoundingVarietyRead(pack.TLanguageFixtureName, "British"), reading.CNotationReadingVariety);
        Assert.False(reading.CNotationReadingFlagged);
        Assert.Equal(new CRespellingMark(false, "[", "]"), reading.CNotationReadingMark);
        Assert.Equal("Phonetician.Empty", ended.CNotationRollNotice);
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task TranscriptionStart_EarlierSearchStep_ListsNothingOfIt()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        editor.CEditorTimbre.CTimbrePronunciationAdd(0);
        long accent = editor.CEditorDesk.TDeskRead()!.LDraftContent.LEntryDraftPronunciations[1].LPronunciationDraftId;
        Channel<Action> marshalled = Channel.CreateUnbounded<Action>();
        editor.CEditorDesk.CDeskObserverAttach(run => marshalled.Writer.TryWrite(run));
        List<string> notices = [];
        errand.CErrandNotationChanged += roll => notices.Add(TErrandNotationRead(roll));

        errand.CErrandTranscriptionStart(0, string.Empty);
        Action earlier = await TErrandStepRead(marshalled);
        CNotationRoll started = errand.CErrandTranscriptionStart(accent, string.Empty);
        Action later = await TErrandStepRead(marshalled);
        earlier();

        Assert.Empty(started.CNotationRollRows);
        Assert.Empty(notices);
        later();
        Assert.Equal(["Stub:"], notices);
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task TranscriptionStart_BlankHeadword_StartsNothingAndStopsTheClip()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        errand.CErrandRecordingStart(0);
        editor.CEditorHeadwordSet("   ");

        CNotationRoll started = errand.CErrandTranscriptionStart(0, string.Empty);

        Assert.False(started.CNotationRollSearching);
        Assert.True(started.CNotationRollEmpty);
        Assert.Equal("Phonetician.Empty", started.CNotationRollNotice);
        Assert.Null(await errand.CErrandPreviewStart(
            new CRecording("Tagged", "https://example.test/gb.mp3", 0, true, "British")));
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task FlagLoad_FlaggedPack_StoresTheVarietyFlagsOnlyForAnUnschemedSearch()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        string flag = Path.Combine(AppContext.BaseDirectory, "languages", pack.TLanguageFixtureName, "hill.svg");
        pack.TLanguageFixtureSave("hill.svg", "<svg xmlns=\"http://www.w3.org/2000/svg\"/>");
        pack.TLanguageFixtureSave(
            "source.json",
            "{ \"varieties\": { \"shown\": \"flag\", \"list\": [ { \"name\": \"British\", \"flag\": \""
            + flag.Replace('\\', '/')
            + "\" } ] } }");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        List<string> stored = [];
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store = (rows, _) =>
        {
            stored.AddRange(rows.Select(static row => row.CEnsignRowKey));
            return static () => { };
        };

        CNotationRoll idle = await errand.CErrandFlagLoad(store);
        errand.CErrandTranscriptionStart(0, "Yale");
        CNotationRoll schemed = await errand.CErrandFlagLoad(store);
        Assert.Empty(stored);
        errand.CErrandTranscriptionStart(0, string.Empty);
        CNotationRoll plain = await errand.CErrandFlagLoad(store);

        Assert.False(idle.CNotationRollSearching);
        Assert.Empty(schemed.CNotationRollRows);
        Assert.Empty(plain.CNotationRollRows);
        Assert.Equal([pack.TLanguageFixtureName + "/British"], stored);
        errand.CErrandCancel();
        await errand.CErrandFlagLoad(store);
        Assert.Equal([pack.TLanguageFixtureName + "/British"], stored);
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task ReadingSet_PrimarySearch_WritesTheIpaAtOnceWithItsVariety()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        errand.CErrandTranscriptionStart(0, string.Empty);

        errand.CErrandReadingSet("hɪl", "British");

        LPronunciationDraft? spoken = editor.CEditorDesk.TDeskHeldRead()?.LDraftContent.LEntryDraftPronunciation;
        Assert.Equal("hɪl", spoken?.LPronunciationDraftIpa);
        Assert.Equal("British", spoken?.LPronunciationDraftVariety);
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void ReadingSet_AccentSearch_WritesTheRowAndSkipsItOnceRemoved()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        editor.CEditorTimbre.CTimbrePronunciationAdd(0);
        long accent = editor.CEditorDesk.TDeskRead()!.LDraftContent.LEntryDraftPronunciations[1].LPronunciationDraftId;
        errand.CErrandTranscriptionStart(accent, string.Empty);

        errand.CErrandReadingSet("hɪl", "British");

        IReadOnlyList<LPronunciationDraft> written =
            editor.CEditorDesk.TDeskHeldRead()!.LDraftContent.LEntryDraftPronunciations;
        Assert.Equal(
            [(string.Empty, string.Empty), ("hɪl", "British")],
            written.Select(static row => (row.LPronunciationDraftIpa, row.LPronunciationDraftVariety)));
        editor.CEditorTimbre.CTimbrePronunciationRemove(accent);
        errand.CErrandReadingSet("hɪːl", "British");
        Assert.Equal(
            [string.Empty],
            editor.CEditorDesk.TDeskRead()!.LDraftContent.LEntryDraftPronunciations.Select(
                static row => row.LPronunciationDraftIpa));
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void ReadingSet_SchemedSearch_WritesTheTranscriptionAtOnceAndNoVariety()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        long entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hill",
            pack.TLanguageFixtureName,
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a rise", 1)],
            [],
            transcriptions: [TInterface.TTranscriptionDraftCreate("Yale", string.Empty)])).LEntryId;
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(entry);
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        long row = Assert.Single(editor.CEditorDesk.TDeskRead()!.LDraftContent.LEntryDraftTranscriptions)
            .LTranscriptionDraftId;
        errand.CErrandTranscriptionStart(row, "Yale");

        errand.CErrandReadingSet("hil", "British");

        LEntryDraft? content = editor.CEditorDesk.TDeskHeldRead()?.LDraftContent;
        Assert.Equal("hil", Assert.Single(content?.LEntryDraftTranscriptions ?? []).LTranscriptionDraftText);
        Assert.Equal(string.Empty, content?.LEntryDraftPronunciation?.LPronunciationDraftVariety ?? string.Empty);
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void ReadingSet_NoSearchOrFillingDesk_WritesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        errand.CErrandReadingSet("hɪl", "British");
        errand.CErrandTranscriptionStart(0, string.Empty);
        bool filling = false;
        editor.CEditorDesk.CDeskDraftChanged += _ =>
        {
            filling = editor.CEditorDesk.CDeskFilling;
            errand.CErrandReadingSet("hɪl", "British");
        };

        editor.CEditorDesk.CDeskDraftResonate();

        Assert.True(filling);
        Assert.Equal(
            string.Empty,
            editor.CEditorDesk.TDeskRead()?.LDraftContent.LEntryDraftPronunciation?.LPronunciationDraftIpa
            ?? string.Empty);
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Theory]
    [InlineData(true, "WAW-tuh")]
    [InlineData(false, "ˈwɔːtə")]
    public void LookupResonate_FoundReadings_ListsEachReadingReadyWithTheSearchMark(bool respelled, string text)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(respelled);
        CEditor editor = TErrandNotationPrepare(engine, "English", "water");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        LForay foray = editor.CEditorDesk.TDeskForayStart(string.Empty)!;
        CNotationRoll? shown = null;
        errand.CErrandNotationChanged += roll => shown = roll;

        errand.TErrandLookupResonate(
            new CLookupStep("Wiktionary", 1, new("Wiktionary", "ˈwɔːtə", 1, true, "British", "WAW-tuh"), false),
            foray);
        errand.TErrandLookupResonate(
            new CLookupStep("Wiktionary", 1, new("Wiktionary", "ˈwɑːtɚ", 1, true, string.Empty, null), false),
            foray);

        Assert.NotNull(shown);
        CNotationItem row = Assert.Single(shown.CNotationRollRows);
        Assert.True(row.CNotationItemReady);
        Assert.Equal(string.Empty, row.CNotationItemNotice);
        CRespellingMark mark = new(respelled, "[", "]");
        Assert.Equal(
            [
                new CNotationReading(
                    "ˈwɔːtə", text, CSounding.CSoundingVarietyRead("English", "British"), true, mark),
                new CNotationReading(
                    "ˈwɑːtɚ", "ˈwɑːtɚ", CSounding.CSoundingVarietyRead("English", string.Empty), false, mark),
            ],
            row.CNotationItemReading);
        foray.TForayCancel();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void LookupResonate_SchemedSearch_ListsThePhoneticBare()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(true);
        CEditor editor = TErrandNotationPrepare(engine, "English", "water");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        LForay foray = editor.CEditorDesk.TDeskForayStart("Yale")!;
        CNotationRoll? shown = null;
        errand.CErrandNotationChanged += roll => shown = roll;

        errand.TErrandLookupResonate(
            new CLookupStep("Yale", 0, new("Yale", "wɔ́ tə", 0, true, string.Empty, "WAW-tuh"), false), foray);

        Assert.NotNull(shown);
        CNotationReading reading = Assert.Single(Assert.Single(shown.CNotationRollRows).CNotationItemReading);
        Assert.Equal("wɔ́ tə", reading.CNotationReadingText);
        Assert.Equal(new CRespellingMark(false, string.Empty, string.Empty), reading.CNotationReadingMark);
        foray.TForayCancel();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void LookupResonate_NoReading_ChoosesMissingWhenReachedAndBrokenOtherwise()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        LForay foray = editor.CEditorDesk.TDeskForayStart(string.Empty)!;
        CNotationRoll? shown = null;
        errand.CErrandNotationChanged += roll => shown = roll;

        errand.TErrandLookupResonate(
            new CLookupStep("Reached", 0, new("Reached", null, 0, true, string.Empty, null), false), foray);
        errand.TErrandLookupResonate(
            new CLookupStep("Lost", 1, new("Lost", null, 1, false, string.Empty, null), false), foray);
        errand.TErrandLookupResonate(
            new CLookupStep("Found", 2, new("Found", "hɪl", 2, true, string.Empty, null), false), foray);
        errand.TErrandLookupResonate(
            new CLookupStep("Found", 2, new("Found", null, 2, true, string.Empty, null), false), foray);

        Assert.NotNull(shown);
        Assert.Equal(
            [("Phonetician.Missing", false, 0), ("Phonetician.Broken", false, 0), (string.Empty, true, 1)],
            shown.CNotationRollRows.Select(
                static row => (row.CNotationItemNotice, row.CNotationItemReady, row.CNotationItemReading.Count)));
        foray.TForayCancel();
        editor.TEditorFinish(false);
    }

    [Fact]
    public void LookupResonate_SourcesThenEnd_KeepsThePackOrderAndEndsOnTheSchemeNotice()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandNotationPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=hɪl", gate.Task));
        CEditor editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        editor.CEditorDesk.CDeskObserverAttach(static _ => { });
        List<CNotationRoll> shown = [];
        errand.CErrandNotationChanged += shown.Add;
        CNotationRoll started = errand.CErrandTranscriptionStart(0, "Yale");
        LForay foray = editor.CEditorDesk.TDeskForayStart("Yale")!;

        errand.TErrandLookupResonate(new CLookupStep("Late", 2, null, false), foray);
        errand.TErrandLookupResonate(new CLookupStep("Early", 1, null, false), foray);
        errand.TErrandLookupResonate(new CLookupStep("Early", 1, null, false), foray);
        errand.TErrandLookupResonate(new CLookupStep(string.Empty, 0, null, true), foray);

        Assert.True(started.CNotationRollSearching);
        Assert.Equal("Phonetician.Searching", started.CNotationRollNotice);
        Assert.Equal(4, shown.Count);
        Assert.True(shown[2].CNotationRollSearching);
        CNotationRoll ended = shown[3];
        Assert.Equal(["Early", "Late"], ended.CNotationRollRows.Select(static row => row.CNotationItemSource));
        Assert.All(
            ended.CNotationRollRows, static row => Assert.Equal("Phonetician.Searching", row.CNotationItemNotice));
        Assert.False(ended.CNotationRollSearching);
        Assert.False(ended.CNotationRollEmpty);
        Assert.Equal("Transcription.Empty", ended.CNotationRollNotice);
        foray.TForayCancel();
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    private static async Task<Action> TErrandStepRead(Channel<Action> marshalled) =>
        await marshalled.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5));

    private static string TErrandNotationRead(CNotationRoll roll) =>
        string.Join(
            ",",
            roll.CNotationRollRows.Select(static row => row.CNotationItemSource + ":"
                + string.Join("|", row.CNotationItemReading.Select(static reading => reading.CNotationReadingText))));

    private static CEditor TErrandNotationPrepare(LEngine engine, string language, string headword)
    {
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("input", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(null);
        editor.CEditorLanguageSet(language);
        editor.CEditorHeadwordSet(headword);
        return editor;
    }
}
