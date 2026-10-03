using Xunit;

namespace Llyn.Tests;

public sealed class TSchemaMigrationReference
{
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
}
