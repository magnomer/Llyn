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
    public void DatabaseCreate_OlderDatabase_MigratesNotRestamps()
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

        workspace.TWorkspaceDatabase.TDatabaseCreate();

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
    public void DatabaseCreate_VersionThirteen_AddsAudioTable()
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

        workspace.TWorkspaceDatabase.TDatabaseCreate();

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
    public void DatabaseCreate_VersionSixteen_DropsSituationSource()
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
                kind TEXT,
                source_state TEXT NOT NULL DEFAULT 'unspecified',
                source_id TEXT
            );
            INSERT INTO situation (id, title) VALUES ('kept', 'in court');
            """);

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(
            0,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('situation') WHERE name = 'source_id';"));
        Assert.Equal(
            0,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('situation') WHERE name = 'source_state';"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation WHERE id = 'kept';"));
    }

    [Fact]
    public void DatabaseCreate_DuplicatePositions_RenumbersBeforeIndex()
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

        workspace.TWorkspaceDatabase.TDatabaseCreate();

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
    public void DatabaseCreate_VersionEighteen_TradesTagIdForText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );
            INSERT INTO schema_version (id, version) VALUES (1, 18);

            CREATE TABLE tag (
                id TEXT NOT NULL PRIMARY KEY,
                text_state TEXT NOT NULL DEFAULT 'unspecified',
                text TEXT,
                CHECK (text_state = 'specified' OR text IS NULL)
            );
            INSERT INTO tag (id, text_state, text) VALUES ('t1', 'specified', 'formal');
            INSERT INTO tag (id, text_state, text) VALUES ('t2', 'specified', 'formal');
            INSERT INTO tag (id, text_state, text) VALUES ('t3', 'unknown', NULL);

            CREATE TABLE sense_tag (
                sense_id TEXT NOT NULL,
                tag_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, tag_id)
            );
            INSERT INTO sense_tag (sense_id, tag_id, position) VALUES ('s1', 't3', 0);
            INSERT INTO sense_tag (sense_id, tag_id, position) VALUES ('s1', 't1', 1);
            INSERT INTO sense_tag (sense_id, tag_id, position) VALUES ('s1', 't2', 2);

            CREATE TABLE collocation_tag (
                collocation_id TEXT NOT NULL,
                tag_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, tag_id)
            );
            INSERT INTO collocation_tag (collocation_id, tag_id, position) VALUES ('c1', 't1', 0);
            """);

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(
            0,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'tag';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sense_tag WHERE text = 'formal' AND position = 0;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_tag;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM collocation_tag WHERE text = 'formal' AND position = 0;"));
    }

    [Fact]
    public void DatabaseCreate_VersionTwenty_MovesTranslationToExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );
            INSERT INTO schema_version (id, version) VALUES (1, 20);

            CREATE TABLE example (
                id TEXT NOT NULL PRIMARY KEY,
                language TEXT NOT NULL,
                text_state TEXT NOT NULL DEFAULT 'unspecified',
                text TEXT,
                local TEXT,
                source_state TEXT NOT NULL DEFAULT 'unspecified',
                source_id TEXT
            );

            CREATE TABLE example_rendition (
                id TEXT NOT NULL PRIMARY KEY,
                example_id TEXT NOT NULL,
                language TEXT NOT NULL,
                text TEXT NOT NULL,
                position INTEGER NOT NULL
            );

            INSERT INTO example (id, language, text_state, text, local)
                VALUES ('e1', 'fr', 'specified', 'casser', NULL);
            INSERT INTO example (id, language, text_state, text, local)
                VALUES ('e2', 'fr', 'specified', 'briser', 'written by hand');

            INSERT INTO example_rendition (id, example_id, language, text, position)
                VALUES ('r1', 'e1', 'ko', '부수다', 0);
            INSERT INTO example_rendition (id, example_id, language, text, position)
                VALUES ('r2', 'e1', 'en', 'to break', 1);
            """);

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(
            0,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'example_rendition';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                """
                SELECT COUNT(*) FROM example
                WHERE id = 'e1' AND translation = '부수다' AND translation_state = 'specified';
                """));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                """
                SELECT COUNT(*) FROM example
                WHERE id = 'e2' AND translation = 'written by hand'
                    AND translation_state = 'specified';
                """));
    }

    [Fact]
    public void DatabaseCreate_VersionNineteen_AddsTranslationTables()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );
            INSERT INTO schema_version (id, version) VALUES (1, 19);
            """);

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'sense_translation';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'collocation_translation';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'index' AND name = 'sense_translation_position';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'index' AND name = 'collocation_translation_position';"));
    }

    [Fact]
    public void DatabaseCreate_VersionTwentyThree_RebuildsSenseExampleWithItsFrame()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );
            INSERT INTO schema_version (id, version) VALUES (1, 23);

            CREATE TABLE sense_example (
                sense_id TEXT NOT NULL,
                example_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, example_id)
            );
            INSERT INTO sense_example (sense_id, example_id, position) VALUES ('s1', 'x1', 0);
            INSERT INTO sense_example (sense_id, example_id, position) VALUES ('s1', 'x2', 1);
            """);

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM pragma_table_info('sense_example') WHERE name = 'id';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('sense_example') WHERE name = 'particle';"));
        Assert.Equal(
            2,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example WHERE length(id) > 0;"));
        Assert.Equal(
            2,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sense_example WHERE particle_state = 'unspecified' AND revision_id IS NULL;"));
        Assert.Equal(
            0,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'sense_example_carried';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'video';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'sense_video';"));
    }

    [Fact]
    public void DatabaseCreate_VersionTwentyFour_RebuildsCollocationExampleWithItsFrame()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        workspace.TWorkspaceScriptRun(
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );
            INSERT INTO schema_version (id, version) VALUES (1, 24);

            CREATE TABLE collocation_example (
                collocation_id TEXT NOT NULL,
                example_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, example_id)
            );
            INSERT INTO collocation_example (collocation_id, example_id, position) VALUES ('c1', 'x1', 0);
            INSERT INTO collocation_example (collocation_id, example_id, position) VALUES ('c1', 'x2', 1);
            """);

        workspace.TWorkspaceDatabase.TDatabaseCreate();

        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('collocation_example') WHERE name = 'id';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM pragma_table_info('collocation_example') WHERE name = 'particle';"));
        Assert.Equal(
            2,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_example WHERE length(id) > 0;"));
        Assert.Equal(
            2,
            workspace.TWorkspaceCountRead(
                """
                SELECT COUNT(*) FROM collocation_example
                WHERE particle_state = 'unspecified' AND revision_id IS NULL;
                """));
        Assert.Equal(
            2,
            workspace.TWorkspaceCountRead(
                """
                SELECT COUNT(*) FROM collocation_example
                WHERE (example_id = 'x1' AND position = 0) OR (example_id = 'x2' AND position = 1);
                """));
        Assert.Equal(
            0,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'collocation_example_carried';"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                """
                SELECT COUNT(*) FROM sqlite_master
                WHERE type = 'index' AND name = 'collocation_example_position';
                """));
    }

    [Fact]
    public void DatabaseCreate_VersionTwelve_AddsCollocationMeaning()
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

        workspace.TWorkspaceDatabase.TDatabaseCreate();

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
    public void DatabaseCreate_VersionFourteen_AddsTitleColumns()
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

        workspace.TWorkspaceDatabase.TDatabaseCreate();

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
    public void DatabaseCreate_VersionFifteen_AddsCustomSpeech()
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

        workspace.TWorkspaceDatabase.TDatabaseCreate();

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
        Assert.Equal(1, TSchemaIndexRead(workspace, "entry_example", "example_id"));
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
