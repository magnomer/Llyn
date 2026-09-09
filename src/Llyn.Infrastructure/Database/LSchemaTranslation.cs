using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaTranslation
{
    public static void LSchemaTranslationCreate(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS sense_translation (
                sense_id TEXT NOT NULL,
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, entry_id),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS collocation_translation (
                collocation_id TEXT NOT NULL,
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, entry_id),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_translation_position
                ON sense_translation (sense_id, position);

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_translation_position
                ON collocation_translation (collocation_id, position);
            """;
        command.ExecuteNonQuery();
    }

    public static void LSchemaTranslationNormalize(SqliteConnection connection)
    {
        if (LSchemaProbe.LSchemaColumnFind(connection, "example", "local"))
        {
            LSchemaExample.LSchemaExampleRebuild(connection, LSchemaExample.LSchemaCarriedRead(connection));
        }

        using SqliteCommand drop = connection.CreateCommand();
        drop.CommandText =
            """
            DROP TABLE IF EXISTS example_rendition;

            DROP TABLE IF EXISTS example_translation;
            """;
        drop.ExecuteNonQuery();
    }
}
