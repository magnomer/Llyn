using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TForay
{
    internal const string TForayPack =
        """
        { "varieties": { "list": [ { "name": "British" } ] },
          "audio": [
            { "name": "Tagged", "attempts": [ { "urls": ["https://example.test/{word}"],
                "strategy": "regex", "match": "uk=(\\S+)", "group": 1 } ] } ] }
        """;

    [Fact]
    public async Task ForayCancel_SearchInFlight_NeverFinishes()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForayPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=https://example.test/gb.mp3", gate.Task));
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        LForay foray = tenure.TTenureRecordingStart(0, listener.TListenerStubHandle)!;
        foray.TForayCancel();
        gate.SetResult();
        await Task.Delay(200);

        Assert.Equal(
            ("tomato", pack.TLanguageFixtureName, 0L),
            (foray.LForayWord, foray.LForayLanguage, foray.LForayTarget));
        Assert.Equal(["Tagged"], listener.TListenerStubSources);
        Assert.Empty(listener.TListenerStubRecordings);
        Assert.Equal(0, listener.TListenerStubFinished);
        tenure.TTenureCancel();
    }

    [Fact]
    public async Task ForayRecordingSave_HeadwordMoved_AttachesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK));
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, "English"));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        LForay foray = tenure.TTenureRecordingStart(0, TPronunciationHelper.TListenerCreate().TListenerStubHandle)!;
        LRecording recording = TInterface.TRecordingCreate(
            "Oxford", "https://example.test/tomato.mp3", 0, true, "British");

        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "potato"));

        Assert.False(await foray.TForayRecordingSave(recording));
        Assert.Equal(string.Empty, tenure.TTenureRead()?.LDraftContent.LEntryDraftAudio);
        tenure.TTenureCancel();
    }

    [Fact]
    public async Task ForayRecordingSave_DraftUnchanged_AttachesAudio()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK));
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, "English"));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        LForay foray = tenure.TTenureRecordingStart(0, TPronunciationHelper.TListenerCreate().TListenerStubHandle)!;
        LRecording recording = TInterface.TRecordingCreate(
            "Oxford", "https://example.test/tomato.mp3", 0, true, "British");

        Assert.True(await foray.TForayRecordingSave(recording));

        LEntryDraft? content = tenure.TTenureRead()?.LDraftContent;
        Assert.NotEqual(string.Empty, content?.LEntryDraftAudio);
        Assert.Equal("Oxford", content?.LEntryDraftPronunciation?.LPronunciationDraftSource);
        Assert.Equal("British", content?.LEntryDraftPronunciation?.LPronunciationDraftVariety);
        tenure.TTenureCancel();
    }

    [Fact]
    public async Task ForayRecordingPrepare_RemoteAddress_AnswersTheCachedFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK));
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, "English"));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        LForay foray = tenure.TTenureRecordingStart(0, TPronunciationHelper.TListenerCreate().TListenerStubHandle)!;
        LRecording recording = TInterface.TRecordingCreate(
            "Oxford", "https://example.test/tomato.mp3", 0, true, "British");

        string? path = await foray.TForayRecordingPrepare(recording);

        Assert.NotNull(path);
        Assert.Equal("audio", await File.ReadAllTextAsync(path));
        Assert.Equal(string.Empty, tenure.TTenureRead()?.LDraftContent.LEntryDraftAudio);
        tenure.TTenureCancel();
    }

    [Fact]
    public async Task ForayRecordingPrepare_HostRefuses_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new HttpRequestException("refused")));
        LForay foray = TForayStart(engine, "English");

        Assert.Null(await foray.TForayRecordingPrepare(
            TInterface.TRecordingCreate("Oxford", "https://example.test/tomato.mp3", 0, true, "British")));
    }

    [Fact]
    public async Task ForayRecordingSave_HostRefuses_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new HttpRequestException("refused")));
        LForay foray = TForayStart(engine, "English");

        Assert.Null(await foray.TForayRecordingSave(
            TInterface.TRecordingCreate("Oxford", "https://example.test/tomato.mp3", 0, true, "British")));
    }

    [Fact]
    public async Task ForayRecordingSave_AudioFolderBlocked_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "audio"), string.Empty);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK));
        LForay foray = TForayStart(engine, "English");

        Assert.Null(await foray.TForayRecordingSave(
            TInterface.TRecordingCreate("Oxford", "https://example.test/tomato.mp3", 0, true, "British")));
    }

    [Fact]
    public async Task ForayRecordingPrepare_CacheFolderBlocked_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "temp"), string.Empty);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("audio", HttpStatusCode.OK));
        LForay foray = TForayStart(engine, "English");

        Assert.Null(await foray.TForayRecordingPrepare(
            TInterface.TRecordingCreate("Oxford", "https://example.test/tomato.mp3", 0, true, "British")));
    }

    [Fact]
    public async Task ForayEnsignLoad_FlagFileFails_StoresNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            "{ \"varieties\": { \"shown\": \"flag\", \"list\": [ { \"name\": \"British\", \"flag\": \"gb\" } ] } }");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "flags"), string.Empty);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("<svg/>", HttpStatusCode.OK));
        LForay foray = TForayStart(engine, pack.TLanguageFixtureName);
        List<LEnsignRow> stored = [];

        await foray.TForayEnsignLoad((rows, _) => () => stored.AddRange(rows));

        Assert.True(foray.LForayFlagged);
        Assert.Empty(stored);
    }

    [Fact]
    public async Task TenureCancel_ForayOpen_CancelsForay()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForayPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=https://example.test/gb.mp3", gate.Task));
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        TListenerStub listener = TPronunciationHelper.TListenerCreate();
        LForay foray = tenure.TTenureRecordingStart(0, listener.TListenerStubHandle)!;

        tenure.TTenureCancel();
        gate.SetResult();

        Assert.Equal(0, listener.TListenerStubFinished);
        Assert.False(await foray.TForayRecordingSave(
            TInterface.TRecordingCreate("Tagged", "https://example.test/gb.mp3", 0, true, "British")));
    }

    [Fact]
    public void RecordingStart_HeadwordPadded_SearchesTheTrimmedWord()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForayPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=https://example.test/gb.mp3", gate.Task));
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "  tomato  "));

        LForay? foray = tenure.TTenureRecordingStart(0, TPronunciationHelper.TListenerCreate().TListenerStubHandle);

        Assert.Equal("tomato", foray?.LForayWord);
        foray?.TForayCancel();
        gate.SetResult();
        tenure.TTenureCancel();
    }

    [Fact]
    public void RecordingStart_BlankHeadword_StartsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, "English"));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "   "));
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        Assert.Null(tenure.TTenureRecordingStart(0, listener.TListenerStubHandle));
        Assert.Empty(listener.TListenerStubSources);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TranscriptionStart_HeadwordPadded_SearchesTheTrimmedWord()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForayPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=https://example.test/gb.mp3", gate.Task));
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestDefer(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "  tomato  "));

        LForay? foray = tenure.TTenureTranscriptionStart(4, "Yale", static _ => { });

        Assert.Equal(
            ("tomato", pack.TLanguageFixtureName, 4L, "Yale"),
            (foray?.LForayWord, foray?.LForayLanguage, foray?.LForayTarget, foray?.LForayScheme));
        foray?.TForayCancel();
        gate.SetResult();
        tenure.TTenureCancel();
    }

    [Fact]
    public void TranscriptionStart_BlankHeadword_StartsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, "English"));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "   "));
        int steps = 0;

        Assert.Null(tenure.TTenureTranscriptionStart(0, string.Empty, _ => Interlocked.Increment(ref steps)));
        Assert.Equal(0, Volatile.Read(ref steps));
        tenure.TTenureCancel();
    }

    [Fact]
    public void RecordingStart_WorkspaceChanged_StartsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, "English"));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        engine.TEngineWorkspaceOpen(workspace.TWorkspaceFolder);
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        Assert.Null(tenure.TTenureRecordingStart(0, listener.TListenerStubHandle));
        Assert.Empty(listener.TListenerStubSources);
    }

    [Fact]
    public void TranscriptionStart_WorkspaceChanged_StartsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, "English"));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        engine.TEngineWorkspaceOpen(workspace.TWorkspaceFolder);
        int steps = 0;

        Assert.Null(tenure.TTenureTranscriptionStart(0, "ipa", _ => Interlocked.Increment(ref steps)));
        Assert.Equal(0, Volatile.Read(ref steps));
    }

    [Fact]
    public async Task ForayEnsignLoad_UnflaggedPack_StoresNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TForayPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TaskCompletionSource gate = new();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("uk=https://example.test/gb.mp3", gate.Task));
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, pack.TLanguageFixtureName));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        LForay foray = tenure.TTenureRecordingStart(0, TPronunciationHelper.TListenerCreate().TListenerStubHandle)!;
        int stored = 0;

        await foray.TForayEnsignLoad((_, _) =>
        {
            stored++;
            return static () => { };
        });

        Assert.False(foray.LForayFlagged);
        Assert.Equal(0, stored);
        foray.TForayCancel();
        gate.SetResult();
        tenure.TTenureCancel();
    }

    private static LForay TForayStart(LEngine engine, string language)
    {
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestApply(TInterface.TRequestLanguageCreate(tenure.LTenureId, language));
        tenure.TTenureRequestApply(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "tomato"));
        return tenure.TTenureRecordingStart(0, TPronunciationHelper.TListenerCreate().TListenerStubHandle)!;
    }
}
