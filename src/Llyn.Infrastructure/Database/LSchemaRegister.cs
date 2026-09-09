using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaRegister
{
    public static void LSchemaRegisterCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS register (
                id TEXT NOT NULL PRIMARY KEY,
                name_state TEXT NOT NULL DEFAULT 'unspecified',
                name TEXT,
                language TEXT,
                builtin INTEGER NOT NULL DEFAULT 0,
                CHECK (name_state = 'specified' OR name IS NULL),
                CHECK (builtin IN (0, 1)),
                CHECK (builtin = 0 OR language IS NOT NULL)
            );

            CREATE TABLE IF NOT EXISTS sense_register (
                sense_id TEXT NOT NULL,
                register_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, register_id),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (register_id) REFERENCES register (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_register_position
                ON sense_register (sense_id, position);

            CREATE TABLE IF NOT EXISTS collocation_register (
                collocation_id TEXT NOT NULL,
                register_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, register_id),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (register_id) REFERENCES register (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_register_position
                ON collocation_register (collocation_id, position);
            """;
        command.ExecuteNonQuery();
    }
}
