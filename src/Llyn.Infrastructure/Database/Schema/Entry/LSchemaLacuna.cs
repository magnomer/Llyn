using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaLacuna
{
    public static void LSchemaLacunaCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS lacuna (
                entry_parent INTEGER NOT NULL,
                morphology_value_ref INTEGER,
                fetched_utc TEXT NOT NULL,
                PRIMARY KEY (entry_parent, morphology_value_ref),
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE,
                FOREIGN KEY (morphology_value_ref)
                    REFERENCES morphology_value (morphology_value_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
