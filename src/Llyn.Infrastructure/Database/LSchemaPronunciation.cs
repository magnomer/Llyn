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
                id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL UNIQUE,
                level TEXT,
                ipa TEXT,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS syllable (
                pronunciation_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                orthography TEXT,
                local TEXT,
                onset TEXT,
                medial TEXT,
                nucleus TEXT NOT NULL,
                coda TEXT,
                tone_number INTEGER,
                tone_local TEXT,
                tone_points TEXT,
                PRIMARY KEY (pronunciation_id, position),
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS representation (
                pronunciation_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                system TEXT NOT NULL,
                role TEXT NOT NULL,
                text TEXT NOT NULL,
                local_tone TEXT,
                PRIMARY KEY (pronunciation_id, position),
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS pronunciation_audio (
                pronunciation_id TEXT NOT NULL PRIMARY KEY,
                file TEXT NOT NULL,
                source TEXT,
                added_utc TEXT NOT NULL,
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
