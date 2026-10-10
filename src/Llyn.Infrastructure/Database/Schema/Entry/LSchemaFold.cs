using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaFold
{
    public static void LSchemaFoldCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS sense_fold (
                sense_parent INTEGER NOT NULL PRIMARY KEY,
                FOREIGN KEY (sense_parent) REFERENCES sense (sense_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS collocation_fold (
                collocation_parent INTEGER NOT NULL PRIMARY KEY,
                FOREIGN KEY (collocation_parent) REFERENCES collocation (collocation_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS reflex_fold (
                entry_parent INTEGER NOT NULL PRIMARY KEY,
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS fanqie_fold (
                entry_parent INTEGER NOT NULL PRIMARY KEY,
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS script_fold (
                entry_parent INTEGER NOT NULL PRIMARY KEY,
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS stem_fold (
                entry_parent INTEGER NOT NULL,
                key TEXT NOT NULL,
                PRIMARY KEY (entry_parent, key),
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
