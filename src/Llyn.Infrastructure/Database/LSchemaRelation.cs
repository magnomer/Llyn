using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaRelation
{
    public static void LSchemaRelationCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS relation (
                id TEXT NOT NULL PRIMARY KEY,
                sense_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                relation_type TEXT NOT NULL,
                label TEXT,
                labels TEXT,
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS relation_entry (
                relation_id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL,
                FOREIGN KEY (relation_id) REFERENCES relation (id) ON DELETE CASCADE,
                FOREIGN KEY (entry_id) REFERENCES entry (id)
            );

            CREATE TABLE IF NOT EXISTS relation_sense (
                relation_id TEXT NOT NULL PRIMARY KEY,
                sense_id TEXT NOT NULL,
                FOREIGN KEY (relation_id) REFERENCES relation (id) ON DELETE CASCADE,
                FOREIGN KEY (sense_id) REFERENCES sense (id)
            );
            """;
        command.ExecuteNonQuery();
    }
}
