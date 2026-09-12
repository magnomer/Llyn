using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaInflection
{
    public static void LSchemaInflectionCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS morphology_feature (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                speech_value_id INTEGER NOT NULL,
                pack_ref INTEGER NOT NULL,
                name TEXT NOT NULL,
                position INTEGER NOT NULL,
                UNIQUE (speech_value_id, pack_ref),
                FOREIGN KEY (speech_value_id) REFERENCES speech_value (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS morphology_value (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                morphology_feature_id INTEGER NOT NULL,
                pack_ref INTEGER NOT NULL,
                name TEXT NOT NULL,
                position INTEGER NOT NULL,
                UNIQUE (morphology_feature_id, pack_ref),
                FOREIGN KEY (morphology_feature_id) REFERENCES morphology_feature (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS inflection (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                text TEXT NOT NULL,
                local TEXT,
                speech_value_ref INTEGER,
                UNIQUE (entry_id, position),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE,
                FOREIGN KEY (speech_value_ref) REFERENCES speech_value (id)
            );

            CREATE TABLE IF NOT EXISTS inflection_feature (
                inflection_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                morphology_value_ref INTEGER NOT NULL,
                PRIMARY KEY (inflection_id, position),
                FOREIGN KEY (inflection_id) REFERENCES inflection (id) ON DELETE CASCADE,
                FOREIGN KEY (morphology_value_ref) REFERENCES morphology_value (id)
            );
            """;
        command.ExecuteNonQuery();
    }
}
