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

public sealed class TErrandClip
{
    private const string TErrandClipPack =
        """
        { "varieties": { "list": [ { "name": "British" } ] },
          "audio": [
            { "name": "Tagged", "attempts": [ { "urls": ["https://example.test/{word}"],
                "strategy": "regex", "match": "uk=(\\S+)", "group": 1 } ] } ] }
        """;

    [Fact]
    public async Task RecordingStart_HeldDraft_FillsTheClipFromEveryStep()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TErrandClipPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=https://example.test/gb.mp3", Task.CompletedTask));
        engine.TEngineDelaySet(0);
        CEditor editor = TErrandClipPrepare(engine);
        editor.CEditorEntryOpen(null);
        editor.CEditorLanguageSet(pack.TLanguageFixtureName);
        editor.CEditorHeadwordSet(" tomato ");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        List<string> notices = [];
        TaskCompletionSource<CClipRoll> finished = new(TaskCreationOptions.RunContinuationsAsynchronously);
        errand.CErrandClipChanged += roll =>
        {
            if (!roll.CClipRollSearching)
            {
                finished.TrySetResult(roll);
                return;
            }

            notices.Add(roll.CClipRollRows[0].CClipItemReady ? "recording" : "source");
        };

        errand.CErrandRecordingStart(0);
        CClipRoll ended = await finished.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(["source", "recording"], notices);
        Assert.False(ended.CClipRollSearching);
        Assert.False(ended.CClipRollEmpty);
        CClipItem row = Assert.Single(ended.CClipRollRows);
        Assert.Equal("Tagged", row.CClipItemSource);
        Assert.True(row.CClipItemReady);
        Assert.Equal(string.Empty, row.CClipItemNotice);
        CClipReading reading = Assert.Single(row.CClipItemReading);
        Assert.Equal("https://example.test/gb.mp3", reading.CClipReadingRecording.CRecordingAddress);
        Assert.Equal(
            CSounding.CSoundingVarietyRead(pack.TLanguageFixtureName, reading.CClipReadingRecording.CRecordingVariety),
            reading.CClipReadingVariety);
        Assert.False(reading.CClipReadingFlagged);
        Assert.Equal("Downloader.Use", reading.CClipReadingAction);
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task RecordingStart_BlankHeadword_AnswersTheEmptyNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        CEditor editor = TErrandClipPrepare(engine);
        editor.CEditorEntryOpen(null);
        editor.CEditorLanguageSet("English");
        editor.CEditorHeadwordSet("   ");

        CClipRoll roll = editor.CEditorDesk.CDeskErrand.CErrandRecordingStart(0);

