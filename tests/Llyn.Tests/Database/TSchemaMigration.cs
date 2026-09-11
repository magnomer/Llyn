using Llyn.Infrastructure;
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
    public void DatabaseCreate_OlderBuild_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun("UPDATE schema_version SET version = 32;");

        InvalidOperationException error =
            Assert.Throws<InvalidOperationException>(workspace.TWorkspaceDatabase.LDatabaseCreate);
        Assert.Contains("cannot upgrade", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void DatabaseCreate_NewWorkspace_MintsOneOwnRealm()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM realm;"));
        Assert.Equal(
            LSchemaRealm.LSchemaRealmOwn,
            workspace.TWorkspaceCountRead("SELECT id FROM realm;"));
    }

    [Fact]
    public void DatabaseCreate_RunTwice_KeepsTheOwnRealmItMinted()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        byte[] first = TRealmValueRead(workspace);
        workspace.TWorkspaceDatabase.TDatabaseCreate();
        byte[] second = TRealmValueRead(workspace);

        Assert.Equal(first, second);
    }

    private static byte[] TRealmValueRead(TWorkspace workspace)
    {
        using SqliteConnection connection = workspace.TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT value FROM realm WHERE id = $id;";
        command.Parameters.AddWithValue("$id", LSchemaRealm.LSchemaRealmOwn);
        return (byte[])command.ExecuteScalar()!;
    }

    [Fact]
    public void DatabaseCreate_NewerBuild_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            $"UPDATE schema_version SET version = {LSchemaMigration.LSchemaMigrationVersion + 5};");

        InvalidOperationException error =
            Assert.Throws<InvalidOperationException>(workspace.TWorkspaceDatabase.LDatabaseCreate);
        Assert.Contains("newer than the version", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void DatabaseCreate_ChildForeignKeys_CarryAnIndex()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        Assert.Equal(1, TSchemaIndexRead(workspace, "collocation", "entry_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "relation", "sense_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "sense", "parent_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "sense_example", "example_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "sense_tag", "text"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "source_author", "author_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "tombstone", "revision_id"));
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
