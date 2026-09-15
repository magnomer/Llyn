using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaFanqie
{
    public static void LSchemaFanqieCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS fanqie (
                fanqie_id INTEGER PRIMARY KEY AUTOINCREMENT,
                language TEXT NOT NULL,
                character TEXT NOT NULL,
                book TEXT NOT NULL,
                position INTEGER NOT NULL,
                text TEXT NOT NULL,
                initial TEXT NOT NULL,
                rime TEXT NOT NULL,
                heading TEXT NOT NULL,
                division TEXT NOT NULL,
                tone TEXT NOT NULL,
                rounded INTEGER NOT NULL,
                source TEXT NOT NULL,
                spelling TEXT NOT NULL,
                UNIQUE (language, character, book, source, position)
            );
            """;
        command.ExecuteNonQuery();
    }
}
