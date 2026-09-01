using Llyn.Infrastructure;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Llyn.Database.Tests;

/// <summary>
/// Covers the schema runner and the migration that brings an older database up to the current version:
/// creating twice changes nothing, the version is recorded and read back, a database written by a newer
/// build is refused, and a database in the shape an earlier build left behind is corrected rather than
/// merely restamped.
/// </summary>
public sealed class TSchemaMigration
{
    [Fact]
    public void CreatingTheSchemaTwiceLeavesOneVersionRowAtTheCurrentVersion()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceDatabase.LDatabaseCreate();
        workspace.TWorkspaceDatabase.LDatabaseCreate();

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM schema_version;"));
        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
    }

    [Fact]
    public void ForeignKeysAreEnforcedOnEveryConnection()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        Assert.Equal(1, workspace.TWorkspaceCountRead("PRAGMA foreign_keys;"));
    }

    [Fact]
    public void AnOlderDatabaseIsMigratedRatherThanRestamped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        // The shape an earlier build left behind: a version table that permits several rows, and an
        // example table whose source_id carries no foreign key because the source table did not exist
        // when the column was declared.
        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (version INTEGER NOT NULL);
            INSERT INTO schema_version (version) VALUES (9);
            INSERT INTO schema_version (version) VALUES (11);

            CREATE TABLE example (
                id TEXT NOT NULL PRIMARY KEY,
                language TEXT NOT NULL,
                text TEXT NOT NULL,
                local TEXT,
                source_id TEXT
            );
            INSERT INTO example (id, language, text, local, source_id)
            VALUES ('kept', 'en', 'a sentence that must survive', NULL, NULL);
            """);

        workspace.TWorkspaceDatabase.LDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM schema_version;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM pragma_foreign_key_list('example');"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example WHERE id = 'kept';"));
    }

    [Fact]
    public void DuplicatePositionsAreRenumberedBeforeTheUniqueIndexIsBuilt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        // Two collocations sharing one position under the same entry — possible before the unique index
        // existed, and fatal to creating it now.
        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (version INTEGER NOT NULL);
            INSERT INTO schema_version (version) VALUES (11);

            CREATE TABLE entry (
                id TEXT NOT NULL PRIMARY KEY,
                headword TEXT NOT NULL,
                language TEXT NOT NULL,
                proficiency TEXT,
                frequency TEXT,
                added_utc TEXT,
                updated_utc TEXT
            );
            INSERT INTO entry (id, headword, language) VALUES ('e1', 'word', 'en');

            CREATE TABLE collocation (
                id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                expression TEXT,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );
            INSERT INTO collocation (id, entry_id, position, expression) VALUES ('c1', 'e1', 0, 'one');
            INSERT INTO collocation (id, entry_id, position, expression) VALUES ('c2', 'e1', 0, 'two');
            """);

        workspace.TWorkspaceDatabase.LDatabaseCreate();

        Assert.Equal(
            2,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(DISTINCT position) FROM collocation WHERE entry_id = 'e1';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'index' AND name = 'collocation_position';"));
    }

    [Fact]
    public void AVersionTwelveDatabaseGainsTheCollocationMeaningWithoutLosingARow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        // A database in the version-12 shape: the collocation table as it stood, with a row in it.
        // CREATE TABLE IF NOT EXISTS never reaches it, so only the migration step can add the column.
        workspace.TWorkspaceScriptRun(
            """
            INSERT INTO entry (id, headword, language) VALUES ('e1', 'word', 'en');
            INSERT INTO collocation (id, entry_id, position, expression)
            VALUES ('c1', 'e1', 0, 'in a word');

            ALTER TABLE collocation DROP COLUMN meaning;
            UPDATE schema_version SET version = 12;
            """);

        workspace.TWorkspaceDatabase.LDatabaseCreate();

        Assert.Equal(13, workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('collocation') WHERE name = 'meaning';"));

        // The row that was there before the column existed survives, expression intact, meaning empty.
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM collocation WHERE id = 'c1' AND expression = 'in a word' "
                + "AND meaning IS NULL;"));
    }

    [Fact]
    public void ADatabaseFromANewerBuildIsRefused()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            $"UPDATE schema_version SET version = {LSchemaMigration.LSchemaMigrationVersion + 5};");

        InvalidOperationException error =
            Assert.Throws<InvalidOperationException>(workspace.TWorkspaceDatabase.LDatabaseCreate);
        Assert.Contains("newer than the version", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void EveryChildForeignKeyColumnCarriesAnIndex()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        // A child column with no index turns each parent delete into a full scan of the child table, so
        // the index set is part of the schema rather than an optimization applied later.
        Assert.Equal(1, TSchemaIndexRead(workspace, "collocation", "entry_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "relation", "sense_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "sense", "parent_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "example_translation", "example_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "entry_example", "example_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "sense_tag", "tag_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "source_author", "author_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "tombstone", "revision_id"));
    }

    // Whether some index on the table has the named column first — which is what the foreign-key check
    // and the cascade actually use.
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
