using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TForay
{
    private const string TForayPack =
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

        LForay foray = tenure.TTenureRecordingStart("tomato", 0, listener);
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
        LForay foray = tenure.TTenureRecordingStart("tomato", 0, TPronunciationHelper.TListenerCreate());
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
        LForay foray = tenure.TTenureRecordingStart("tomato", 0, TPronunciationHelper.TListenerCreate());
        LRecording recording = TInterface.TRecordingCreate(
            "Oxford", "https://example.test/tomato.mp3", 0, true, "British");

        Assert.True(await foray.TForayRecordingSave(recording));

        LEntryDraft? content = tenure.TTenureRead()?.LDraftContent;
        Assert.NotEqual(string.Empty, content?.LEntryDraftAudio);
        Assert.Equal("Oxford", content?.LEntryDraftPronunciation?.LPronunciationDraftSource);
        tenure.TTenureCancel();
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
        TListenerStub listener = TPronunciationHelper.TListenerCreate();
        LForay foray = tenure.TTenureRecordingStart("tomato", 0, listener);

        tenure.TTenureCancel();
        gate.SetResult();

        Assert.Equal(0, listener.TListenerStubFinished);
        Assert.False(await foray.TForayRecordingSave(
            TInterface.TRecordingCreate("Tagged", "https://example.test/gb.mp3", 0, true, "British")));
    }
}
