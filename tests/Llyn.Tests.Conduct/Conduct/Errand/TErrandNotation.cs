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
    internal const string TErrandNotationPack =
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
        TEditorFixture editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, " hill ");
        CErrand errand = editor.TEditorFixtureDesk.CDeskErrand;
        editor.TEditorFixtureDesk.TDeskRead();
        Channel<Action> marshalled = Channel.CreateUnbounded<Action>();
        editor.TEditorFixtureDesk.CDeskObserverAttach(run => marshalled.Writer.TryWrite(run));
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
            CVariety.CVarietyRead(pack.TLanguageFixtureName, "British"), reading.CNotationReadingVariety);
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
        TEditorFixture editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CDesk desk = editor.TEditorFixtureDesk;
        CErrand errand = desk.CDeskErrand;
        editor.TEditorFixtureTimbre.CTimbrePronunciationAdd(0);
        long accent = desk.TDeskRead()!.LDraftContent.LEntryDraftPronunciations[1].LPronunciationDraftId;
        Channel<Action> marshalled = Channel.CreateUnbounded<Action>();
        desk.CDeskObserverAttach(run => marshalled.Writer.TryWrite(run));
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
        TEditorFixture editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.TEditorFixtureDesk.CDeskErrand;
        errand.CErrandRecordingStart(0);
        editor.TEditorFixtureEntry.CEntryHeadwordSet("   ");

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
        TEditorFixture editor = TErrandNotationPrepare(engine, pack.TLanguageFixtureName, "hill");
        CErrand errand = editor.TEditorFixtureDesk.CDeskErrand;
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

    internal static async Task<Action> TErrandStepRead(Channel<Action> marshalled) =>
        await marshalled.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5));

    internal static string TErrandNotationRead(CNotationRoll roll) =>
        string.Join(
            ",",
            roll.CNotationRollRows.Select(static row => row.CNotationItemSource + ":"
                + string.Join("|", row.CNotationItemReading.Select(static reading => reading.CNotationReadingText))));

    internal static TEditorFixture TErrandNotationPrepare(LEngine engine, string language, string headword)
    {
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("input", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(null);
        editor.TEditorFixtureEntry.CEntryLanguageSet(language);
        editor.TEditorFixtureEntry.CEntryHeadwordSet(headword);
        return editor;
    }
}
