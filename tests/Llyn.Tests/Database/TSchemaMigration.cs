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
        Guid realm = TRealmValueRead(workspace);

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
        Assert.Equal(realm, TRealmValueRead(workspace));
        Assert.Single(Directory.GetFiles(workspace.TWorkspaceFolder, "*.v32.db"));
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
    public void DatabaseCreate_NewWorkspace_MintsOneRealm()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM realm;"));
        Assert.Equal(16, workspace.TWorkspaceCountRead("SELECT length(value) FROM realm;"));
        Assert.NotEqual(Guid.Empty, TInterface.TRealmRead(workspace.TWorkspaceDatabase).LRealmValue);
    }

    [Fact]
    public void DatabaseCreate_RunTwice_KeepsTheRealmItMinted()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        Guid first = TRealmValueRead(workspace);
        workspace.TWorkspaceDatabase.TDatabaseCreate();
        Guid second = TRealmValueRead(workspace);

        Assert.Equal(first, second);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM realm;"));
    }

    [Fact]
    public void DatabaseCreate_SecondRealmRow_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        SqliteException error = Assert.Throws<SqliteException>(() => workspace.TWorkspaceScriptRun(
            "INSERT INTO realm (realm_id, value) VALUES (2, randomblob(16));"));
        Assert.Contains("CHECK", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void DatabaseCreate_RowMadeHere_CarriesTheRealmAndItsOwnId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO entry (headword, language, added_utc, updated_utc) " +
            "VALUES ('word', 'English', '2026-01-01', '2026-01-01');");

        Assert.Equal(1, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM entry WHERE origin_id = entry_id " +
            "AND origin_realm = (SELECT value FROM realm WHERE realm_id = 1);"));
    }

    [Fact]
    public void DatabaseCreate_RowFromElsewhere_KeepsItsStamp()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO entry (origin_realm, origin_id, headword, language, added_utc, updated_utc) " +
            "VALUES (randomblob(16), 7, 'word', 'English', '2026-01-01', '2026-01-01');");

        Assert.Equal(1, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM entry WHERE origin_id = 7 " +
            "AND origin_realm <> (SELECT value FROM realm WHERE realm_id = 1);"));
    }

    [Fact]
    public void DatabaseCreate_StampHalfWritten_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        SqliteException error = Assert.Throws<SqliteException>(() => workspace.TWorkspaceScriptRun(
            "INSERT INTO entry (origin_id, headword, language, added_utc, updated_utc) " +
            "VALUES (7, 'word', 'English', '2026-01-01', '2026-01-01');"));
        Assert.Contains("CHECK", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void DatabaseCreate_HistoryBearingRows_NeverReuseAnId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        Assert.Equal(1, TSchemaAutoincrementRead(workspace, "sense"));
        Assert.Equal(1, TSchemaAutoincrementRead(workspace, "collocation"));
    }

    [Fact]
    public void EngineStart_AfterRestart_NeverReusesATemporaryId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        long first;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            first = engine.TEngineDraftStart("test", null).LDraftId;
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();
        long second = reopened.TEngineDraftStart("test", null).LDraftId;

        Assert.True(first < 0);
        Assert.True(second < first);
        Assert.Equal(
            reopened.TEngineStateRead().LWorkspaceStateFloor,
            workspace.TWorkspaceCountRead("SELECT identity_floor FROM workspace;"));
    }

    private static long TSchemaAutoincrementRead(TWorkspace workspace, string table)
    {
        using SqliteConnection connection = workspace.TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "SELECT COUNT(*) FROM sqlite_master WHERE name = $table AND sql LIKE '%AUTOINCREMENT%';";
        command.Parameters.AddWithValue("$table", table);
        return Convert.ToInt64(command.ExecuteScalar());
    }

    private static Guid TRealmValueRead(TWorkspace workspace)
    {
        return TInterface.TRealmRead(workspace.TWorkspaceDatabase).LRealmValue;
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
