using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaRealm
{
    public const long LSchemaRealmOwn = 1;

    public static void LSchemaRealmCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                CREATE TABLE IF NOT EXISTS realm (
                    id INTEGER PRIMARY KEY,
                    value BLOB NOT NULL UNIQUE
                );
                """;
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO realm (value)
                VALUES ($value)
                ON CONFLICT (id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$id", LSchemaRealmOwn);
            command.Parameters.AddWithValue("$value", Guid.NewGuid().ToByteArray());
            command.ExecuteNonQuery();
        }
    }
}
