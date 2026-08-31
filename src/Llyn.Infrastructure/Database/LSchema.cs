using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Creates the database schema idempotently and records its version. Job01 defines only the runner
/// and the version row; each later job adds its own <c>CREATE TABLE IF NOT EXISTS</c> statements
/// here (or in a per-area file it owns) so the schema grows one job at a time without ever dropping
/// what an earlier job built.
/// </summary>
public static class LSchema
{
    /// <summary>The schema version this build produces. Later jobs raise it as they extend the schema.</summary>
    private const long LSchemaVersion = 1;

    /// <summary>
    /// Creates every table that does not yet exist and stamps the schema version. Safe to run on each
    /// startup: existing tables and an existing version row are left as they are.
    /// </summary>
    public static void LSchemaCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        // Data-contract names (table and column identifiers) are persisted keys, so they stay
        // lowercase and independent of code member names.
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS schema_version (
                version INTEGER NOT NULL
            );
            """;
        command.ExecuteNonQuery();

        command.CommandText = "SELECT COUNT(*) FROM schema_version;";
        long rows = Convert.ToInt64(command.ExecuteScalar());
        if (rows == 0)
        {
            command.CommandText = "INSERT INTO schema_version (version) VALUES ($version);";
            command.Parameters.AddWithValue("$version", LSchemaVersion);
            command.ExecuteNonQuery();
        }
    }
}
