using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaSource
{
    public static void LSchemaSourceNormalize(SqliteConnection connection)
    {
        if (LSchemaProbe.LSchemaColumnFind(connection, "source", "kind"))
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
            using SqliteTransaction rebuild = connection.BeginTransaction();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                DROP TABLE IF EXISTS source_next;

                CREATE TABLE source_next (
                    id TEXT NOT NULL PRIMARY KEY,
                    title_state TEXT NOT NULL DEFAULT 'unspecified',
                    title TEXT,
                    year_state TEXT NOT NULL,
                    year TEXT,
                    kind TEXT NOT NULL DEFAULT 'unspecified',
                    note_state TEXT NOT NULL,
                    note TEXT,
                    url_state TEXT NOT NULL,
                    url TEXT,
                    author_state TEXT NOT NULL,
                    CHECK (title_state = 'specified' OR title IS NULL),
                    CHECK (year_state = 'specified' OR year IS NULL),
                    CHECK (note_state = 'specified' OR note IS NULL),
                    CHECK (url_state = 'specified' OR url IS NULL)
                );

                INSERT INTO source_next (
                    id, title_state, title, year_state, year, kind,
                    note_state, note, url_state, url, author_state)
                    SELECT
                        source.id,
                        source.title_state,
                        source.title,
                        source.year_state,
                        source.year,
                        'unspecified',
                        CASE WHEN NULLIF(
                                TRIM(COALESCE(source.program_name, '') || ' ' || COALESCE(source.channel_name, '')),
                                '') IS NULL
                            THEN 'unspecified' ELSE 'specified' END,
                        NULLIF(
                            TRIM(COALESCE(source.program_name, '') || ' ' || COALESCE(source.channel_name, '')),
                            ''),
                        source.url_state,
                        source.url,
                        source.author_state
                    FROM source;

                DROP TABLE source;

                ALTER TABLE source_next RENAME TO source;
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
}
