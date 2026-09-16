using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaAnchor
{
    public static void LSchemaAnchorCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS anchor (
                reflex_parent INTEGER NOT NULL,
                fanqie_ref INTEGER NOT NULL,
                PRIMARY KEY (reflex_parent, fanqie_ref),
                FOREIGN KEY (reflex_parent) REFERENCES reflex (reflex_id) ON DELETE CASCADE,
                FOREIGN KEY (fanqie_ref) REFERENCES fanqie (fanqie_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
