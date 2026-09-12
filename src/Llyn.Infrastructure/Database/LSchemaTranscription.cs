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
                id INTEGER PRIMARY KEY,
                entry_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                scheme TEXT NOT NULL,
                text TEXT NOT NULL,
                UNIQUE (entry_id, scheme),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
