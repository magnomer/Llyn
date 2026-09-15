using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaDiwei
{
    public static void LSchemaDiweiCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS diwei (
                diwei_id INTEGER PRIMARY KEY AUTOINCREMENT,
                language TEXT NOT NULL,
                kind TEXT NOT NULL,
                key TEXT NOT NULL,
                UNIQUE (language, kind, key),
                CHECK (length(key) > 0)
            );

            CREATE TABLE IF NOT EXISTS fanqie_diwei (
                fanqie_parent INTEGER NOT NULL,
                diwei_ref INTEGER NOT NULL,
                PRIMARY KEY (fanqie_parent, diwei_ref),
                FOREIGN KEY (fanqie_parent) REFERENCES fanqie (fanqie_id) ON DELETE CASCADE,
                FOREIGN KEY (diwei_ref) REFERENCES diwei (diwei_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
