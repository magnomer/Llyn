using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaScript
{
    public static void LSchemaScriptCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS script (
                script_id INTEGER PRIMARY KEY AUTOINCREMENT,
                language TEXT NOT NULL,
                character TEXT NOT NULL,
                style TEXT NOT NULL,
                position INTEGER NOT NULL,
                caption TEXT NOT NULL,
                gloss TEXT NOT NULL,
                data BLOB NOT NULL,
                UNIQUE (language, character, style, position)
            );
            """;
        command.ExecuteNonQuery();
    }
}
