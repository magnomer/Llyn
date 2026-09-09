using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaExample
{
    public static void LSchemaExampleNormalize(SqliteConnection connection)
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

    public static void LSchemaExampleRebuild(SqliteConnection connection, string carried)
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

    public static string LSchemaCarriedRead(SqliteConnection connection)
    {
        const string local = "NULLIF(TRIM(COALESCE(example.local, '')), '')";

        string owned = LSchemaProbe.LSchemaTableFind(connection, "example_rendition")
            ? "example_rendition"
            : LSchemaProbe.LSchemaTableFind(connection, "example_translation")
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
}
