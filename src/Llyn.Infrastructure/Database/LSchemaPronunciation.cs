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
                pronunciation_id INTEGER PRIMARY KEY,
                entry_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                variety TEXT,
                ipa TEXT,
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS syllable (
                pronunciation_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                onset TEXT,
                medial TEXT,
                nucleus TEXT NOT NULL,
                coda TEXT,
                tone_number INTEGER,
                tone_points TEXT,
                PRIMARY KEY (pronunciation_parent, position),
                FOREIGN KEY (pronunciation_parent) REFERENCES pronunciation (pronunciation_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS pronunciation_audio (
                pronunciation_parent INTEGER NOT NULL PRIMARY KEY,
                file TEXT NOT NULL,
                site TEXT,
                added_utc TEXT NOT NULL,
                FOREIGN KEY (pronunciation_parent) REFERENCES pronunciation (pronunciation_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
