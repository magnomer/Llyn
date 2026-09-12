using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaTranscription
{
    public static void LSchemaTranscriptionCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS transcription (
                transcription_id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                scheme TEXT NOT NULL,
                text TEXT NOT NULL,
                UNIQUE (entry_parent, scheme),
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
