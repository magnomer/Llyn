using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaMigration
{
    public const long LSchemaMigrationVersion = 25;

    public static void LSchemaMigrationApply(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (!LSchemaVersionExist(connection))
        {
            LSchemaVersionCreate(connection);
            return;
        }

        long stored = LSchemaVersionRead(connection);
        if (stored > LSchemaMigrationVersion)
        {
            throw new InvalidOperationException(
                $"The workspace database is at schema version {stored}, which is newer than the version " +
                $"{LSchemaMigrationVersion} this build understands. Update Llyn before opening it.");
        }

        if (stored >= LSchemaMigrationVersion)
        {
            return;
        }

        if (stored < 20)
        {
            LSchemaTranslationCreate(connection);
        }

        if (stored < 12)
        {
            LSchemaExampleNormalize(connection);
            LSchemaPositionNormalize(connection, "relation", "sense_id");
            LSchemaPositionNormalize(connection, "collocation", "entry_id");
            LSchemaPositionNormalize(connection, "collocation_synonym", "collocation_id");
            LSchemaVersionNormalize(connection);
        }

        if (stored < 13)
        {
            LSchemaCollocationNormalize(connection);
        }

        if (stored < 14)
        {
            LSchemaAudioNormalize(connection);
        }

        if (stored < 15)
        {
            LSchemaTitleNormalize(connection, "sense");
            LSchemaTitleNormalize(connection, "collocation");
        }

        if (stored < 16)
        {
            LSchemaSpeechNormalize(connection);
        }

        if (stored < 18)
        {
            LSchemaState.LSchemaStateNormalize(connection);
        }

        if (stored < 19)
        {
            LSchemaTagNormalize(connection);
        }

        if (stored < 21)
        {
            LSchemaTranslationNormalize(connection);
        }

        if (stored < 22)
        {
            LSchemaSituationNormalize(connection);
        }

        if (stored < 23)
        {
            LSchemaFavoriteCreate(connection);
        }

        if (stored < 24)
        {
            LSchemaSentenceNormalize(connection, "sense_example", "sense_id", "sense");
        }

        if (stored < 25)
        {
            LSchemaSentenceNormalize(connection, "collocation_example", "collocation_id", "collocation");
        }

        LSchemaVersionSave(connection);
    }

    private static void LSchemaTranslationNormalize(SqliteConnection connection)
    {
        if (LSchemaColumnFind(connection, "example", "local"))
        {
            LSchemaExampleRebuild(connection, LSchemaCarriedRead(connection));
        }

        using SqliteCommand drop = connection.CreateCommand();
        drop.CommandText =
            """
            DROP TABLE IF EXISTS example_rendition;

            DROP TABLE IF EXISTS example_translation;
            """;
        drop.ExecuteNonQuery();
    }

    private static void LSchemaExampleRebuild(SqliteConnection connection, string carried)
    {
        using (SqliteCommand off = connection.CreateCommand())
        {
            off.CommandText = "PRAGMA foreign_keys = OFF;";
            off.ExecuteNonQuery();
        }

        try
        {
            using SqliteTransaction rebuild = connection.BeginTransaction();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                $"""
                DROP TABLE IF EXISTS example_next;

                CREATE TABLE example_next (
                    id TEXT NOT NULL PRIMARY KEY,
                    language TEXT NOT NULL,
                    text_state TEXT NOT NULL DEFAULT 'unspecified',
                    text TEXT,
                    translation_state TEXT NOT NULL DEFAULT 'unspecified',
                    translation TEXT,
                    source_state TEXT NOT NULL DEFAULT 'unspecified',
                    source_id TEXT,
                    CHECK (text_state = 'specified' OR text IS NULL),
                    CHECK (translation_state = 'specified' OR translation IS NULL),
                    CHECK (source_state = 'specified' OR source_id IS NULL),
                    FOREIGN KEY (source_id) REFERENCES source (id)
                );

                INSERT INTO example_next (
                    id, language, text_state, text, translation_state, translation,
                    source_state, source_id)
                    SELECT
                        example.id,
                        example.language,
                        example.text_state,
                        example.text,
                        CASE WHEN {carried} IS NULL THEN 'unspecified' ELSE 'specified' END,
                        {carried},
                        example.source_state,
                        example.source_id
                    FROM example;

                DROP TABLE example;

                ALTER TABLE example_next RENAME TO example;
                """;
            command.ExecuteNonQuery();
            rebuild.Commit();
        }
        finally
        {
            using SqliteCommand on = connection.CreateCommand();
            on.CommandText = "PRAGMA foreign_keys = ON;";
            on.ExecuteNonQuery();
        }
    }

    private static string LSchemaCarriedRead(SqliteConnection connection)
    {
        const string local = "NULLIF(TRIM(COALESCE(example.local, '')), '')";

        string owned = LSchemaTableFind(connection, "example_rendition")
            ? "example_rendition"
            : LSchemaTableFind(connection, "example_translation")
                ? "example_translation"
                : string.Empty;

        if (owned.Length == 0)
        {
            return local;
        }

        return $"""
            COALESCE({local}, (SELECT NULLIF(TRIM(owned.text), '')
                               FROM {owned} owned
                               WHERE owned.example_id = example.id
                               ORDER BY owned.position LIMIT 1))
            """;
    }

    private static bool LSchemaTableFind(SqliteConnection connection, string table)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = $table;";
        command.Parameters.AddWithValue("$table", table);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    private static bool LSchemaColumnFind(SqliteConnection connection, string table, string column)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"SELECT COUNT(*) FROM pragma_table_info('{table}') WHERE name = $column;";
        command.Parameters.AddWithValue("$column", column);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    private static void LSchemaSentenceNormalize(
        SqliteConnection connection, string table, string column, string owner)
    {
        if (LSchemaColumnFind(connection, table, "id"))
        {
            return;
        }

        using (SqliteCommand off = connection.CreateCommand())
        {
            off.CommandText = "PRAGMA foreign_keys = OFF;";
            off.ExecuteNonQuery();
        }

        try
        {
            LSchemaSentenceRebuild(connection, table, column, owner);
        }
        finally
        {
            using SqliteCommand on = connection.CreateCommand();
            on.CommandText = "PRAGMA foreign_keys = ON;";
            on.ExecuteNonQuery();
        }
    }

    private static void LSchemaSentenceRebuild(
        SqliteConnection connection, string table, string column, string owner)
    {
        using SqliteTransaction rebuild = connection.BeginTransaction();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            ALTER TABLE {table} RENAME TO {table}_carried;

            DROP INDEX IF EXISTS {table}_position;

            DROP INDEX IF EXISTS {table}_member;

            CREATE TABLE {table} (
                id TEXT NOT NULL PRIMARY KEY,
                {column} TEXT NOT NULL,
                example_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                revision_state TEXT NOT NULL DEFAULT 'unspecified',
                revision_id TEXT,
                particle_state TEXT NOT NULL DEFAULT 'unspecified',
                particle TEXT,
                dependence_state TEXT NOT NULL DEFAULT 'unspecified',
                dependence TEXT,
                CHECK (revision_state = 'specified' OR revision_id IS NULL),
                CHECK (particle_state = 'specified' OR particle IS NULL),
                CHECK (dependence_state = 'specified' OR dependence IS NULL),
                FOREIGN KEY ({column}) REFERENCES {owner} (id) ON DELETE CASCADE,
                FOREIGN KEY (example_id) REFERENCES example (id),
                FOREIGN KEY (revision_id) REFERENCES example (id)
            );

            INSERT INTO {table} (id, {column}, example_id, position)
            SELECT lower(hex(randomblob(6))), {column}, example_id, position FROM {table}_carried;

            DROP TABLE {table}_carried;

            CREATE UNIQUE INDEX IF NOT EXISTS {table}_position
                ON {table} ({column}, position);
            """;
        command.ExecuteNonQuery();
        rebuild.Commit();
    }

    private static void LSchemaFavoriteCreate(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS favorite (
                entry_id TEXT NOT NULL PRIMARY KEY,
                marked_utc TEXT NOT NULL,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE INDEX IF NOT EXISTS favorite_marked ON favorite (marked_utc);
            """;
        command.ExecuteNonQuery();
    }

    private static void LSchemaTranslationCreate(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS sense_translation (
                sense_id TEXT NOT NULL,
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, entry_id),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS collocation_translation (
                collocation_id TEXT NOT NULL,
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, entry_id),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_translation_position
                ON sense_translation (sense_id, position);

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_translation_position
                ON collocation_translation (collocation_id, position);
            """;
        command.ExecuteNonQuery();
    }

    private static void LSchemaTagNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                "SELECT COUNT(*) FROM pragma_table_info('sense_tag') WHERE name = 'text';";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using (SqliteCommand off = connection.CreateCommand())
        {
            off.CommandText = "PRAGMA foreign_keys = OFF;";
            off.ExecuteNonQuery();
        }

        try
        {
            using SqliteTransaction rebuild = connection.BeginTransaction();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                DROP TABLE IF EXISTS sense_tag_next;

                CREATE TABLE sense_tag_next (
                    sense_id TEXT NOT NULL,
                    text TEXT NOT NULL,
                    position INTEGER NOT NULL,
                    PRIMARY KEY (sense_id, text),
                    CHECK (length(text) > 0),
                    FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE
                );

                INSERT INTO sense_tag_next (sense_id, text, position)
                    SELECT sense_id, text, position - 1 FROM (
                        SELECT link.sense_id AS sense_id,
                               tag.text AS text,
                               ROW_NUMBER() OVER (
                                   PARTITION BY link.sense_id ORDER BY MIN(link.position)) AS position
                        FROM sense_tag link
                        JOIN tag ON tag.id = link.tag_id
                        WHERE tag.text IS NOT NULL AND length(tag.text) > 0
                        GROUP BY link.sense_id, tag.text
                    );

                DROP TABLE sense_tag;

                ALTER TABLE sense_tag_next RENAME TO sense_tag;

                DROP TABLE IF EXISTS collocation_tag_next;

                CREATE TABLE collocation_tag_next (
                    collocation_id TEXT NOT NULL,
                    text TEXT NOT NULL,
                    position INTEGER NOT NULL,
                    PRIMARY KEY (collocation_id, text),
                    CHECK (length(text) > 0),
                    FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE
                );

                INSERT INTO collocation_tag_next (collocation_id, text, position)
                    SELECT collocation_id, text, position - 1 FROM (
                        SELECT link.collocation_id AS collocation_id,
                               tag.text AS text,
                               ROW_NUMBER() OVER (
                                   PARTITION BY link.collocation_id ORDER BY MIN(link.position)) AS position
                        FROM collocation_tag link
                        JOIN tag ON tag.id = link.tag_id
                        WHERE tag.text IS NOT NULL AND length(tag.text) > 0
                        GROUP BY link.collocation_id, tag.text
                    );

                DROP TABLE collocation_tag;

                ALTER TABLE collocation_tag_next RENAME TO collocation_tag;

                DROP TABLE tag;

                CREATE UNIQUE INDEX IF NOT EXISTS sense_tag_position
                    ON sense_tag (sense_id, position);

                CREATE UNIQUE INDEX IF NOT EXISTS collocation_tag_position
                    ON collocation_tag (collocation_id, position);

                CREATE INDEX IF NOT EXISTS sense_tag_member ON sense_tag (text);

                CREATE INDEX IF NOT EXISTS collocation_tag_member ON collocation_tag (text);
                """;
            command.ExecuteNonQuery();
            rebuild.Commit();
        }
        finally
        {
            using SqliteCommand on = connection.CreateCommand();
            on.CommandText = "PRAGMA foreign_keys = ON;";
            on.ExecuteNonQuery();
        }
    }

    private static void LSchemaSituationNormalize(SqliteConnection connection)
    {
        foreach (string column in new[] { "source_id", "source_state" })
        {
            using SqliteCommand check = connection.CreateCommand();
            check.CommandText =
                $"SELECT COUNT(*) FROM pragma_table_info('situation') WHERE name = '{column}';";
            if (Convert.ToInt64(check.ExecuteScalar()) == 0)
            {
                continue;
            }

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"ALTER TABLE situation DROP COLUMN {column};";
            command.ExecuteNonQuery();
        }
    }

    private static void LSchemaSpeechNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                "SELECT COUNT(*) FROM pragma_table_info('part_of_speech') WHERE name = 'custom_name';";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using (SqliteCommand off = connection.CreateCommand())
        {
            off.CommandText = "PRAGMA foreign_keys = OFF;";
            off.ExecuteNonQuery();
        }

        try
        {
            using SqliteTransaction rebuild = connection.BeginTransaction();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                DROP TABLE IF EXISTS part_of_speech_next;

                CREATE TABLE part_of_speech_next (
                    entry_id TEXT NOT NULL,
                    position INTEGER NOT NULL,
                    value_id TEXT,
                    custom_name TEXT,
                    PRIMARY KEY (entry_id, position),
                    CHECK ((value_id IS NULL) <> (custom_name IS NULL)),
                    FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
                );

                INSERT INTO part_of_speech_next (entry_id, position, value_id, custom_name)
                    SELECT entry_id, position, value_id, NULL FROM part_of_speech;

                DROP TABLE part_of_speech;

                ALTER TABLE part_of_speech_next RENAME TO part_of_speech;
                """;
            command.ExecuteNonQuery();
            rebuild.Commit();
        }
        finally
        {
            using SqliteCommand on = connection.CreateCommand();
            on.CommandText = "PRAGMA foreign_keys = ON;";
            on.ExecuteNonQuery();
        }
    }

    private static bool LSchemaVersionExist(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'schema_version';";
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    private static void LSchemaVersionCreate(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );

            INSERT INTO schema_version (id, version) VALUES (1, $version);
            """;
        command.Parameters.AddWithValue("$version", LSchemaMigrationVersion);
        command.ExecuteNonQuery();
    }

    private static long LSchemaVersionRead(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT ifnull(MAX(version), 0) FROM schema_version;";
        return Convert.ToInt64(command.ExecuteScalar());
    }

    private static void LSchemaVersionNormalize(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            DROP TABLE IF EXISTS schema_version_next;

            CREATE TABLE schema_version_next (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );

            INSERT INTO schema_version_next (id, version) VALUES (1, $version);

            DROP TABLE schema_version;

            ALTER TABLE schema_version_next RENAME TO schema_version;
            """;
        command.Parameters.AddWithValue("$version", LSchemaMigrationVersion);
        command.ExecuteNonQuery();
    }

    private static void LSchemaVersionSave(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE schema_version SET version = $version;";
        command.Parameters.AddWithValue("$version", LSchemaMigrationVersion);
        command.ExecuteNonQuery();
    }

    private static void LSchemaAudioNormalize(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS pronunciation_audio (
                pronunciation_id TEXT NOT NULL PRIMARY KEY,
                file TEXT NOT NULL,
                source TEXT,
                added_utc TEXT NOT NULL,
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }

    private static void LSchemaTitleNormalize(SqliteConnection connection, string table)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                $"SELECT COUNT(*) FROM pragma_table_info('{table}') WHERE name = 'title';";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"ALTER TABLE {table} ADD COLUMN title TEXT;";
        command.ExecuteNonQuery();
    }

    private static void LSchemaCollocationNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                "SELECT COUNT(*) FROM pragma_table_info('collocation') WHERE name = 'meaning';";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "ALTER TABLE collocation ADD COLUMN meaning TEXT;";
        command.ExecuteNonQuery();
    }

    private static void LSchemaExampleNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText = "SELECT COUNT(*) FROM pragma_foreign_key_list('example');";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using (SqliteCommand off = connection.CreateCommand())
        {
            off.CommandText = "PRAGMA foreign_keys = OFF;";
            off.ExecuteNonQuery();
        }

        try
        {
            using SqliteTransaction rebuild = connection.BeginTransaction();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                DROP TABLE IF EXISTS example_next;

                CREATE TABLE example_next (
                    id TEXT NOT NULL PRIMARY KEY,
                    language TEXT NOT NULL,
                    text TEXT NOT NULL,
                    local TEXT,
                    source_id TEXT,
                    FOREIGN KEY (source_id) REFERENCES source (id)
                );

                INSERT INTO example_next (id, language, text, local, source_id)
                    SELECT id, language, text, local, source_id FROM example;

                DROP TABLE example;

                ALTER TABLE example_next RENAME TO example;
                """;
            command.ExecuteNonQuery();
            rebuild.Commit();
        }
        finally
        {
            using SqliteCommand on = connection.CreateCommand();
            on.CommandText = "PRAGMA foreign_keys = ON;";
            on.ExecuteNonQuery();
        }
    }

    private static void LSchemaPositionNormalize(SqliteConnection connection, string table, string ownerColumn)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            DROP TABLE IF EXISTS temp.schema_position;

            CREATE TEMP TABLE schema_position AS
                SELECT id,
                       ROW_NUMBER() OVER (PARTITION BY {ownerColumn} ORDER BY position, id) - 1 AS position
                FROM {table};

            UPDATE {table} SET position =
                (SELECT position FROM temp.schema_position WHERE temp.schema_position.id = {table}.id);

            DROP TABLE temp.schema_position;
            """;
        command.ExecuteNonQuery();
    }
}
