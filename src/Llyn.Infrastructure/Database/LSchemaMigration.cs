using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaMigration
{
    public const long LSchemaMigrationVersion = 18;

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

        if (stored < 12)
        {
            LSchemaExampleNormalize(connection);
            LSchemaPositionNormalize(connection, "relation", "sense_id");
            LSchemaPositionNormalize(connection, "collocation", "entry_id");
            LSchemaPositionNormalize(connection, "collocation_synonym", "collocation_id");
            LSchemaPositionNormalize(connection, "example_translation", "example_id");
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

        if (stored < 17)
        {
            LSchemaSituationNormalize(connection);
        }

        if (stored < 18)
        {
            LSchemaState.LSchemaStateNormalize(connection);
        }

        LSchemaVersionSave(connection);
    }

    private static void LSchemaSituationNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                "SELECT COUNT(*) FROM pragma_table_info('situation') WHERE name = 'source_id';";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "ALTER TABLE situation ADD COLUMN source_id TEXT REFERENCES source (id);";
        command.ExecuteNonQuery();
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
