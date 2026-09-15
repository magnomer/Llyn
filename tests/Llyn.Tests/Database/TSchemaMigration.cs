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
    public void DatabaseCreate_OlderBuildHoldingOneNamePerLanguage_KeepsOneRowAndEveryMark()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "DROP INDEX register_name; " +
            "INSERT INTO entry (entry_id, headword, language, added_utc, updated_utc) " +
            "VALUES (1, 'word', 'English', '2026-01-01', '2026-01-01'), " +
            "       (2, 'palabra', 'Spanish', '2026-01-01', '2026-01-01'); " +
            "INSERT INTO sense (sense_id, entry_parent, position) VALUES (1, 1, 0), (2, 2, 0); " +
            "INSERT INTO register (register_id, name_state, name) " +
            "VALUES (1, 'specified', 'Formal'), (2, 'specified', 'Polite'), (3, 'specified', 'Formal'); " +
            "INSERT INTO sense_register (sense_parent, register_ref, position) " +
            "VALUES (1, 1, 0), (2, 3, 0), (2, 2, 1), (2, 1, 2); " +
            "UPDATE schema_version SET version = 32;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM register WHERE name = 'Formal';"));
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            "SELECT register_ref FROM sense_register WHERE sense_parent = 1;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM sense_register WHERE sense_parent = 2;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM sense_register WHERE sense_parent = 2 AND register_ref = 1;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            "SELECT MAX(position) FROM sense_register WHERE sense_parent = 2;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            "SELECT MIN(position) FROM sense_register WHERE sense_parent = 2;"));
    }

    [Fact]
    public void DatabaseCreate_OlderBuildBeforeTheNote_DropsEveryReflexRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO entry (entry_id, headword, language, added_utc, updated_utc) " +
            "VALUES (1, '弄', 'Classical Chinese', '2026-01-01', '2026-01-01'); " +
            "INSERT INTO reflex (entry_parent, position, language, kind, text, main) " +
            "VALUES (1, 0, 'Mandarin', 'nòng', '[nʊŋ⁵¹]', 0); " +
            $"UPDATE schema_version SET version = {LSchemaReflex.LSchemaReflexNoted - 1};");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reflex;"));
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
    public void DatabaseCreate_SituationMediaAbsent_RebuildsWithSituationsIntact()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO situation (title_state, title) VALUES ('specified', 'in court'); " +
            "INSERT INTO image (location_state, location) VALUES ('specified', 'court.png'); " +
            "DROP TABLE situation_image; " +
            "DROP TABLE situation_video; " +
            "UPDATE schema_version SET version = 47;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation WHERE title = 'in court';"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM image;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation_image;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation_video;"));
        Assert.Single(Directory.GetFiles(workspace.TWorkspaceFolder, "*.v47.db"));
    }

    [Fact]
    public void DatabaseCreate_UnknownSourceWording_LinksTheUnknownSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO example (language, text_state, text, reference_state) " +
            "VALUES ('English', 'specified', 'he said a word', 'unknown'); " +
            "INSERT INTO example (language, text_state, text) " +
            "VALUES ('English', 'specified', 'not a word was spoken'); " +
            "UPDATE schema_version SET version = 48;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reference WHERE title = 'Unknown';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE reference_state = 'specified' " +
                "AND reference_ref = (SELECT reference_id FROM reference WHERE title = 'Unknown');"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example WHERE reference_state = 'unspecified';"));
        Assert.Equal(
            0,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example WHERE reference_state = 'unknown';"));
    }

    [Fact]
    public void DatabaseCreate_UnknownSourceDeleted_MintsItForOldWording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "DELETE FROM reference; " +
            "INSERT INTO example (language, text_state, text, reference_state) " +
            "VALUES ('English', 'specified', 'he said a word', 'unknown'); " +
            "UPDATE schema_version SET version = 48;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reference;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reference WHERE title = 'Unknown';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE reference_ref = (SELECT reference_id FROM reference);"));
    }

    [Fact]
    public void DatabaseCreate_NewWorkspace_SeedsTheUnknownSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reference;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM reference WHERE title_state = 'specified' AND title = 'Unknown';"));
    }

    [Fact]
    public void DatabaseCreate_UnknownSourceDeleted_SeedsNothingAgain()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        workspace.TWorkspaceScriptRun("DELETE FROM reference;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reference;"));
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