        Assert.Empty(roll.CClipRollRows);
        Assert.True(roll.CClipRollEmpty);
        Assert.False(roll.CClipRollSearching);
        Assert.Equal("Downloader.Empty", roll.CClipRollNotice);
        Assert.Null(await editor.CEditorDesk.CDeskErrand.CErrandPreviewStart(
            new CRecording("Tagged", "https://example.test/gb.mp3", 0, true, "British")));
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task ErrandEnsignLoad_FlaggedPack_StoresTheVarietyFlagsAndAnswersTheClip()
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
            TPronunciationHelper.TSourceClientCreate("uk=https://example.test/gb.mp3", gate.Task));
        engine.TEngineDelaySet(0);
        CEditor editor = TErrandClipPrepare(engine);
        editor.CEditorEntryOpen(null);
        editor.CEditorLanguageSet(pack.TLanguageFixtureName);
        editor.CEditorHeadwordSet("hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        errand.CErrandRecordingStart(0);
        List<string> stored = [];

        CClipRoll roll = await errand.CErrandEnsignLoad((rows, _) =>
        {
            stored.AddRange(rows.Select(static row => row.CEnsignRowKey));
            return static () => { };
        });

        Assert.Equal([pack.TLanguageFixtureName + "/British"], stored);
        Assert.True(roll.CClipRollEmpty);
        errand.CErrandCancel();
        gate.SetResult();
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task ErrandEnsignLoad_NoSearch_StoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TErrandClipPrepare(engine);
        int stored = 0;

        CClipRoll roll = await editor.CEditorDesk.CDeskErrand.CErrandEnsignLoad((_, _) =>
        {
            stored++;
            return static () => { };
        });

        Assert.Equal(0, stored);
        Assert.True(roll.CClipRollEmpty);
        Assert.False(roll.CClipRollSearching);
    }

    [Fact]
    public async Task PreviewStart_StepLandsMidFetch_KeepsThePreviewState()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("audio", gate.Task));
        engine.TEngineDelaySet(0);
        CEditor editor = TErrandClipPrepare(engine);
        CErrand errand = await TErrandClipStart(editor, pack.TLanguageFixtureName);
        CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
        List<CClipRoll> changes = [];
        errand.CErrandClipChanged += changes.Add;

        Task<Uri?> preview = errand.CErrandPreviewStart(found);
        errand.TErrandHarvestResonate(new CHarvestStep("Other", 1, null, false));

        Assert.Equal(2, changes.Count);
        CClipRoll landed = changes[^1];
        Assert.Equal(2, landed.CClipRollRows.Count);
        CClipReading fetching = landed.CClipRollRows[0].CClipItemReading[0];
        Assert.True(fetching.CClipReadingFetching);
        Assert.False(fetching.CClipReadingPlaying);
        gate.SetResult();
        Uri? address = await preview.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.NotNull(address);
        Assert.Equal("audio", await File.ReadAllTextAsync(address.LocalPath));
        Assert.Equal(3, changes.Count);
        CClipReading playing = changes[^1].CClipRollRows[0].CClipItemReading[0];
        Assert.False(playing.CClipReadingFetching);
        Assert.True(playing.CClipReadingPlaying);

        errand.CErrandPreviewFinish();
        errand.CErrandPreviewFinish();

        Assert.Equal(4, changes.Count);
        Assert.False(changes[^1].CClipRollRows[0].CClipItemReading[0].CClipReadingPlaying);
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task PreviewStart_HostRefuses_MarksTheReadingRefused()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new Dictionary<string, string>()));
        engine.TEngineDelaySet(0);
        CEditor editor = TErrandClipPrepare(engine);
        CErrand errand = await TErrandClipStart(editor, pack.TLanguageFixtureName);
        CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
        List<CClipRoll> changes = [];
        errand.CErrandClipChanged += changes.Add;

        Uri? address = await errand.CErrandPreviewStart(found).WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Null(address);
        Assert.Equal(2, changes.Count);
        Assert.True(changes[0].CClipRollRows[0].CClipItemReading[0].CClipReadingFetching);
        CClipReading refused = changes[^1].CClipRollRows[0].CClipItemReading[0];
        Assert.False(refused.CClipReadingFetching);
        Assert.False(refused.CClipReadingPlaying);
        Assert.True(refused.CClipReadingRefused);
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task PreviewStart_ClipCancelledMidFetch_PlaysNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("audio", gate.Task));
        engine.TEngineDelaySet(0);
        CEditor editor = TErrandClipPrepare(engine);
        CErrand errand = await TErrandClipStart(editor, pack.TLanguageFixtureName);
        CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
        List<CClipRoll> changes = [];
        errand.CErrandClipChanged += changes.Add;

        Task<Uri?> preview = errand.CErrandPreviewStart(found);
        errand.CErrandCancel();
        gate.SetResult();

        Assert.Null(await preview.WaitAsync(TimeSpan.FromSeconds(5)));
        Assert.Single(changes);
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task RecordingSave_HeldSearch_SavesTheRecordingWithItsVariety()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("audio", gate.Task));
        engine.TEngineDelaySet(0);
        CEditor editor = TErrandClipPrepare(engine);
        CErrand errand = await TErrandClipStart(editor, pack.TLanguageFixtureName);
        CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
        List<CClipRoll> changes = [];
        errand.CErrandClipChanged += changes.Add;

        Task<bool> saved = errand.CErrandRecordingSave(found);
        errand.TErrandHarvestResonate(new CHarvestStep("Other", 1, null, false));
        bool repeated = await errand.CErrandRecordingSave(found);

        Assert.False(repeated);
        Assert.Equal(2, changes.Count);
        CClipRoll landed = changes[^1];
        Assert.Equal(2, landed.CClipRollRows.Count);
        CClipReading saving = landed.CClipRollRows[0].CClipItemReading[0];
        Assert.Equal("Downloader.Saving", saving.CClipReadingAction);
        Assert.False(saving.CClipReadingReady);
        gate.SetResult();
        Assert.True(await saved.WaitAsync(TimeSpan.FromSeconds(5)));
        CClipReading done = changes[^1].CClipRollRows[0].CClipItemReading[0];
        Assert.Equal("Downloader.Saved", done.CClipReadingAction);
        Assert.False(done.CClipReadingReady);
        LPronunciationDraft? spoken = editor.CEditorDesk.TDeskRead()?.LDraftContent.LEntryDraftPronunciation;
        Assert.Equal("British", spoken?.LPronunciationDraftVariety);
        Assert.NotEqual(string.Empty, spoken?.LPronunciationDraftAudio ?? string.Empty);
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task RecordingSave_HostRefuses_OffersTheRetry()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new Dictionary<string, string>()));
        engine.TEngineDelaySet(0);
        CEditor editor = TErrandClipPrepare(engine);
        CErrand errand = await TErrandClipStart(editor, pack.TLanguageFixtureName);
        CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
        List<CClipRoll> changes = [];
        errand.CErrandClipChanged += changes.Add;

        Assert.False(await errand.CErrandRecordingSave(found).WaitAsync(TimeSpan.FromSeconds(5)));

        Assert.Equal(2, changes.Count);
        CClipReading retry = changes[^1].CClipRollRows[0].CClipItemReading[0];
        Assert.Equal("Downloader.Retry", retry.CClipReadingAction);
        Assert.True(retry.CClipReadingReady);
        Assert.Equal(
            string.Empty,
            editor.CEditorDesk.TDeskRead()?.LDraftContent.LEntryDraftPronunciation?.LPronunciationDraftAudio
            ?? string.Empty);
        editor.TEditorFinish(false);
    }

    private static async Task<CErrand> TErrandClipStart(CEditor editor, string language)
    {
        editor.CEditorEntryOpen(null);
        editor.CEditorLanguageSet(language);
        editor.CEditorHeadwordSet("hill");
        CErrand errand = editor.CEditorDesk.CDeskErrand;
        TaskCompletionSource finished = new(TaskCreationOptions.RunContinuationsAsynchronously);
        errand.CErrandClipChanged += roll =>
        {
            if (!roll.CClipRollSearching)
            {
                finished.TrySetResult();
            }
        };
        errand.CErrandRecordingStart(0);
        await finished.Task.WaitAsync(TimeSpan.FromSeconds(5));
        return errand;
    }

    private static CEditor TErrandClipPrepare(LEngine engine)
    {
        CEditor editor = TInterfaceEditor.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("input", LCatalogOrder.LCatalogOrderHeadword));
        return editor;
    }
}
