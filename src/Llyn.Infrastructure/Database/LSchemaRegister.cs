using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaRegister
{
    private const long LSchemaRegisterShift = 1000000000;

    public static void LSchemaRegisterCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS register (
                register_id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                name_state TEXT NOT NULL DEFAULT 'unspecified',
                name TEXT,
                builtin INTEGER NOT NULL DEFAULT 0,
                CHECK (name_state = 'specified' OR name IS NULL),
                CHECK (builtin IN (0, 1)),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS register_name
            ON register (name) WHERE name IS NOT NULL;

            CREATE TABLE IF NOT EXISTS sense_register (
                sense_parent INTEGER NOT NULL,
                register_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_parent, register_ref),
                FOREIGN KEY (sense_parent) REFERENCES sense (sense_id) ON DELETE CASCADE,
                FOREIGN KEY (register_ref) REFERENCES register (register_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_register_position
                ON sense_register (sense_parent, position);

            CREATE TABLE IF NOT EXISTS collocation_register (
                collocation_parent INTEGER NOT NULL,
                register_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_parent, register_ref),
                FOREIGN KEY (collocation_parent) REFERENCES collocation (collocation_id) ON DELETE CASCADE,
                FOREIGN KEY (register_ref) REFERENCES register (register_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_register_position
                ON collocation_register (collocation_parent, position);
            """;
        command.ExecuteNonQuery();
    }

    public static void LSchemaRegisterSettle(SqliteConnection connection, string schema, string other)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(other);

        LSchemaLinkSettle(connection, schema, other, "sense_register", "sense_parent");
        LSchemaLinkSettle(connection, schema, other, "collocation_register", "collocation_parent");
    }

    private static void LSchemaLinkSettle(
        SqliteConnection connection,
        string schema,
        string other,
        string table,
        string column)
    {
        using (SqliteCommand repoint = connection.CreateCommand())
        {
            repoint.CommandText =
                $"""
                UPDATE OR REPLACE {schema}.{table} AS link
                SET register_ref = (
                    SELECT kept.register_id FROM {other}.register lost
                    JOIN {schema}.register kept ON kept.name = lost.name
                    WHERE lost.register_id = link.register_ref
                )
                WHERE link.register_ref NOT IN (SELECT register_id FROM {schema}.register)
                  AND EXISTS (
                    SELECT 1 FROM {other}.register lost
                    JOIN {schema}.register kept ON kept.name = lost.name
                    WHERE lost.register_id = link.register_ref
                  );
                """;
            repoint.ExecuteNonQuery();
        }

        using SqliteCommand renumber = connection.CreateCommand();
        renumber.CommandText =
            $"""
            UPDATE {schema}.{table} AS link
            SET position = ranked.rank - {LSchemaRegisterShift}
            FROM (
                SELECT rowid AS row, ROW_NUMBER() OVER (PARTITION BY {column} ORDER BY position) - 1 AS rank
                FROM {schema}.{table}
            ) AS ranked
            WHERE link.rowid = ranked.row;

            UPDATE {schema}.{table} SET position = position + {LSchemaRegisterShift};
            """;
        renumber.ExecuteNonQuery();
    }
}
