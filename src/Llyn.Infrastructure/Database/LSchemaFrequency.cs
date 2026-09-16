using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaFrequency
{
    private const string LSchemaFrequencyColumn = "frequency";

    public static void LSchemaFrequencyCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS frequency (
                entry_parent INTEGER NOT NULL,
                source TEXT NOT NULL,
                raw TEXT NOT NULL,
                band TEXT,
                fetched_utc TEXT NOT NULL,
                PRIMARY KEY (entry_parent, source),
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }

    public static void LSchemaFrequencySettle(SqliteConnection connection, string into, string from)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(into);
        ArgumentException.ThrowIfNullOrWhiteSpace(from);

        if (!LSchemaFrequencyCheck(connection, from))
        {
            return;
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            INSERT OR IGNORE INTO {into}.frequency (entry_parent, source, raw, band, fetched_utc)
            SELECT entry_id,
                   substr(frequency, 1, instr(frequency, '|') - 1),
                   substr(frequency, instr(frequency, '|') + 1),
                   NULL,
                   ifnull(updated_utc, '')
            FROM {from}.entry
            WHERE frequency IS NOT NULL AND instr(frequency, '|') > 1;
            """;
        command.ExecuteNonQuery();
    }

    private static bool LSchemaFrequencyCheck(SqliteConnection connection, string schema)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"SELECT COUNT(*) FROM {schema}.pragma_table_info('entry') WHERE name = $column;";
        command.Parameters.AddWithValue("$column", LSchemaFrequencyColumn);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }
}
