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
            CREATE TABLE IF NOT EXISTS inflection (
                entry_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                text TEXT NOT NULL,
                local TEXT,
                part_of_speech_id TEXT,
                PRIMARY KEY (entry_id, position),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS inflection_feature (
                entry_id INTEGER NOT NULL,
                inflection_position INTEGER NOT NULL,
                position INTEGER NOT NULL,
                feature_id TEXT NOT NULL,
                value_id TEXT NOT NULL,
                PRIMARY KEY (entry_id, inflection_position, position),
                FOREIGN KEY (entry_id, inflection_position)
                    REFERENCES inflection (entry_id, position) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS morphology_value (
                language TEXT NOT NULL,
                part_of_speech_id TEXT NOT NULL,
                feature_id TEXT NOT NULL,
                feature_display_name TEXT NOT NULL,
                value_id TEXT NOT NULL,
                value_display_name TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (language, part_of_speech_id, feature_id, value_id)
            );
            """;
        command.ExecuteNonQuery();
    }
}
