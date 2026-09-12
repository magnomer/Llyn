using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaVideo
{
    public static void LSchemaVideoCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS video (
                video_id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                location_state TEXT NOT NULL DEFAULT 'unspecified',
                location TEXT,
                span_state TEXT NOT NULL DEFAULT 'unspecified',
                span TEXT,
                CHECK (location_state = 'specified' OR location IS NULL),
                CHECK (span_state = 'specified' OR span IS NULL),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE TABLE IF NOT EXISTS sense_video (
                sense_parent INTEGER NOT NULL,
                video_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_parent, video_ref),
                FOREIGN KEY (sense_parent) REFERENCES sense (sense_id) ON DELETE CASCADE,
                FOREIGN KEY (video_ref) REFERENCES video (video_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_video_position
                ON sense_video (sense_parent, position);

            CREATE TABLE IF NOT EXISTS collocation_video (
                collocation_parent INTEGER NOT NULL,
                video_ref INTEGER NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_parent, video_ref),
                FOREIGN KEY (collocation_parent) REFERENCES collocation (collocation_id) ON DELETE CASCADE,
                FOREIGN KEY (video_ref) REFERENCES video (video_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_video_position
                ON collocation_video (collocation_parent, position);
            """;
        command.ExecuteNonQuery();
    }
}
