using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFrequency
{
    internal const string TEngineFrequencyPack =
        """
        { "frequency": [
            { "name": "First", "attempts": [
                { "urls": ["https://example.test/a/{word}"], "strategy": "regex", "match": "a=(\\S+)", "group": 1 } ],
              "once": { "total": 1000000 },
              "bands": [
                { "match": "^[SW]1$", "name": "Core" },
                { "match": "^\\d+$", "name": "Rare" } ] },
            { "name": "Second", "attempts": [
                { "urls": ["https://example.test/b/{word}"], "strategy": "regex",
                  "match": "b=(\\S+)", "group": 1 } ],
              "once": { "factor": 12 },
              "bands": [
                { "match": "^unranked$", "name": "Rare" } ] },
            { "name": "Third", "attempts": [
                { "urls": ["https://example.test/c/{word}"], "strategy": "regex",
                  "match": "c=(\\S+)", "group": 1 } ],
              "unit": "Level" } ] }
        """;

    internal static readonly TimeSpan TEngineFrequencyPatience = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task FrequencyStart_FirstSourceSilent_StoresSecondAlone()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/a/tomato"] = "nothing here",
                ["https://example.test/b/tomato"] = "b=7",
            }));

        IReadOnlyList<LFrequency> found = await TFrequencyFetchRead(engine, pack.TLanguageFixtureName);

        LFrequency row = Assert.Single(found);
        Assert.Equal(
            ("Second", "7", "Core", 84L),
            (row.LFrequencySource, row.LFrequencyRaw, row.LFrequencyBand, row.LFrequencyOnce));
    }

    [Fact]
    public async Task FrequencyStart_BothSourcesAnswer_StoresBothInOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/a/tomato"] = "a=S1",
                ["https://example.test/b/tomato"] = "b=250",
            }));

        IReadOnlyList<LFrequency> found = await TFrequencyFetchRead(engine, pack.TLanguageFixtureName);

        Assert.Equal(
            [("First", "S1", "Core", null), ("Second", "250", "Core", 3000L)],
            found.Select(row => (row.LFrequencySource, row.LFrequencyRaw, row.LFrequencyBand, row.LFrequencyOnce)));
    }

    [Fact]
    public async Task FrequencyStart_UnitSource_StampsUnitOnNumericOnly()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/a/tomato"] = "a=S1",
                ["https://example.test/c/tomato"] = "c=3",
            }));

        IReadOnlyList<LFrequency> found = await TFrequencyFetchRead(engine, pack.TLanguageFixtureName);

        Assert.Equal(
            [("First", "S1", null, null), ("Third", "3", "Level", null)],
            found.Select(row => (row.LFrequencySource, row.LFrequencyRaw, row.LFrequencyUnit, row.LFrequencyOnce)));
    }

    [Fact]
    public async Task FrequencyStart_FirstSourceAnswers_AsksEverySource()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("a=2.5", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));

        IReadOnlyList<LFrequency> found = await TFrequencyFetchRead(engine, pack.TLanguageFixtureName);

        LFrequency row = Assert.Single(found);
        Assert.Equal(
            ("First", "2.5", "Advanced", 400000L),
            (row.LFrequencySource, row.LFrequencyRaw, row.LFrequencyBand, row.LFrequencyOnce));
        Assert.Equal(3, handler.TSourceHandlerCount);
    }

    private static async Task<IReadOnlyList<LFrequency>> TFrequencyFetchRead(LEngine engine, string language)
    {
        TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer.TFrequencyObserverHandle);
        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", language));
        await observer.TFrequencyObserverRaised.WaitAsync(TEngineFrequencyPatience);
        return engine.TEngineFrequencyRead(entry.LEntryId);
    }

    internal static LEntryDraft TFrequencyDraftCreate(string headword, string language)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a red fruit", [], [], [], [], [], 1)],
            []);
    }

    internal static void TFrequencyStoredSet(
        TWorkspace workspace, long entryId, string source, string raw, string? band)
    {
        string label = band is null ? "NULL" : $"'{band}'";
        workspace.TWorkspaceScriptRun(
            "INSERT OR REPLACE INTO frequency (entry_parent, source, raw, band, fetched_utc) " +
            $"VALUES ({entryId}, '{source}', '{raw}', {label}, '2026-01-01');");
    }

    internal static long TFrequencyCountRead(TWorkspace workspace, long entryId)
    {
        return workspace.TWorkspaceCountRead($"SELECT COUNT(*) FROM frequency WHERE entry_parent = {entryId};");
    }

    internal sealed class TFrequencyObserver
    {
        private readonly TaskCompletionSource<LBulletin> _tFrequencyObserverRaised =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal Task<LBulletin> TFrequencyObserverRaised => _tFrequencyObserverRaised.Task;

        internal void TFrequencyObserverHandle(LBulletin bulletin)
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectFrequency)
            {
                _tFrequencyObserverRaised.TrySetResult(bulletin);
            }
        }
    }
}
