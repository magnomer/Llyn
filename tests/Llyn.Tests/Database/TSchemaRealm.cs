using Llyn.ShellEngine;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Llyn.Tests;

public sealed class TSchemaRealm
{
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

        Guid first = TInterface.TRealmValueRead(workspace);
        workspace.TWorkspaceDatabase.TDatabaseCreate();
        Guid second = TInterface.TRealmValueRead(workspace);

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
}
