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
                id TEXT NOT NULL PRIMARY KEY,
                location_state TEXT NOT NULL DEFAULT 'unspecified',
                location TEXT,
                span_state TEXT NOT NULL DEFAULT 'unspecified',
                span TEXT,
                CHECK (location_state = 'specified' OR location IS NULL),
                CHECK (span_state = 'specified' OR span IS NULL)
            );

            CREATE TABLE IF NOT EXISTS sense_video (
                sense_id TEXT NOT NULL,
                video_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, video_id),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (video_id) REFERENCES video (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_video_position
                ON sense_video (sense_id, position);

            CREATE TABLE IF NOT EXISTS collocation_video (
                collocation_id TEXT NOT NULL,
                video_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, video_id),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (video_id) REFERENCES video (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_video_position
                ON collocation_video (collocation_id, position);
            """;
        command.ExecuteNonQuery();
    }
}
