using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaPosition
{
    public static void LSchemaPositionNormalize(SqliteConnection connection, string table, string ownerColumn)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            DROP TABLE IF EXISTS temp.schema_position;

            CREATE TEMP TABLE schema_position AS
                SELECT id,
                       ROW_NUMBER() OVER (PARTITION BY {ownerColumn} ORDER BY position, id) - 1 AS position
                FROM {table};

            UPDATE {table} SET position =
                (SELECT position FROM temp.schema_position WHERE temp.schema_position.id = {table}.id);

            DROP TABLE temp.schema_position;
            """;
        command.ExecuteNonQuery();
    }
}
