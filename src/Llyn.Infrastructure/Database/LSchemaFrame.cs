using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaFrame
{
    public static void LSchemaFrameNormalize(
        SqliteConnection connection, string table, string column, string owner)
    {
        using (SqliteCommand off = connection.CreateCommand())
        {
            off.CommandText = "PRAGMA foreign_keys = OFF;";
            off.ExecuteNonQuery();
        }

        try
        {
            LSchemaFrameRebuild(connection, table, column, owner);
        }
        finally
        {
            using SqliteCommand on = connection.CreateCommand();
            on.CommandText = "PRAGMA foreign_keys = ON;";
            on.ExecuteNonQuery();
        }
    }

    public static void LSchemaFrameRebuild(
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
                example_id TEXT,
                position INTEGER NOT NULL,
                particle_state TEXT NOT NULL DEFAULT 'unspecified',
                particle TEXT,
                dependence_state TEXT NOT NULL DEFAULT 'unspecified',
                dependence TEXT,
                CHECK (particle_state = 'specified' OR particle IS NULL),
                CHECK (dependence_state = 'specified' OR dependence IS NULL),
                CHECK (example_id IS NOT NULL
                       OR particle_state <> 'unspecified'
                       OR dependence_state <> 'unspecified'),
                FOREIGN KEY ({column}) REFERENCES {owner} (id) ON DELETE CASCADE,
                FOREIGN KEY (example_id) REFERENCES example (id)
            );

            INSERT INTO {table} (
                id, {column}, example_id, position,
                particle_state, particle, dependence_state, dependence)
            SELECT id, {column}, example_id, position,
                   particle_state, particle, dependence_state, dependence
            FROM {table}_carried;

            DROP TABLE {table}_carried;

            CREATE UNIQUE INDEX IF NOT EXISTS {table}_position
                ON {table} ({column}, position);
            """;
        command.ExecuteNonQuery();
        rebuild.Commit();
    }
}
