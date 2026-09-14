using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaCitation
{
    public const string LSchemaCitationTitle = "Unknown";

    public static long LSchemaCitationCreate(SqliteConnection connection, string schema)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            INSERT INTO {schema}.reference (title_state, title)
            VALUES ('specified', $title)
            RETURNING reference_id;
            """;
        command.Parameters.AddWithValue("$title", LSchemaCitationTitle);
        return (long)command.ExecuteScalar()!;
    }

    public static void LSchemaCitationSettle(SqliteConnection connection, string schema)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);

        if (!LSchemaCitationCheck(connection, schema))
        {
            return;
        }

        long reference = LSchemaCitationFind(connection, schema) ?? LSchemaCitationCreate(connection, schema);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            UPDATE {schema}.example
            SET reference_state = 'specified', reference_ref = $reference
            WHERE reference_state = 'unknown';
            """;
        command.Parameters.AddWithValue("$reference", reference);
        command.ExecuteNonQuery();
    }

    private static bool LSchemaCitationCheck(SqliteConnection connection, string schema)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {schema}.example WHERE reference_state = 'unknown';";
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    private static long? LSchemaCitationFind(SqliteConnection connection, string schema)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            SELECT reference_id FROM {schema}.reference
            WHERE title_state = 'specified' AND title = $title
            ORDER BY reference_id
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$title", LSchemaCitationTitle);
        object? found = command.ExecuteScalar();
        return found is long id ? id : null;
    }
}
