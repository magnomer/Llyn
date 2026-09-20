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
            CREATE TABLE IF NOT EXISTS tag (
                tag_id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                text TEXT NOT NULL UNIQUE,
                CHECK (length(text) > 0),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE TABLE IF NOT EXISTS sense_tag (
                sense_parent INTEGER NOT NULL,
                tag_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_parent, tag_ref),
                FOREIGN KEY (sense_parent) REFERENCES sense (sense_id) ON DELETE CASCADE,
                FOREIGN KEY (tag_ref) REFERENCES tag (tag_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_tag_position
                ON sense_tag (sense_parent, position);

            CREATE TABLE IF NOT EXISTS collocation_tag (
                collocation_parent INTEGER NOT NULL,
                tag_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_parent, tag_ref),
                FOREIGN KEY (collocation_parent) REFERENCES collocation (collocation_id) ON DELETE CASCADE,
                FOREIGN KEY (tag_ref) REFERENCES tag (tag_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_tag_position
                ON collocation_tag (collocation_parent, position);

            CREATE TABLE IF NOT EXISTS sense_translation (
                sense_parent INTEGER NOT NULL,
                entry_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_parent, entry_ref),
                FOREIGN KEY (sense_parent) REFERENCES sense (sense_id) ON DELETE CASCADE,
                FOREIGN KEY (entry_ref) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS collocation_translation (
                collocation_parent INTEGER NOT NULL,
                entry_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_parent, entry_ref),
                FOREIGN KEY (collocation_parent) REFERENCES collocation (collocation_id) ON DELETE CASCADE,
                FOREIGN KEY (entry_ref) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS situation (
                situation_id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                title_state TEXT NOT NULL DEFAULT 'unspecified',
                title TEXT,
                description_state TEXT NOT NULL DEFAULT 'unspecified',
                description TEXT,
                kind_state TEXT NOT NULL DEFAULT 'unspecified',
                kind TEXT,
                CHECK (title_state = 'specified' OR title IS NULL),
                CHECK (description_state = 'specified' OR description IS NULL),
                CHECK (kind_state = 'specified' OR kind IS NULL),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE TABLE IF NOT EXISTS sense_situation (
                sense_parent INTEGER NOT NULL,
                situation_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_parent, situation_ref),
                FOREIGN KEY (sense_parent) REFERENCES sense (sense_id) ON DELETE CASCADE,
                FOREIGN KEY (situation_ref) REFERENCES situation (situation_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_situation_position
                ON sense_situation (sense_parent, position);

            CREATE TABLE IF NOT EXISTS collocation_situation (
                collocation_parent INTEGER NOT NULL,
                situation_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_parent, situation_ref),
                FOREIGN KEY (collocation_parent) REFERENCES collocation (collocation_id) ON DELETE CASCADE,
                FOREIGN KEY (situation_ref) REFERENCES situation (situation_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_situation_position
                ON collocation_situation (collocation_parent, position);
            """;
        command.ExecuteNonQuery();
    }
}
