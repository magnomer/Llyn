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
                register_id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                name_state TEXT NOT NULL DEFAULT 'unspecified',
                name TEXT,
                language TEXT,
                pack_code INTEGER,
                CHECK (name_state = 'specified' OR name IS NULL),
                CHECK (pack_code IS NULL OR language IS NOT NULL),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS register_pack
            ON register (language, pack_code) WHERE pack_code IS NOT NULL;

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
}
