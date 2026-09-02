using Llyn.Infrastructure;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Llyn.Database.Tests;

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
    public void ADatabaseAtVersionThirteenGainsTheAudioTableWithoutLosingItsRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );
            INSERT INTO schema_version (id, version) VALUES (1, 13);

            CREATE TABLE entry (
                id TEXT NOT NULL PRIMARY KEY,
                headword TEXT NOT NULL,
                language TEXT NOT NULL,
                proficiency TEXT,
                frequency TEXT,
                added_utc TEXT,
                updated_utc TEXT
            );
            INSERT INTO entry (id, headword, language) VALUES ('kept', 'word', 'English');
            """);

        workspace.TWorkspaceDatabase.LDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'pronunciation_audio';"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry WHERE id = 'kept';"));
    }

    [Fact]
    public void ADatabaseAtVersionSixteenGainsTheSituationSourceColumnWithoutLosingItsRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );
            INSERT INTO schema_version (id, version) VALUES (1, 16);

            CREATE TABLE situation (
                id TEXT NOT NULL PRIMARY KEY,
                title TEXT NOT NULL,
                description TEXT,
                kind TEXT
            );
            INSERT INTO situation (id, title) VALUES ('kept', 'in court');
            """);

        workspace.TWorkspaceDatabase.LDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('situation') WHERE name = 'source_id';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_foreign_key_list('situation') WHERE \"table\" = 'source';"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation WHERE id = 'kept';"));
    }

    [Fact]
    public void DuplicatePositionsAreRenumberedBeforeTheUniqueIndexIsBuilt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

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
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );
            INSERT INTO schema_version (id, version) VALUES (1, 12);

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
            INSERT INTO collocation (id, entry_id, position, expression)
            VALUES ('c1', 'e1', 0, 'in a word');
            """);

        workspace.TWorkspaceDatabase.LDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('collocation') WHERE name = 'meaning';"));

        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM collocation WHERE id = 'c1' AND expression = 'in a word' "
                + "AND meaning IS NULL;"));
    }

    [Fact]
    public void AVersionFourteenDatabaseGainsBothTitleColumnsWithoutLosingARow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );
            INSERT INTO schema_version (id, version) VALUES (1, 14);

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

            CREATE TABLE sense (
                id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL,
                parent_id TEXT,
                position INTEGER NOT NULL,
                gloss TEXT,
                definition_language TEXT,
                definition TEXT,
                labels TEXT NOT NULL,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );
            INSERT INTO sense (id, entry_id, position, labels) VALUES ('s1', 'e1', 0, '');

            CREATE TABLE collocation (
                id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                expression TEXT,
                meaning TEXT,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );
            INSERT INTO collocation (id, entry_id, position, expression)
            VALUES ('c1', 'e1', 0, 'in a word');
            """);

        workspace.TWorkspaceDatabase.LDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));

        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('sense') WHERE name = 'title';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('collocation') WHERE name = 'title';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense WHERE id = 's1' AND title IS NULL;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM collocation WHERE id = 'c1' AND expression = 'in a word' "
                + "AND title IS NULL;"));
    }

    [Fact]
    public void AVersionFifteenDatabaseGainsTheCustomPartOfSpeechWithoutLosingAnAssignment()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        workspace.TWorkspaceScriptRun(
            """
            INSERT INTO entry (id, headword, language) VALUES ('e1', 'word', 'English');

            DROP TABLE part_of_speech;

            CREATE TABLE part_of_speech (
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                value_id TEXT NOT NULL,
                PRIMARY KEY (entry_id, position),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            INSERT INTO part_of_speech (entry_id, position, value_id) VALUES ('e1', 0, 'noun');
            UPDATE schema_version SET version = 15;
            """);

        workspace.TWorkspaceDatabase.LDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));

        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('part_of_speech') WHERE name = 'custom_name';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM part_of_speech "
                + "WHERE entry_id = 'e1' AND value_id = 'noun' AND custom_name IS NULL;"));

        workspace.TWorkspaceScriptRun(
            """
            INSERT INTO entry (id, headword, language) VALUES ('e2', 'other', 'English');
            INSERT INTO part_of_speech (entry_id, position, value_id, custom_name)
            VALUES ('e2', 0, NULL, 'Verb, ergative');
            """);
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM part_of_speech WHERE custom_name = 'Verb, ergative';"));
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

        Assert.Equal(1, TSchemaIndexRead(workspace, "collocation", "entry_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "relation", "sense_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "sense", "parent_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "example_translation", "example_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "entry_example", "example_id"));
        Assert.Equal(1, TSchemaIndexRead(workspace, "sense_tag", "tag_id"));
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
