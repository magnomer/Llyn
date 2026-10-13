using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TSchemaMigrationReflex
{
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
    public void DatabaseCreate_OlderBuildBeforeTheRegion_KeepsRowsAsRomanizations()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "ALTER TABLE reflex DROP COLUMN region; ALTER TABLE reflex DROP COLUMN meaning; " +
            "ALTER TABLE reflex DROP COLUMN owned; ALTER TABLE reflex DROP COLUMN romanization; " +
            "INSERT INTO entry (entry_id, headword, language, added_utc, updated_utc) " +
            "VALUES (1, '弄', 'Classical Chinese', '2026-01-01', '2026-01-01'); " +
            "INSERT INTO reflex (entry_parent, position, language, kind, text, main, note) " +
            "VALUES (1, 0, 'Mandarin', '', '[nʊŋ⁵¹]', 0, 'nòng'); " +
            "UPDATE schema_version SET version = 62;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(1, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM reflex WHERE region = '' AND note = '' AND meaning = '' " +
            "AND owned = 0 AND romanization = 'nòng';"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reflex WHERE text = '[nʊŋ⁵¹]';"));
    }

    [Fact]
    public void DatabaseCreate_Version71_PartsOldNoteAndRemark()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "ALTER TABLE reflex DROP COLUMN meaning; " +
            "ALTER TABLE reflex DROP COLUMN owned; " +
            "ALTER TABLE reflex DROP COLUMN romanization; " +
            "ALTER TABLE reflex ADD COLUMN remark TEXT NOT NULL DEFAULT ''; " +
            "INSERT INTO entry (entry_id, headword, language, added_utc, updated_utc) " +
            "VALUES (1, '惡', 'Classical Chinese', '2026-01-01', '2026-01-01'); " +
            "INSERT INTO reflex (entry_parent, position, language, kind, text, main, note, remark) " +
            "VALUES (1, 0, 'Southern Min', '', 'ɔk³²', 1, 'ok4', 'literary'); " +
            "UPDATE schema_version SET version = 71;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(1, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM reflex WHERE romanization = 'ok4' AND meaning = '' " +
            "AND owned = 0 AND note = 'literary';"));
    }
}
