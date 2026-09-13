using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaMigration
{
    public const long LSchemaMigrationVersion = 45;

    public static void LSchemaMigrationApply(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (!LSchemaVersion.LSchemaVersionExist(connection))
        {
            LSchemaVersion.LSchemaVersionCreate(connection);
            return;
        }

        long stored = LSchemaVersion.LSchemaVersionRead(connection);
        if (stored > LSchemaMigrationVersion)
        {
            throw new InvalidOperationException(
                $"The workspace database is at schema version {stored}, which is newer than the version " +
                $"{LSchemaMigrationVersion} this build understands. Update Llyn before opening it.");
        }

        if (stored < LSchemaMigrationVersion)
        {
            throw new InvalidOperationException(
                $"The workspace database is at schema version {stored}, which this build cannot upgrade. " +
                "No upgrade path exists before 1.0.0. Create a new workspace and import the old one.");
        }
    }
}
