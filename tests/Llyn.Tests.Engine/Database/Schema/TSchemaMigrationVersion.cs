using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TSchemaMigrationVersion
{
    [Fact]
    public void DatabaseCreate_BuildHoldingEtymology_DropsTheOldTablesAndKeepsTheEntries()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "CREATE TABLE etymology (entry_parent INTEGER PRIMARY KEY, text TEXT); " +
            "CREATE TABLE etymology_mention (entry_parent INTEGER, entry_ref INTEGER); " +
            "CREATE TABLE etymon (entry_parent INTEGER, entry_ref INTEGER, position INTEGER); " +
            "INSERT INTO entry (headword, language, added_utc, updated_utc) " +
            "VALUES ('run', 'English', '2026-01-01', '2026-01-01'); " +
            "UPDATE schema_version SET version = 81;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry WHERE headword = 'run';"));
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM sqlite_master WHERE name IN ('etymology', 'etymology_mention', 'etymon');"));
    }

    [Fact]
    public void DatabaseCreate_CollocationSharingMeaningId_RenumbersItAndItsLinks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO entry (entry_id, headword, language, added_utc, updated_utc) " +
            "VALUES (1, 'word', 'English', '2026-01-01', '2026-01-01'), " +
            "       (2, 'Wort', 'German', '2026-01-01', '2026-01-01'); " +
            "INSERT INTO sense (sense_id, entry_parent, position) VALUES (1, 1, 0); " +
            "INSERT INTO collocation (collocation_id, entry_parent, position) VALUES (1, 1, 0), (5, 1, 1); " +
            "INSERT INTO collocation_translation (collocation_parent, entry_ref, position) VALUES (1, 2, 0); " +
            "INSERT INTO revision (revision_id, created_utc) VALUES (1, '2026-01-01'); " +
            "INSERT INTO revision_change (revision_parent, position, target_ref, target_type, kind) " +
            "VALUES (1, 0, 1, 'collocation', 'update'), (1, 1, 1, 'sense', 'update'); " +
            "UPDATE schema_version SET version = 74;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            [5L, 6L],
            workspace.TWorkspaceColumnRead("SELECT collocation_id FROM collocation ORDER BY collocation_id;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT sense_id FROM sense;"));
        Assert.Equal(6, workspace.TWorkspaceCountRead("SELECT collocation_parent FROM collocation_translation;"));
        Assert.Equal(6, workspace.TWorkspaceCountRead(
            "SELECT target_ref FROM revision_change WHERE target_type = 'collocation';"));
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            "SELECT target_ref FROM revision_change WHERE target_type = 'sense';"));
        Assert.Equal(6, workspace.TWorkspaceCountRead("SELECT seq FROM sqlite_sequence WHERE name = 'collocation';"));
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
    public void DatabaseCreate_OlderBuildWithEntryFrequency_MovesItIntoTheFrequencyTable()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "ALTER TABLE entry ADD COLUMN frequency TEXT; " +
            "INSERT INTO entry (entry_id, headword, language, frequency, added_utc, updated_utc) " +
            "VALUES (1, 'word', 'English', 'Datamuse|12.5|per million', '2026-01-01', '2026-01-02'), " +
            "       (2, 'bare', 'English', 'S1', '2026-01-01', '2026-01-01'), " +
            "       (3, 'none', 'English', NULL, '2026-01-01', '2026-01-01'); " +
            "UPDATE schema_version SET version = 66;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(0, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM pragma_table_info('entry') WHERE name = 'frequency';"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM frequency;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM frequency WHERE entry_parent = 1 AND source = 'Datamuse' " +
            "AND raw = '12.5|per million' AND band IS NULL AND fetched_utc = '2026-01-02';"));
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
    public void DatabaseCreate_RetiredUnits_ClearsThemAndKeepsTheEntries()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "DROP TABLE entry; " +
            "CREATE TABLE entry (entry_id INTEGER PRIMARY KEY AUTOINCREMENT, headword TEXT NOT NULL, " +
            "language TEXT NOT NULL, added_utc TEXT, updated_utc TEXT, " +
            "unit INTEGER NOT NULL DEFAULT 0 CHECK (unit BETWEEN 0 AND 4)); " +
            "INSERT INTO entry (entry_id, headword, language, unit) " +
            "VALUES (1, 'run', 'English', 1), (2, 'the', 'English', 2), " +
            "       (3, 'un', 'English', 3), (4, 'cat', 'English', 4); " +
            "UPDATE schema_version SET version = 79;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            [0L, 0L, 3L, 4L],
            workspace.TWorkspaceColumnRead("SELECT unit FROM entry ORDER BY entry_id;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry WHERE freedom <> 0;"));
    }

    [Fact]
    public void DatabaseCreate_VersionEightyOne_AddsCollocationSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "DROP TABLE collocation_sense; " +
            "INSERT INTO entry (entry_id, headword, language, added_utc, updated_utc) " +
            "VALUES (1, 'set', 'English', '2026-01-01', '2026-01-01'); " +
            "INSERT INTO sense (sense_id, entry_parent, position) VALUES (1, 1, 0); " +
            "INSERT INTO collocation (collocation_id, entry_parent, position) VALUES (2, 1, 0); " +
            "UPDATE schema_version SET version = 80;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation WHERE collocation_id = 2;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_sense;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'index' AND name = 'collocation_sense_member';"));
    }

    [Fact]
    public void DatabaseCreate_MissingSense_KeepsCueOrphaned()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO stencil (stencil_id) VALUES (1); " +
            "INSERT INTO deck (deck_id, position, name, grain, stencil_ref, deck_code, added_utc, updated_utc) " +
            "VALUES (1, 0, 'Verbs', 2, 1, 'Verbs', '2026-01-01', '2026-01-01'); " +
            "PRAGMA foreign_keys = OFF; " +
            "INSERT INTO cue (cue_id, deck_parent, position, grain, sense_ref, added_utc) " +
            "VALUES (1, 1, 0, 2, 9, '2026-01-01'); " +
            "UPDATE schema_version SET version = 82;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM cue WHERE sense_ref IS NULL;"));
    }

    [Fact]
    public void DatabaseCreate_MissingDeck_SweepsCues()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO stencil (stencil_id) VALUES (1); " +
            "INSERT INTO deck (deck_id, position, name, grain, stencil_ref, deck_code, added_utc, updated_utc) " +
            "VALUES (1, 0, 'Verbs', 1, 1, 'Verbs', '2026-01-01', '2026-01-01'); " +
            "PRAGMA foreign_keys = OFF; " +
            "INSERT INTO cue (cue_id, deck_parent, position, grain, added_utc) " +
            "VALUES (1, 1, 0, 0, '2026-01-01'), (2, 9, 0, 0, '2026-01-01'); " +
            "INSERT INTO cue_cell (cue_parent, column_kind, column_index, value) " +
            "VALUES (1, 1, 1, 'kept'), (2, 1, 1, 'swept'); " +
            "UPDATE schema_version SET version = 82;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal([1L], workspace.TWorkspaceColumnRead("SELECT cue_id FROM cue ORDER BY cue_id;"));
        Assert.Equal([1L], workspace.TWorkspaceColumnRead("SELECT cue_parent FROM cue_cell ORDER BY cue_parent;"));
    }

    [Fact]
    public void DatabaseCreate_SharedCollocationId_FollowsRenumber()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO entry (entry_id, headword, language, added_utc, updated_utc) " +
            "VALUES (1, 'word', 'English', '2026-01-01', '2026-01-01'); " +
            "INSERT INTO sense (sense_id, entry_parent, position) VALUES (1, 1, 0); " +
            "INSERT INTO collocation (collocation_id, entry_parent, position) VALUES (1, 1, 0), (5, 1, 1); " +
            "INSERT INTO stencil (stencil_id) VALUES (1); " +
            "INSERT INTO deck (deck_id, position, name, grain, stencil_ref, deck_code, added_utc, updated_utc) " +
            "VALUES (1, 0, 'Phrases', 3, 1, 'Phrases', '2026-01-01', '2026-01-01'); " +
            "INSERT INTO cue (cue_id, deck_parent, position, grain, collocation_ref, added_utc) " +
            "VALUES (1, 1, 0, 3, 1, '2026-01-01'); " +
            "UPDATE schema_version SET version = 82;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            [5L, 6L],
            workspace.TWorkspaceColumnRead("SELECT collocation_id FROM collocation ORDER BY collocation_id;"));
        Assert.Equal(6, workspace.TWorkspaceCountRead("SELECT collocation_ref FROM cue;"));
    }

    [Fact]
    public void DatabaseCreate_SweptSenseParent_KeepsCueOrphaned()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO stencil (stencil_id) VALUES (1); " +
            "INSERT INTO deck (deck_id, position, name, grain, stencil_ref, deck_code, added_utc, updated_utc) " +
            "VALUES (1, 0, 'Verbs', 2, 1, 'Verbs', '2026-01-01', '2026-01-01'); " +
            "PRAGMA foreign_keys = OFF; " +
            "INSERT INTO sense (sense_id, entry_parent, position) VALUES (1, 9, 0); " +
            "INSERT INTO cue (cue_id, deck_parent, position, grain, sense_ref, added_utc) " +
            "VALUES (1, 1, 0, 2, 1, '2026-01-01'); " +
            "UPDATE schema_version SET version = 82;");

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM cue WHERE sense_ref IS NULL;"));
    }
}
