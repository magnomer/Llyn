using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaStem
{
    public static void LSchemaStemCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS stem (
                stem_id INTEGER PRIMARY KEY AUTOINCREMENT,
                language TEXT NOT NULL,
                key TEXT NOT NULL,
                UNIQUE (language, key),
                CHECK (length(key) > 0)
            );

            CREATE TABLE IF NOT EXISTS shengfu_stem (
                shengfu_parent INTEGER NOT NULL,
                stem_ref INTEGER NOT NULL,
                PRIMARY KEY (shengfu_parent, stem_ref),
                FOREIGN KEY (shengfu_parent) REFERENCES shengfu (shengfu_id) ON DELETE CASCADE,
                FOREIGN KEY (stem_ref) REFERENCES stem (stem_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
