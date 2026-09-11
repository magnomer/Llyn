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
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                realm_origin INTEGER REFERENCES realm (id),
                id_origin INTEGER,
                headword TEXT NOT NULL,
                language TEXT NOT NULL,
                proficiency TEXT,
                frequency TEXT,
                added_utc TEXT,
                updated_utc TEXT,
                CHECK ((realm_origin IS NULL) = (id_origin IS NULL))
            );

            CREATE TABLE IF NOT EXISTS form (
                entry_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                text TEXT NOT NULL,
                local TEXT,
                role TEXT NOT NULL,
                PRIMARY KEY (entry_id, position),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS part_of_speech (
                entry_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                value_id TEXT,
                custom_name TEXT,
                PRIMARY KEY (entry_id, position),
                CHECK ((value_id IS NULL) <> (custom_name IS NULL)),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS part_of_speech_value (
                language TEXT NOT NULL,
                value_id TEXT NOT NULL,
                display_name TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (language, value_id)
            );
            """;
        command.ExecuteNonQuery();
    }
}
