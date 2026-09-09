using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaSentence
{
    public static void LSchemaSentenceNormalize(
        SqliteConnection connection, string table, string column, string owner)
    {
        if (LSchemaProbe.LSchemaColumnFind(connection, table, "id"))
        {
            return;
        }

        using (SqliteCommand off = connection.CreateCommand())
        {
            off.CommandText = "PRAGMA foreign_keys = OFF;";
            off.ExecuteNonQuery();
        }

        try
        {
            LSchemaSentenceRebuild(connection, table, column, owner);
        }
        finally
        {
            using SqliteCommand on = connection.CreateCommand();
            on.CommandText = "PRAGMA foreign_keys = ON;";
            on.ExecuteNonQuery();
        }
    }

    public static void LSchemaSentenceRebuild(
        SqliteConnection connection, string table, string column, string owner)
    {
        using SqliteTransaction rebuild = connection.BeginTransaction();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            ALTER TABLE {table} RENAME TO {table}_carried;

            DROP INDEX IF EXISTS {table}_position;

            DROP INDEX IF EXISTS {table}_member;

            CREATE TABLE {table} (
                id TEXT NOT NULL PRIMARY KEY,
                {column} TEXT NOT NULL,
                example_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                particle_state TEXT NOT NULL DEFAULT 'unspecified',
                particle TEXT,
                dependence_state TEXT NOT NULL DEFAULT 'unspecified',
                dependence TEXT,
                CHECK (particle_state = 'specified' OR particle IS NULL),
                CHECK (dependence_state = 'specified' OR dependence IS NULL),
                FOREIGN KEY ({column}) REFERENCES {owner} (id) ON DELETE CASCADE,
                FOREIGN KEY (example_id) REFERENCES example (id)
            );

            INSERT INTO {table} (id, {column}, example_id, position)
            SELECT lower(hex(randomblob(6))), {column}, example_id, position FROM {table}_carried;

            DROP TABLE {table}_carried;

            CREATE UNIQUE INDEX IF NOT EXISTS {table}_position
                ON {table} ({column}, position);
            """;
        command.ExecuteNonQuery();
        rebuild.Commit();
    }
}
