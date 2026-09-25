using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaFavorite
{
    public static void LSchemaFavoriteCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS favorite (
                entry_parent INTEGER NOT NULL PRIMARY KEY,
                marked_utc TEXT NOT NULL,
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE INDEX IF NOT EXISTS favorite_marked ON favorite (marked_utc);
            """;
        command.ExecuteNonQuery();
    }
}
