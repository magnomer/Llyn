using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaRealm
{
    public const long LSchemaRealmRow = 1;

    public static void LSchemaRealmCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                CREATE TABLE IF NOT EXISTS realm (
                    id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                    value BLOB NOT NULL CHECK (length(value) = 16)
                );
                """;
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO realm (id, value)
                VALUES ($id, $value)
                ON CONFLICT (id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$id", LSchemaRealmRow);
            command.Parameters.AddWithValue("$value", Guid.NewGuid().ToByteArray());
            command.ExecuteNonQuery();
        }
    }
}
