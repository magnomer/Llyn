using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaCollocation
{
    public const string LSchemaCollocationSequence =
        """
        (SELECT MAX(value) + 1 FROM (
            SELECT seq AS value FROM sqlite_sequence WHERE name IN ('sense', 'collocation')
            UNION ALL SELECT MAX(sense_id) FROM sense
            UNION ALL SELECT MAX(collocation_id) FROM collocation
            UNION ALL SELECT 0))
        """;

    public static void LSchemaCollocationCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS collocation (
                collocation_id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                title_state TEXT NOT NULL DEFAULT 'unspecified',
                title TEXT,
                expression_state TEXT NOT NULL DEFAULT 'unspecified',
                expression TEXT,
                meaning_state TEXT NOT NULL DEFAULT 'unspecified',
                meaning TEXT,
                CHECK (title_state = 'specified' OR title IS NULL),
                CHECK (expression_state = 'specified' OR expression IS NULL),
                CHECK (meaning_state = 'specified' OR meaning IS NULL),
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS note (
                entry_parent INTEGER NOT NULL PRIMARY KEY,
                text TEXT NOT NULL,
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }

    public static void LSchemaCollocationSettle(SqliteConnection connection, string into)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(into);

        List<long> shared = [];
        using (SqliteCommand read = connection.CreateCommand())
        {
            read.CommandText =
                $"""
                SELECT collocation_id FROM {into}.collocation
                WHERE collocation_id IN (SELECT sense_id FROM {into}.sense)
                ORDER BY collocation_id;
                """;
            using SqliteDataReader reader = read.ExecuteReader();
            while (reader.Read())
            {
                shared.Add(reader.GetInt64(0));
            }
        }

        if (shared.Count == 0)
        {
            return;
        }

        long next;
        using (SqliteCommand floor = connection.CreateCommand())
        {
            floor.CommandText =
                $"""
                SELECT MAX(value) + 1 FROM (
                    SELECT seq AS value FROM {into}.sqlite_sequence WHERE name IN ('sense', 'collocation')
                    UNION ALL SELECT MAX(sense_id) FROM {into}.sense
                    UNION ALL SELECT MAX(collocation_id) FROM {into}.collocation);
                """;
            next = Convert.ToInt64(floor.ExecuteScalar());
        }

        IReadOnlyList<(string LSchemaTable, string LSchemaColumn)> links = LSchemaLinkRead(connection, into);
        foreach (long old in shared)
        {
            foreach ((string table, string column) in links)
            {
                LSchemaLinkUpdate(connection, $"{into}.\"{table}\"", column, old, next, null);
            }

            LSchemaLinkUpdate(connection, $"{into}.revision_change", "target_ref", old, next, "collocation");
            LSchemaLinkUpdate(connection, $"{into}.collocation", "collocation_id", old, next, null);
            next++;
        }

        using SqliteCommand sequence = connection.CreateCommand();
        sequence.CommandText =
            $"""
            UPDATE {into}.sqlite_sequence SET seq = $seq WHERE name = 'collocation';
            INSERT INTO {into}.sqlite_sequence (name, seq)
            SELECT 'collocation', $seq WHERE NOT EXISTS (
                SELECT 1 FROM {into}.sqlite_sequence WHERE name = 'collocation');
            """;
        sequence.Parameters.AddWithValue("$seq", next - 1);
        sequence.ExecuteNonQuery();
    }

    private static IReadOnlyList<(string LSchemaTable, string LSchemaColumn)> LSchemaLinkRead(
        SqliteConnection connection, string schema)
    {
        List<(string LSchemaTable, string LSchemaColumn)> links = [];
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            SELECT master.name, link."from"
            FROM {schema}.sqlite_master AS master
            JOIN pragma_foreign_key_list(master.name, '{schema}') AS link
            WHERE master.type = 'table' AND link."table" = 'collocation';
            """;
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            links.Add((reader.GetString(0), reader.GetString(1)));
        }

        return links;
    }

    private static void LSchemaLinkUpdate(
        SqliteConnection connection, string table, string column, long old, long next, string? kind)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = kind is null
            ? $"UPDATE {table} SET \"{column}\" = $next WHERE \"{column}\" = $old;"
            : $"UPDATE {table} SET \"{column}\" = $next WHERE \"{column}\" = $old AND target_type = $kind;";
        command.Parameters.AddWithValue("$next", next);
        command.Parameters.AddWithValue("$old", old);
        if (kind is not null)
        {
            command.Parameters.AddWithValue("$kind", kind);
        }

        command.ExecuteNonQuery();
    }
}
