using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaPronunciation
{
    public static void LSchemaPronunciationCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS pronunciation (
                id INTEGER PRIMARY KEY,
                entry_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                variety TEXT,
                ipa TEXT,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS syllable (
                pronunciation_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                onset TEXT,
                medial TEXT,
                nucleus TEXT NOT NULL,
                coda TEXT,
                tone_number INTEGER,
                tone_points TEXT,
                PRIMARY KEY (pronunciation_id, position),
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS pronunciation_audio (
                pronunciation_id INTEGER NOT NULL PRIMARY KEY,
                file TEXT NOT NULL,
                source TEXT,
                added_utc TEXT NOT NULL,
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
