using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaWorkspace
{
    private static readonly string[] LSchemaWorkspaceColumn =
    [
        "library_order",
        "phonology_order",
        "favorite_order",
        "taxonomy_order",
        "repertoire_order",
        "reference_order",
        "corpus_order",
    ];

    public static void LSchemaWorkspaceNormalize(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        foreach (string column in LSchemaWorkspaceColumn)
        {
            if (LSchemaWorkspaceFind(connection, column))
            {
                continue;
            }

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"ALTER TABLE workspace ADD COLUMN {column} TEXT;";
            command.ExecuteNonQuery();
        }

        using (SqliteCommand split = connection.CreateCommand())
        {
            split.CommandText =
                """
                UPDATE workspace
                SET split = NULL
                WHERE split IS NOT NULL AND split NOT IN ('Editor', 'Display');
                """;
            split.ExecuteNonQuery();
        }
    }

    private static bool LSchemaWorkspaceFind(SqliteConnection connection, string column)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM pragma_table_info('workspace') WHERE name = $column;";
        command.Parameters.AddWithValue("$column", column);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }
}
