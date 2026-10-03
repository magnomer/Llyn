using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Llyn.Tests;

public sealed class TSchemaMigration
{
    [Fact]
    public void DatabaseCreate_RunTwice_LeavesOneVersionRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceDatabase.TDatabaseCreate();
        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM schema_version;"));
        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
    }

    [Fact]
    public void WorkspacePrepare_NewConnection_EnforcesForeignKeys()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        Assert.Equal(1, workspace.TWorkspaceCountRead("PRAGMA foreign_keys;"));
    }

    [Fact]
    public void DatabaseCreate_OlderBuild_CarriesKnownColumnsAcross()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Guid realm = TInterface.TRealmValueRead(workspace);

        workspace.TWorkspaceScriptRun(
            "ALTER TABLE entry ADD COLUMN mystery TEXT; " +
            "INSERT INTO entry (headword, language, added_utc, updated_utc, mystery) " +
            "VALUES ('word', 'English', '2026-01-01', '2026-01-01', 'gone'); " +
            "UPDATE schema_version SET version = 32;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry WHERE headword = 'word';"));
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM pragma_table_info('entry') WHERE name = 'mystery';"));
        Assert.Equal(realm, TInterface.TRealmValueRead(workspace));
        Assert.Single(Directory.GetFiles(workspace.TWorkspaceFolder, "*.v32.db"));
    }

    [Fact]
    public void DatabaseCreate_OlderBuild_KeepsTheFileAndLeavesNoResidue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        string file = Path.Combine(workspace.TWorkspaceFolder, "llyn.db");
        workspace.TWorkspaceScriptRun("UPDATE schema_version SET version = 32;");
        SqliteConnection.ClearAllPools();
        DateTime born = File.GetCreationTimeUtc(file);

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(born, File.GetCreationTimeUtc(file));
        Assert.False(File.Exists(file + ".fresh"));
        Assert.False(File.Exists(file + ".fresh-wal"));
        Assert.Single(Directory.GetFiles(workspace.TWorkspaceFolder, "*.v32.db"));
    }

    [Fact]
    public void DatabaseCreate_RebuildFails_LeavesTheOldFileUntouched()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        workspace.TWorkspaceScriptRun(
            "INSERT INTO entry (headword, language, added_utc, updated_utc) " +
            "VALUES ('word', 'English', '2026-01-01', '2026-01-01'); " +
            "UPDATE schema_version SET version = 32;");
        SqliteConnection.ClearAllPools();
        string fresh = Path.Combine(workspace.TWorkspaceFolder, "llyn.db.fresh");
        Directory.CreateDirectory(fresh);

        Assert.Throws<SqliteException>(() => workspace.TWorkspaceDatabase.TDatabaseCreate());

        Assert.Equal(32, workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry WHERE headword = 'word';"));
        Assert.Empty(Directory.GetFiles(workspace.TWorkspaceFolder, "*.v32.db"));
    }

    [Fact]
    public void DatabaseCreate_OlderBuild_DropsRowsTheSchemaRefuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO entry (headword, language, added_utc, updated_utc) " +
            "VALUES ('word', 'English', '2026-01-01', '2026-01-01'); " +
            "INSERT INTO collocation (entry_parent, position) SELECT entry_id, 0 FROM entry; " +
            "PRAGMA foreign_keys = OFF; " +
            "UPDATE collocation SET entry_parent = entry_parent + 100; " +
            "UPDATE schema_version SET version = 32;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM pragma_foreign_key_check;"));
    }

    [Fact]
    public void DatabaseCreate_OlderBuild_KeepsTheIdCounter()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO entry (headword, language, added_utc, updated_utc) " +
            "VALUES ('word', 'English', '2026-01-01', '2026-01-01'); " +
            "DELETE FROM entry; " +
            "UPDATE schema_version SET version = 32;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();
        workspace.TWorkspaceScriptRun(
            "INSERT INTO entry (headword, language, added_utc, updated_utc) " +
            "VALUES ('again', 'English', '2026-01-01', '2026-01-01');");

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT entry_id FROM entry;"));
    }

    [Fact]
    public void DatabaseCreate_NewerBuild_CarriesKnownColumnsAcross()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "CREATE TABLE future (future_id INTEGER PRIMARY KEY, note TEXT); " +
            "INSERT INTO entry (headword, language, added_utc, updated_utc) " +
            "VALUES ('word', 'English', '2026-01-01', '2026-01-01'); " +
            $"UPDATE schema_version SET version = {LSchemaMigration.LSchemaMigrationVersion + 5};");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM sqlite_master WHERE name = 'future';"));
    }

    [Fact]
    public void DatabaseCreate_ChildForeignKeys_CarryAnIndex()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        Assert.Equal(1, TSchemaIndexRead(workspace, "collocation", "entry_parent"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "sense", "sense_parent"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "sense_example", "example_ref"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "sense_tag", "tag_ref"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "reference_author", "author_ref"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "tombstone", "revision_ref"));
    }

    private static long TSchemaIndexRead(TWorkspace workspace, string table, string column)
    {
        using SqliteConnection connection = workspace.TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            SELECT COUNT(*) FROM (
                SELECT 1 FROM pragma_index_list('{table}') indexes
                JOIN pragma_index_info(indexes.name) columns
                WHERE columns.seqno = 0 AND columns.name = $column
                LIMIT 1
            );
            """;
        command.Parameters.AddWithValue("$column", column);
        return Convert.ToInt64(command.ExecuteScalar());
    }
}
