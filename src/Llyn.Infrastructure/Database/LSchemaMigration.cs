using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaMigration
{
    public const long LSchemaMigrationVersion = 48;

    private const string LSchemaMigrationFresh = "fresh";

    private const string LSchemaMigrationTable = "schema_version";

    private const string LSchemaMigrationRealm = "realm";

    private const string LSchemaMigrationSequence = "sqlite_sequence";

    private static readonly string[] LSchemaMigrationCompanion = ["", "-wal", "-shm"];

    public static void LSchemaMigrationApply(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (!LSchemaVersion.LSchemaVersionExist(connection))
        {
            LSchemaVersion.LSchemaVersionCreate(connection);
            return;
        }

        long stored = LSchemaVersion.LSchemaVersionRead(connection);
        if (stored != LSchemaMigrationVersion)
        {
            throw new InvalidOperationException(
                $"The workspace database is at schema version {stored}, not the version " +
                $"{LSchemaMigrationVersion} this build produces, and was not migrated before the schema ran.");
        }
    }

    public static bool LSchemaMigrationCheck(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        return LSchemaVersion.LSchemaVersionExist(connection)
            && LSchemaVersion.LSchemaVersionRead(connection) != LSchemaMigrationVersion;
    }

    public static string LSchemaMigrationRun(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        string fresh = file + "." + LSchemaMigrationFresh;
        SqliteConnection.ClearAllPools();
        LSchemaMigrationDelete(fresh);

        try
        {
            return LSchemaFileApply(file, fresh);
        }
        catch
        {
            SqliteConnection.ClearAllPools();
            LSchemaMigrationDelete(fresh);
            throw;
        }
    }

    private static string LSchemaFileApply(string file, string fresh)
    {
        using (SqliteConnection target = LDatabase.LDatabaseConnectionRead(fresh))
        {
            LSchema.LSchemaCreate(target);
        }

        long stored;
        using (SqliteConnection source = LDatabase.LDatabaseConnectionRead(file))
        {
            stored = LSchemaVersion.LSchemaVersionRead(source);

            using (SqliteCommand attach = source.CreateCommand())
            {
                attach.CommandText = $"PRAGMA foreign_keys = OFF; ATTACH DATABASE $fresh AS {LSchemaMigrationFresh};";
                attach.Parameters.AddWithValue("$fresh", fresh);
                attach.ExecuteNonQuery();
            }

            using (SqliteTransaction transaction = source.BeginTransaction())
            {
                foreach (string table in LSchemaTableRead(source, LSchemaMigrationFresh))
                {
                    LSchemaTableApply(source, table);
                }

                LSchemaOrphanSweep(source);
                transaction.Commit();
            }

            using SqliteCommand detach = source.CreateCommand();
            detach.CommandText = $"DETACH DATABASE {LSchemaMigrationFresh};";
            detach.ExecuteNonQuery();
        }

        SqliteConnection.ClearAllPools();

        string backup = LSchemaBackupResolve(file, stored);
        foreach (string companion in LSchemaMigrationCompanion)
        {
            if (File.Exists(file + companion))
            {
                File.Move(file + companion, backup + companion);
            }
        }

        File.Move(fresh, file);
        LSchemaMigrationDelete(fresh);

        return backup;
    }

    private static void LSchemaTableApply(SqliteConnection connection, string table)
    {
        if (string.Equals(table, LSchemaMigrationTable, StringComparison.Ordinal))
        {
            return;
        }

        List<string> shared = [];
        HashSet<string> held = new(LSchemaColumnRead(connection, "main", table), StringComparer.Ordinal);
        foreach (string column in LSchemaColumnRead(connection, LSchemaMigrationFresh, table))
        {
            if (held.Contains(column))
            {
                shared.Add($"\"{column}\"");
            }
        }

        if (shared.Count == 0)
        {
            return;
        }

        string columns = string.Join(", ", shared);
        string clause = table switch
        {
            LSchemaMigrationRealm => "INSERT OR REPLACE",
            LSchemaMigrationSequence => "INSERT",
            _ => "INSERT OR IGNORE",
        };

        if (string.Equals(table, LSchemaMigrationSequence, StringComparison.Ordinal))
        {
            using SqliteCommand clear = connection.CreateCommand();
            clear.CommandText = $"DELETE FROM {LSchemaMigrationFresh}.\"{table}\";";
            clear.ExecuteNonQuery();
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"{clause} INTO {LSchemaMigrationFresh}.\"{table}\" ({columns}) SELECT {columns} FROM main.\"{table}\";";
        command.ExecuteNonQuery();
    }

    private static void LSchemaOrphanSweep(SqliteConnection connection)
    {
        while (true)
        {
            List<(string LSchemaTable, long LSchemaRow)> orphans = [];
            using (SqliteCommand check = connection.CreateCommand())
            {
                check.CommandText = $"PRAGMA {LSchemaMigrationFresh}.foreign_key_check;";
                using SqliteDataReader reader = check.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(1))
                    {
                        orphans.Add((reader.GetString(0), reader.GetInt64(1)));
                    }
                }
            }

            if (orphans.Count == 0)
            {
                return;
            }

            foreach ((string table, long row) in orphans)
            {
                using SqliteCommand delete = connection.CreateCommand();
                delete.CommandText = $"DELETE FROM {LSchemaMigrationFresh}.\"{table}\" WHERE rowid = $row;";
                delete.Parameters.AddWithValue("$row", row);
                delete.ExecuteNonQuery();
            }
        }
    }

    private static List<string> LSchemaTableRead(SqliteConnection connection, string schema)
    {
        List<string> tables = [];
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            SELECT name FROM {schema}.sqlite_master
            WHERE type = 'table' AND (name NOT LIKE 'sqlite_%' OR name = '{LSchemaMigrationSequence}')
              AND name IN (SELECT name FROM main.sqlite_master WHERE type = 'table')
            ORDER BY rowid;
            """;
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            tables.Add(reader.GetString(0));
        }

        return tables;
    }

    private static List<string> LSchemaColumnRead(SqliteConnection connection, string schema, string table)
    {
        List<string> columns = [];
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"PRAGMA {schema}.table_info(\"{table}\");";
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            columns.Add(reader.GetString(1));
        }

        return columns;
    }

    private static string LSchemaBackupResolve(string file, long stored)
    {
        string folder = Path.GetDirectoryName(file) ?? string.Empty;
        string name = Path.GetFileNameWithoutExtension(file);
        string extension = Path.GetExtension(file);
        string stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        for (int round = 0; ; round++)
        {
            string mark = round == 0 ? $"v{stored}" : $"v{stored}-{round}";
            string candidate = Path.Combine(folder, $"{name}.{stamp}.{mark}{extension}");
            if (!File.Exists(candidate))
            {
                return candidate;
            }
        }
    }

    private static void LSchemaMigrationDelete(string fresh)
    {
        foreach (string companion in LSchemaMigrationCompanion)
        {
            if (File.Exists(fresh + companion))
            {
                File.Delete(fresh + companion);
            }
        }
    }
}
