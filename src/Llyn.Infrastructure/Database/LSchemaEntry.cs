using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaEntry
{
    public static void LSchemaEntryCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS entry (
                entry_id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                headword TEXT NOT NULL,
                language TEXT NOT NULL,
                grasp INTEGER NOT NULL DEFAULT 0 CHECK (grasp BETWEEN 0 AND 10),
                frequency TEXT,
                added_utc TEXT,
                updated_utc TEXT,
                epithet TEXT NOT NULL DEFAULT '',
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE TABLE IF NOT EXISTS form (
                entry_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                text TEXT NOT NULL,
                local TEXT,
                role TEXT NOT NULL,
                PRIMARY KEY (entry_parent, position),
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS speech_value (
                speech_value_id INTEGER PRIMARY KEY AUTOINCREMENT,
                language TEXT NOT NULL,
                pack_code INTEGER NOT NULL,
                name TEXT NOT NULL,
                position INTEGER NOT NULL,
                UNIQUE (language, pack_code)
            );

            CREATE TABLE IF NOT EXISTS part_of_speech (
                entry_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                speech_value_ref INTEGER,
                custom_name TEXT,
                PRIMARY KEY (entry_parent, position),
                CHECK ((speech_value_ref IS NULL) <> (custom_name IS NULL)),
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE,
                FOREIGN KEY (speech_value_ref) REFERENCES speech_value (speech_value_id)
            );
            """;
        command.ExecuteNonQuery();
    }
}
