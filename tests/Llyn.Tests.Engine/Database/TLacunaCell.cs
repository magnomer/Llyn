using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLacunaCell
{
    [Fact]
    public void LacunaSave_CellMiss_ReadsBackCell()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "go", "English", string.Empty, string.Empty, [TInterface.TCardCreate("to move", 1)], []));
        long morphologyId = workspace.TWorkspaceCountRead("SELECT MIN(morphology_value_id) FROM morphology_value;");
        LLacunaArchive lacunae = TInterface.TLacunaArchiveCreate(workspace.TWorkspaceDatabase);
        LLacuna single = new(morphologyId);
        LLacuna cell = new(morphologyId, "3+5");

        lacunae.TLacunaSave(entry.LEntryId, [single, cell]);

        Assert.Equal([single, cell], lacunae.TLacunaRead(entry.LEntryId));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM lacuna WHERE cell = '3+5';"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM lacuna WHERE cell = '';"));
    }

    [Fact]
    public void DatabaseCreate_OlderLacuna_CarriesRowsWithEmptyCell()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        workspace.TWorkspaceScriptRun(
            "DROP TABLE lacuna; " +
            "CREATE TABLE lacuna (entry_parent INTEGER NOT NULL, morphology_value_ref INTEGER, " +
            "fetched_utc TEXT NOT NULL, PRIMARY KEY (entry_parent, morphology_value_ref)); " +
            "INSERT INTO entry (headword, language, added_utc, updated_utc) " +
            "VALUES ('word', 'English', '2026-01-01', '2026-01-01'); " +
            "INSERT INTO lacuna (entry_parent, morphology_value_ref, fetched_utc) " +
            "VALUES (last_insert_rowid(), NULL, '2026-01-01'); " +
            $"UPDATE schema_version SET version = {LSchemaMigration.LSchemaMigrationVersion - 1};");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM lacuna;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM lacuna WHERE cell = '';"));
    }
}
