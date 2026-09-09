using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaColumn
{
    public static void LSchemaTitleNormalize(SqliteConnection connection, string table)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                $"SELECT COUNT(*) FROM pragma_table_info('{table}') WHERE name = 'title';";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"ALTER TABLE {table} ADD COLUMN title TEXT;";
        command.ExecuteNonQuery();
    }

    public static void LSchemaCollocationNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                "SELECT COUNT(*) FROM pragma_table_info('collocation') WHERE name = 'meaning';";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "ALTER TABLE collocation ADD COLUMN meaning TEXT;";
        command.ExecuteNonQuery();
    }

    public static void LSchemaVideoNormalize(SqliteConnection connection)
    {
        if (LSchemaProbe.LSchemaColumnFind(connection, "video", "span"))
        {
            return;
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            ALTER TABLE video ADD COLUMN span_state TEXT NOT NULL DEFAULT 'unspecified';

            ALTER TABLE video ADD COLUMN span TEXT;
            """;
        command.ExecuteNonQuery();
    }

    public static void LSchemaSituationNormalize(SqliteConnection connection)
    {
        foreach (string column in new[] { "source_id", "source_state" })
        {
            using SqliteCommand check = connection.CreateCommand();
            check.CommandText =
                $"SELECT COUNT(*) FROM pragma_table_info('situation') WHERE name = '{column}';";
            if (Convert.ToInt64(check.ExecuteScalar()) == 0)
            {
                continue;
            }

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"ALTER TABLE situation DROP COLUMN {column};";
            command.ExecuteNonQuery();
        }
    }
}
