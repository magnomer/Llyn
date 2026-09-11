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
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                name TEXT NOT NULL,
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE TABLE IF NOT EXISTS source (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
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
                CHECK (url_state = 'specified' OR url IS NULL),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE TABLE IF NOT EXISTS source_author (
                source_id INTEGER NOT NULL,
                author_id INTEGER NOT NULL,
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
