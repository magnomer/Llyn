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
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                name_state TEXT NOT NULL DEFAULT 'unspecified',
                name TEXT,
                language TEXT,
                pack_ref INTEGER,
                CHECK (name_state = 'specified' OR name IS NULL),
                CHECK (pack_ref IS NULL OR language IS NOT NULL),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS register_pack
            ON register (language, pack_ref) WHERE pack_ref IS NOT NULL;

            CREATE TABLE IF NOT EXISTS sense_register (
                sense_id INTEGER NOT NULL,
                register_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, register_ref),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (register_ref) REFERENCES register (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_register_position
                ON sense_register (sense_id, position);

            CREATE TABLE IF NOT EXISTS collocation_register (
                collocation_id INTEGER NOT NULL,
                register_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, register_ref),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (register_ref) REFERENCES register (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_register_position
                ON collocation_register (collocation_id, position);
            """;
        command.ExecuteNonQuery();
    }
}
