using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaVersion
{
    public static bool LSchemaVersionExist(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'schema_version';";
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    public static void LSchemaVersionCreate(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );

            INSERT INTO schema_version (id, version) VALUES (1, $version);
            """;
        command.Parameters.AddWithValue("$version", LSchemaMigration.LSchemaMigrationVersion);
        command.ExecuteNonQuery();
    }

    public static long LSchemaVersionRead(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT ifnull(MAX(version), 0) FROM schema_version;";
        return Convert.ToInt64(command.ExecuteScalar());
    }

    public static void LSchemaVersionNormalize(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            DROP TABLE IF EXISTS schema_version_next;

            CREATE TABLE schema_version_next (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );

            INSERT INTO schema_version_next (id, version) VALUES (1, $version);

            DROP TABLE schema_version;

            ALTER TABLE schema_version_next RENAME TO schema_version;
            """;
        command.Parameters.AddWithValue("$version", LSchemaMigration.LSchemaMigrationVersion);
        command.ExecuteNonQuery();
    }

    public static void LSchemaVersionSave(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE schema_version SET version = $version;";
        command.Parameters.AddWithValue("$version", LSchemaMigration.LSchemaMigrationVersion);
        command.ExecuteNonQuery();
    }
}
