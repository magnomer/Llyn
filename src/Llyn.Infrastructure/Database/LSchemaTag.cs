using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaTag
{
    public static void LSchemaTagNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                "SELECT COUNT(*) FROM pragma_table_info('sense_tag') WHERE name = 'text';";
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
                DROP TABLE IF EXISTS sense_tag_next;

                CREATE TABLE sense_tag_next (
                    sense_id TEXT NOT NULL,
                    text TEXT NOT NULL,
                    position INTEGER NOT NULL,
                    PRIMARY KEY (sense_id, text),
                    CHECK (length(text) > 0),
                    FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE
                );

                INSERT INTO sense_tag_next (sense_id, text, position)
                    SELECT sense_id, text, position - 1 FROM (
                        SELECT link.sense_id AS sense_id,
                               tag.text AS text,
                               ROW_NUMBER() OVER (
                                   PARTITION BY link.sense_id ORDER BY MIN(link.position)) AS position
                        FROM sense_tag link
                        JOIN tag ON tag.id = link.tag_id
                        WHERE tag.text IS NOT NULL AND length(tag.text) > 0
                        GROUP BY link.sense_id, tag.text
                    );

                DROP TABLE sense_tag;

                ALTER TABLE sense_tag_next RENAME TO sense_tag;

                DROP TABLE IF EXISTS collocation_tag_next;

                CREATE TABLE collocation_tag_next (
                    collocation_id TEXT NOT NULL,
                    text TEXT NOT NULL,
                    position INTEGER NOT NULL,
                    PRIMARY KEY (collocation_id, text),
                    CHECK (length(text) > 0),
                    FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE
                );

                INSERT INTO collocation_tag_next (collocation_id, text, position)
                    SELECT collocation_id, text, position - 1 FROM (
                        SELECT link.collocation_id AS collocation_id,
                               tag.text AS text,
                               ROW_NUMBER() OVER (
                                   PARTITION BY link.collocation_id ORDER BY MIN(link.position)) AS position
                        FROM collocation_tag link
                        JOIN tag ON tag.id = link.tag_id
                        WHERE tag.text IS NOT NULL AND length(tag.text) > 0
                        GROUP BY link.collocation_id, tag.text
                    );

                DROP TABLE collocation_tag;

                ALTER TABLE collocation_tag_next RENAME TO collocation_tag;

                DROP TABLE tag;

                CREATE UNIQUE INDEX IF NOT EXISTS sense_tag_position
                    ON sense_tag (sense_id, position);

                CREATE UNIQUE INDEX IF NOT EXISTS collocation_tag_position
                    ON collocation_tag (collocation_id, position);

                CREATE INDEX IF NOT EXISTS sense_tag_member ON sense_tag (text);

                CREATE INDEX IF NOT EXISTS collocation_tag_member ON collocation_tag (text);
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
