using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaShengfu
{
    public static void LSchemaShengfuCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS shengfu (
                shengfu_id INTEGER PRIMARY KEY AUTOINCREMENT,
                language TEXT NOT NULL,
                character TEXT NOT NULL,
                text TEXT NOT NULL,
                source TEXT NOT NULL,
                UNIQUE (language, character)
            );
            """;
        command.ExecuteNonQuery();
    }
}
