using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaReflex
{
    public const long LSchemaReflexNoted = 60;

    public static void LSchemaReflexCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS reflex (
                reflex_id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                language TEXT NOT NULL,
                kind TEXT NOT NULL,
                text TEXT NOT NULL,
                main INTEGER NOT NULL DEFAULT 0,
                note TEXT NOT NULL DEFAULT '',
                respelling TEXT NOT NULL DEFAULT '',
                region TEXT NOT NULL DEFAULT '',
                remark TEXT NOT NULL DEFAULT '',
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }

    public static void LSchemaReflexSettle(SqliteConnection connection, string schema, long stored)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);

        if (stored >= LSchemaReflexNoted)
        {
            return;
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {schema}.reflex;";
        command.ExecuteNonQuery();
    }
}
