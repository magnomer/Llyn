using System;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaShortcut
{
    public static void LSchemaShortcutRemove(SqliteConnection connection)
    {
        if (LSchemaProbe.LSchemaTableFind(connection, "entry_source"))
        {
            LSchemaShortcutRecord(connection);

            using SqliteCommand dropped = connection.CreateCommand();
            dropped.CommandText =
                """
                DROP INDEX IF EXISTS entry_source_position;
                DROP INDEX IF EXISTS entry_source_member;
                DROP TABLE IF EXISTS entry_source;
                """;
            dropped.ExecuteNonQuery();
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            DROP INDEX IF EXISTS entry_example_position;
            DROP INDEX IF EXISTS entry_example_member;
            DROP TABLE IF EXISTS entry_example;
            """;
        command.ExecuteNonQuery();
    }

    public static void LSchemaShortcutRecord(SqliteConnection connection)
    {
        long lost;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                SELECT COUNT(*) FROM entry_source link
                WHERE NOT EXISTS (
                    SELECT 1 FROM sense_example card
                    JOIN sense ON sense.id = card.sense_id
                    JOIN example ON example.id = card.example_id
                    WHERE sense.entry_id = link.entry_id
                      AND example.source_id = link.source_id)
                  AND NOT EXISTS (
                    SELECT 1 FROM collocation_example card
                    JOIN collocation ON collocation.id = card.collocation_id
                    JOIN example ON example.id = card.example_id
                    WHERE collocation.entry_id = link.entry_id
                      AND example.source_id = link.source_id);
                """;
            lost = Convert.ToInt64(command.ExecuteScalar());
        }

        if (lost == 0)
        {
            return;
        }

        string? root = Path.GetDirectoryName(connection.DataSource);
        if (string.IsNullOrWhiteSpace(root))
        {
            return;
        }

        LAuditWriter.LAuditWriterRecord(
            root,
            string.Format(
                CultureInfo.InvariantCulture,
                "Schema revision 32 dropped entry_source. {0} entry citation(s) named a source no example under that entry cites and were not carried over.",
                lost));
    }
}
