using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaStamp
{
    private static readonly string[] LSchemaStampTable =
    [
        "entry",
        "example",
        "source",
        "author",
        "situation",
        "register",
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
                WHEN NEW.id_origin IS NULL
                BEGIN
                    UPDATE {table}
                    SET realm_origin = {LSchemaRealm.LSchemaRealmOwn}, id_origin = NEW.id
                    WHERE id = NEW.id;
                END;

                CREATE UNIQUE INDEX IF NOT EXISTS {table}_origin_unique
                ON {table} (realm_origin, id_origin);
                """;
            command.ExecuteNonQuery();
        }
    }
}
