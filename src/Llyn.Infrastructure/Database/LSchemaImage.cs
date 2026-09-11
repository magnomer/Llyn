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
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                location_state TEXT NOT NULL DEFAULT 'unspecified',
                location TEXT,
                CHECK (location_state = 'specified' OR location IS NULL),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE TABLE IF NOT EXISTS sense_image (
                sense_id INTEGER NOT NULL,
                image_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, image_id),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (image_id) REFERENCES image (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_image_position
                ON sense_image (sense_id, position);

            CREATE TABLE IF NOT EXISTS collocation_image (
                collocation_id INTEGER NOT NULL,
                image_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, image_id),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (image_id) REFERENCES image (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_image_position
                ON collocation_image (collocation_id, position);
            """;
        command.ExecuteNonQuery();
    }
}
