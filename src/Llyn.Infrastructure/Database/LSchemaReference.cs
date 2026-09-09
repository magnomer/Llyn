using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaReference
{
    public static void LSchemaReferenceCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS author (
                id TEXT NOT NULL PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS source (
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

            CREATE TABLE IF NOT EXISTS source_author (
                source_id TEXT NOT NULL,
                author_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (source_id, author_id),
                FOREIGN KEY (source_id) REFERENCES source (id) ON DELETE CASCADE,
                FOREIGN KEY (author_id) REFERENCES author (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS source_author_position
                ON source_author (source_id, position);
            """;
        command.ExecuteNonQuery();
    }
}
