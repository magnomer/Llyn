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
                morphology_feature_id INTEGER PRIMARY KEY AUTOINCREMENT,
                speech_value_parent INTEGER NOT NULL,
                pack_code INTEGER NOT NULL,
                name TEXT NOT NULL,
                position INTEGER NOT NULL,
                UNIQUE (speech_value_parent, pack_code),
                FOREIGN KEY (speech_value_parent) REFERENCES speech_value (speech_value_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS morphology_value (
                morphology_value_id INTEGER PRIMARY KEY AUTOINCREMENT,
                morphology_feature_parent INTEGER NOT NULL,
                pack_code INTEGER NOT NULL,
                name TEXT NOT NULL,
                position INTEGER NOT NULL,
                UNIQUE (morphology_feature_parent, pack_code),
                FOREIGN KEY (morphology_feature_parent)
                    REFERENCES morphology_feature (morphology_feature_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS inflection (
                inflection_id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                text TEXT NOT NULL,
                local TEXT,
                speech_value_ref INTEGER,
                UNIQUE (entry_parent, position),
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE,
                FOREIGN KEY (speech_value_ref) REFERENCES speech_value (speech_value_id)
            );

            CREATE TABLE IF NOT EXISTS inflection_feature (
                inflection_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                morphology_value_ref INTEGER NOT NULL,
                PRIMARY KEY (inflection_parent, position),
                FOREIGN KEY (inflection_parent) REFERENCES inflection (inflection_id) ON DELETE CASCADE,
                FOREIGN KEY (morphology_value_ref) REFERENCES morphology_value (morphology_value_id)
            );
            """;
        command.ExecuteNonQuery();
    }
}
