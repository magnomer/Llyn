using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaEtymology
{
    public static void LSchemaEtymologyCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS etymology (
                etymology_id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_parent INTEGER NOT NULL UNIQUE,
                text TEXT NOT NULL DEFAULT '',
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS etymology_mention (
                etymology_mention_id INTEGER PRIMARY KEY AUTOINCREMENT,
                etymology_parent INTEGER NOT NULL,
                start INTEGER NOT NULL,
                length INTEGER NOT NULL,
                entry_ref INTEGER NOT NULL,
                CHECK (start >= 0),
                CHECK (length > 0),
                FOREIGN KEY (etymology_parent) REFERENCES etymology (etymology_id) ON DELETE CASCADE,
                FOREIGN KEY (entry_ref) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS etymon (
                etymon_id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                entry_ref INTEGER NOT NULL,
                CHECK (position >= 0),
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE,
                FOREIGN KEY (entry_ref) REFERENCES entry (entry_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
