using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFrequencyBand
{
    [Fact]
    public void FrequencyRead_StoredRaw_GradesByIntervalBeforePatterns()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        long entryId = engine.TEngineEntrySave(draft).LEntryId;

        Assert.Equal("Advanced", TFrequencyBandRead(workspace, engine, entryId, "First", "5"));
        Assert.Equal("Everyday", TFrequencyBandRead(workspace, engine, entryId, "First", "50"));
        Assert.Equal("Core", TFrequencyBandRead(workspace, engine, entryId, "First", "500"));
        Assert.Equal("Rare", TFrequencyBandRead(workspace, engine, entryId, "First", "0.5"));
        Assert.Equal("Core", TFrequencyBandRead(workspace, engine, entryId, "First", "W1"));
        Assert.Null(TFrequencyBandRead(workspace, engine, entryId, "First", "W2"));
        Assert.Equal("Core", TFrequencyBandRead(workspace, engine, entryId, "Second", "100"));
        Assert.Equal("Advanced", TFrequencyBandRead(workspace, engine, entryId, "Second", "50000"));
        Assert.Equal("Rare", TFrequencyBandRead(workspace, engine, entryId, "Second", "unranked"));
        Assert.Null(TFrequencyBandRead(workspace, engine, entryId, "Third", "5"));
        Assert.Null(TFrequencyBandRead(workspace, engine, entryId, "Fourth", "5"));
    }

    [Fact]
    public void FrequencyRead_StaleStoredBand_RegradesFromRawAndStoresIt()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        LEntry entry = engine.TEngineEntrySave(draft);
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "First", "5", "Stale");

        IReadOnlyList<LFrequency> read = engine.TEngineFrequencyRead(entry.LEntryId);

        LFrequency row = Assert.Single(read);
        Assert.Equal(
            ("First", "5", "Advanced", 200000L),
            (row.LFrequencySource, row.LFrequencyRaw, row.LFrequencyBand, row.LFrequencyOnce));
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            $"SELECT COUNT(*) FROM frequency WHERE entry_parent = {entry.LEntryId} AND band = 'Advanced';"));
    }

    [Fact]
    public void FrequencyRead_MigratedRowWithoutBand_ResolvesAndStoresIt()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        LEntry entry = engine.TEngineEntrySave(draft);
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "First", "5", null);

        IReadOnlyList<LFrequency> read = engine.TEngineFrequencyRead(entry.LEntryId);

        Assert.Equal("Advanced", Assert.Single(read).LFrequencyBand);
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            $"SELECT COUNT(*) FROM frequency WHERE entry_parent = {entry.LEntryId} AND band = 'Advanced';"));
    }

    [Fact]
    public void FrequencyRead_BlankLanguageWithStoredRow_LeavesBandAndOnceNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TEngineFrequency.TFrequencyDraftCreate("tomato", "English"));
        workspace.TWorkspaceScriptRun($"UPDATE entry SET language = '' WHERE entry_id = {entry.LEntryId};");
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "First", "5", null);

        LFrequency read = Assert.Single(engine.TEngineFrequencyRead(entry.LEntryId));

        Assert.Equal(("First", "5"), (read.LFrequencySource, read.LFrequencyRaw));
        Assert.Null(read.LFrequencyBand);
        Assert.Null(read.LFrequencyOnce);
    }

    private static string? TFrequencyBandRead(
        TWorkspace workspace, LEngine engine, long entryId, string source, string raw)
    {
        workspace.TWorkspaceScriptRun($"DELETE FROM frequency WHERE entry_parent = {entryId};");
        TEngineFrequency.TFrequencyStoredSet(workspace, entryId, source, raw, null);
        return Assert.Single(engine.TEngineFrequencyRead(entryId)).LFrequencyBand;
    }
}
