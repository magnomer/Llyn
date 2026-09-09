using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaSpeech
{
    public static void LSchemaSpeechNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                "SELECT COUNT(*) FROM pragma_table_info('part_of_speech') WHERE name = 'custom_name';";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using (SqliteCommand off = connection.CreateCommand())
        {
            off.CommandText = "PRAGMA foreign_keys = OFF;";
            off.ExecuteNonQuery();
        }

        try
        {
            using SqliteTransaction rebuild = connection.BeginTransaction();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                DROP TABLE IF EXISTS part_of_speech_next;

                CREATE TABLE part_of_speech_next (
                    entry_id TEXT NOT NULL,
                    position INTEGER NOT NULL,
                    value_id TEXT,
                    custom_name TEXT,
                    PRIMARY KEY (entry_id, position),
                    CHECK ((value_id IS NULL) <> (custom_name IS NULL)),
                    FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
                );

                INSERT INTO part_of_speech_next (entry_id, position, value_id, custom_name)
                    SELECT entry_id, position, value_id, NULL FROM part_of_speech;

                DROP TABLE part_of_speech;

                ALTER TABLE part_of_speech_next RENAME TO part_of_speech;
                """;
            command.ExecuteNonQuery();
            rebuild.Commit();
        }
        finally
        {
            using SqliteCommand on = connection.CreateCommand();
            on.CommandText = "PRAGMA foreign_keys = ON;";
            on.ExecuteNonQuery();
        }
    }
}
