using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaMarker
{
    public static void LSchemaMarkerCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS sense_tag (
                sense_id INTEGER NOT NULL,
                text TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, text),
                CHECK (length(text) > 0),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_tag_position
                ON sense_tag (sense_id, position);

            CREATE TABLE IF NOT EXISTS collocation_tag (
                collocation_id INTEGER NOT NULL,
                text TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, text),
                CHECK (length(text) > 0),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_tag_position
                ON collocation_tag (collocation_id, position);

            CREATE TABLE IF NOT EXISTS sense_translation (
                sense_id INTEGER NOT NULL,
                entry_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, entry_id),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS collocation_translation (
                collocation_id INTEGER NOT NULL,
                entry_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, entry_id),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS situation (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                realm_origin INTEGER REFERENCES realm (id),
                id_origin INTEGER,
                title_state TEXT NOT NULL DEFAULT 'unspecified',
                title TEXT,
                description_state TEXT NOT NULL DEFAULT 'unspecified',
                description TEXT,
                kind_state TEXT NOT NULL DEFAULT 'unspecified',
                kind TEXT,
                CHECK (title_state = 'specified' OR title IS NULL),
                CHECK (description_state = 'specified' OR description IS NULL),
                CHECK (kind_state = 'specified' OR kind IS NULL),
                CHECK ((realm_origin IS NULL) = (id_origin IS NULL))
            );

            CREATE TABLE IF NOT EXISTS sense_situation (
                sense_id INTEGER NOT NULL,
                situation_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, situation_id),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (situation_id) REFERENCES situation (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_situation_position
                ON sense_situation (sense_id, position);

            CREATE TABLE IF NOT EXISTS collocation_situation (
                collocation_id INTEGER NOT NULL,
                situation_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, situation_id),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (situation_id) REFERENCES situation (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_situation_position
                ON collocation_situation (collocation_id, position);
            """;
        command.ExecuteNonQuery();
    }
}
