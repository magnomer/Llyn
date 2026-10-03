using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFanqieRank
{
    [Fact]
    public async Task FanqieSet_HeldRank_StoresTheResolvedRank()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFanqieSource.TEngineWikiPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/broad"] = TEngineFanqie.TEngineFanqieBroad,
            ["https://example.test/wiki/%E5%90%B3?raw"] = TEngineFanqieSource.TEngineFanqieWiki,
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));
        LEntry entry = engine.TEngineEntrySave(TEngineFanqie.TFanqieDraftCreate("吳", pack.TLanguageFixtureName));
        engine.TEngineFanqieStart(entry.LEntryId);
        await TEngineFanqie.TFanqieSettle(engine, entry.LEntryId);
        IReadOnlyList<long> ids = engine.TEngineFanqieRead(entry.LEntryId).Select(row => row.LFanqieRowId).ToList();

        engine.TEngineFanqieSet(entry.LEntryId, ids[0], 0, false);
        engine.TEngineFanqieSet(entry.LEntryId, ids[1], 0, true);
        List<int> appended = TFanqieRankRead(engine, entry.LEntryId, ids);
        engine.TEngineFanqieSet(entry.LEntryId, ids[1], 2, true);
        List<int> raised = TFanqieRankRead(engine, entry.LEntryId, ids);
        engine.TEngineFanqieSet(entry.LEntryId, ids[0], 2, false);

        Assert.Equal([1, 2, 0], appended);
        Assert.Equal([2, 1, 0], raised);
        Assert.Equal([0, 1, 0], TFanqieRankRead(engine, entry.LEntryId, ids));
    }

    private static List<int> TFanqieRankRead(LEngine engine, long entryId, IReadOnlyList<long> ids)
    {
        IReadOnlyList<LFanqieRow> rows = engine.TEngineFanqieRead(entryId);
        return ids.Select(id => rows.Single(row => row.LFanqieRowId == id).LFanqieRowRepresentative).ToList();
    }
}
