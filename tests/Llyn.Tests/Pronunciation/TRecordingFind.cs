using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRecordingFind
{
    private const string TRecordingFindBody =
        "uk=https://example.test/gb.mp3 us=https://example.test/us.mp3 flat=https://example.test/flat.mp3";

    private const string TRecordingFindPack =
        """
        { "varieties": { "list": [ { "name": "British" }, { "name": "American" } ] },
          "audio": [
            { "name": "Tagged", "attempts": [ { "urls": ["https://example.test/{word}"], "readings": [
                { "variety": "British", "strategy": "regex", "match": "uk=(\\S+)", "group": 1 },
                { "variety": "American", "strategy": "regex", "match": "us=(\\S+)", "group": 1 } ] } ] },
            { "name": "Flat", "attempts": [ { "urls": ["https://example.test/{word}"],
                "strategy": "regex", "match": "flat=(\\S+)", "group": 1 } ] } ] }
        """;

    [Fact]
    public async Task RecordingFind_PrimaryRowAmerican_ReturnsAmericanOnly()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TRecordingFindPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TRecordingFindBody, HttpStatusCode.OK));
        LDraft started = engine.TEngineDraftStart("Input", null);
        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "təˈmeɪtoʊ"));
        long primary = answered.LDraftContent.LEntryDraftPronunciations[0].LPronunciationDraftId;
        engine.TEngineRequestApply(TInterface.TPronunciationVarietyCreate(started.LDraftId, primary, "American"));
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        await engine.TEngineRecordingFind(
            started.LDraftId, "tomato", pack.TLanguageFixtureName, 0, listener.TListenerStubHandle,
            CancellationToken.None);

        Assert.Equal(
            [
                ("Tagged", "American", "https://example.test/us.mp3"),
                ("Flat", "American", "https://example.test/flat.mp3"),
            ],
            listener.TListenerStubRecordings
                .OrderBy(recording => recording.LRecordingOrder)
                .Select(recording =>
                    (recording.LRecordingSource, recording.LRecordingVariety, recording.LRecordingAddress)));
        Assert.Equal(1, listener.TListenerStubFinished);
    }

    [Fact]
    public async Task RecordingFind_PrimaryRowUntagged_ReturnsBothVarieties()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TRecordingFindPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TRecordingFindBody, HttpStatusCode.OK));
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "təˈmɑːtəʊ"));
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        await engine.TEngineRecordingFind(
            started.LDraftId, "tomato", pack.TLanguageFixtureName, 0, listener.TListenerStubHandle,
            CancellationToken.None);

        Assert.Equal(
            [
                ("Tagged", "British", "https://example.test/gb.mp3"),
                ("Tagged", "American", "https://example.test/us.mp3"),
                ("Flat", "British", "https://example.test/flat.mp3"),
                ("Flat", "American", "https://example.test/flat.mp3"),
            ],
            listener.TListenerStubRecordings
                .OrderBy(recording => recording.LRecordingOrder)
                .Select(recording =>
                    (recording.LRecordingSource, recording.LRecordingVariety, recording.LRecordingAddress)));
    }

    [Fact]
    public async Task RecordingFind_RowVarietyChanged_SearchesAgain()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TRecordingFindPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TRecordingFindBody, HttpStatusCode.OK));
        LDraft started = engine.TEngineDraftStart("Input", null);
        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestIpaCreate(started.LDraftId, "təˈmeɪtoʊ"));
        long primary = answered.LDraftContent.LEntryDraftPronunciations[0].LPronunciationDraftId;
        TListenerStub first = TPronunciationHelper.TListenerCreate();
        await engine.TEngineRecordingFind(
            started.LDraftId, "tomato", pack.TLanguageFixtureName, 0, first.TListenerStubHandle,
            CancellationToken.None);

        engine.TEngineRequestApply(TInterface.TPronunciationVarietyCreate(started.LDraftId, primary, "British"));
        TListenerStub second = TPronunciationHelper.TListenerCreate();
        await engine.TEngineRecordingFind(
            started.LDraftId, "tomato", pack.TLanguageFixtureName, 0, second.TListenerStubHandle,
            CancellationToken.None);

        Assert.Equal(4, first.TListenerStubRecordings.Count);
        Assert.Equal(
            ["British", "British"],
            second.TListenerStubRecordings.Select(recording => recording.LRecordingVariety));
    }
}
