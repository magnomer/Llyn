using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaStamp
{
    private static readonly string[] LSchemaStampTable =
    [
        "entry",
        "example",
        "reference",
        "author",
        "situation",
        "register",
        "tag",
        "image",
        "video",
    ];

    public static void LSchemaStampCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        foreach (string table in LSchemaStampTable)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                $"""
                CREATE TRIGGER IF NOT EXISTS {table}_origin
                AFTER INSERT ON {table}
                WHEN NEW.origin_id = 0
                BEGIN
                    UPDATE {table}
                    SET origin_realm = (SELECT value FROM realm WHERE realm_id = {LSchemaRealm.LSchemaRealmRow}),
                        origin_id = NEW.{table}_id
                    WHERE {table}_id = NEW.{table}_id;
                END;

                CREATE UNIQUE INDEX IF NOT EXISTS {table}_origin_unique
                ON {table} (origin_realm, origin_id);
                """;
            command.ExecuteNonQuery();
        }
    }
}
