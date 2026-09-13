using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaImage
{
    public static void LSchemaImageCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS image (
                image_id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                location_state TEXT NOT NULL DEFAULT 'unspecified',
                location TEXT,
                CHECK (location_state = 'specified' OR location IS NULL),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE TABLE IF NOT EXISTS sense_image (
                sense_parent INTEGER NOT NULL,
                image_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_parent, image_ref),
                FOREIGN KEY (sense_parent) REFERENCES sense (sense_id) ON DELETE CASCADE,
                FOREIGN KEY (image_ref) REFERENCES image (image_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_image_position
                ON sense_image (sense_parent, position);

            CREATE TABLE IF NOT EXISTS collocation_image (
                collocation_parent INTEGER NOT NULL,
                image_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_parent, image_ref),
                FOREIGN KEY (collocation_parent) REFERENCES collocation (collocation_id) ON DELETE CASCADE,
                FOREIGN KEY (image_ref) REFERENCES image (image_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_image_position
                ON collocation_image (collocation_parent, position);

            CREATE TABLE IF NOT EXISTS situation_image (
                situation_parent INTEGER NOT NULL,
                image_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (situation_parent, image_ref),
                FOREIGN KEY (situation_parent) REFERENCES situation (situation_id) ON DELETE CASCADE,
                FOREIGN KEY (image_ref) REFERENCES image (image_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS situation_image_position
                ON situation_image (situation_parent, position);
            """;
        command.ExecuteNonQuery();
    }
}
