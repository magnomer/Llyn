using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaState
{
    public static void LSchemaStateNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                "SELECT COUNT(*) FROM pragma_table_info('example') WHERE name = 'text_state';";
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
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    """
                    DROP TABLE IF EXISTS sense_next;

                    CREATE TABLE sense_next (
                        id TEXT NOT NULL PRIMARY KEY,
                        entry_id TEXT NOT NULL,
                        parent_id TEXT,
                        position INTEGER NOT NULL,
                        title_state TEXT NOT NULL DEFAULT 'unspecified',
                        title TEXT,
                        gloss TEXT,
                        definition_language TEXT,
                        definition_state TEXT NOT NULL DEFAULT 'unspecified',
                        definition TEXT,
                        labels TEXT NOT NULL,
                        CHECK (title_state = 'specified' OR title IS NULL),
                        CHECK (definition_state = 'specified' OR definition IS NULL),
                        FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE,
                        FOREIGN KEY (parent_id) REFERENCES sense (id) ON DELETE CASCADE
                    );

                    INSERT INTO sense_next
                        SELECT id, entry_id, parent_id, position,
                               iif(title IS NULL, 'unspecified', 'specified'), title,
                               gloss, definition_language,
                               iif(definition IS NULL, 'unspecified', 'specified'), definition,
                               labels
                        FROM sense;

                    DROP TABLE sense;

                    ALTER TABLE sense_next RENAME TO sense;

                    DROP TABLE IF EXISTS collocation_next;

                    CREATE TABLE collocation_next (
                        id TEXT NOT NULL PRIMARY KEY,
                        entry_id TEXT NOT NULL,
                        position INTEGER NOT NULL,
                        title_state TEXT NOT NULL DEFAULT 'unspecified',
                        title TEXT,
                        expression_state TEXT NOT NULL DEFAULT 'unspecified',
                        expression TEXT,
                        meaning_state TEXT NOT NULL DEFAULT 'unspecified',
                        meaning TEXT,
                        CHECK (title_state = 'specified' OR title IS NULL),
                        CHECK (expression_state = 'specified' OR expression IS NULL),
                        CHECK (meaning_state = 'specified' OR meaning IS NULL),
                        FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
                    );

                    INSERT INTO collocation_next
                        SELECT id, entry_id, position,
                               iif(title IS NULL, 'unspecified', 'specified'), title,
                               iif(expression IS NULL, 'unspecified', 'specified'), expression,
                               iif(meaning IS NULL, 'unspecified', 'specified'), meaning
                        FROM collocation;

                    DROP TABLE collocation;

                    ALTER TABLE collocation_next RENAME TO collocation;
                    """;
                command.ExecuteNonQuery();
            }

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    """
                    DROP TABLE IF EXISTS example_next;

                    CREATE TABLE example_next (
                        id TEXT NOT NULL PRIMARY KEY,
                        language TEXT NOT NULL,
                        text_state TEXT NOT NULL DEFAULT 'unspecified',
                        text TEXT,
                        local TEXT,
                        source_state TEXT NOT NULL DEFAULT 'unspecified',
                        source_id TEXT,
                        CHECK (text_state = 'specified' OR text IS NULL),
                        CHECK (source_state = 'specified' OR source_id IS NULL),
                        FOREIGN KEY (source_id) REFERENCES source (id)
                    );

                    INSERT INTO example_next
                        SELECT id, language,
                               iif(text IS NULL, 'unspecified', 'specified'), text,
                               local,
                               iif(source_id IS NULL, 'unspecified', 'specified'), source_id
                        FROM example;

                    DROP TABLE example;

                    ALTER TABLE example_next RENAME TO example;

                    DROP TABLE IF EXISTS situation_next;

                    CREATE TABLE situation_next (
                        id TEXT NOT NULL PRIMARY KEY,
                        title_state TEXT NOT NULL DEFAULT 'unspecified',
                        title TEXT,
                        description_state TEXT NOT NULL DEFAULT 'unspecified',
                        description TEXT,
                        kind_state TEXT NOT NULL DEFAULT 'unspecified',
                        kind TEXT,
                        CHECK (title_state = 'specified' OR title IS NULL),
                        CHECK (description_state = 'specified' OR description IS NULL),
                        CHECK (kind_state = 'specified' OR kind IS NULL)
                    );

                    INSERT INTO situation_next
                        SELECT id,
                               iif(title IS NULL, 'unspecified', 'specified'), title,
                               iif(description IS NULL, 'unspecified', 'specified'), description,
                               iif(kind IS NULL, 'unspecified', 'specified'), kind
                        FROM situation;

                    DROP TABLE situation;

                    ALTER TABLE situation_next RENAME TO situation;
                    """;
                command.ExecuteNonQuery();
            }

            rebuild.Commit();
        }
        finally
        {
            using SqliteCommand on = connection.CreateCommand();
            on.CommandText = "PRAGMA foreign_keys = ON;";
            on.ExecuteNonQuery();
        }
    }
}
