using System;
using System.Globalization;
using System.IO;
using Llyn.Core;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Llyn.Infrastructure;

public sealed class LDoctor : LDoctorVault
{
    private const string LDoctorMark = "broken";

    private static readonly string[] LDoctorCompanion = ["", "-wal", "-shm"];

    private readonly LDatabase _lDoctorDatabase;

    public LDoctor(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lDoctorDatabase = database;
    }

    public LDoctorRescue LDoctorDatabaseCreate()
    {
        try
        {
            _lDoctorDatabase.LDatabaseCreate();
            return LDoctorRescue.LDoctorRescueHealthy;
        }
        catch (SqliteException fault) when (LDoctorRescueCheck(fault))
        {
            string backup = LDoctorDatabaseSave(_lDoctorDatabase.LDatabaseFile);
            _lDoctorDatabase.LDatabaseCreate();
            return new LDoctorRescue(true, backup, fault.Message);
        }
    }

    public static bool LDoctorRescueCheck(Exception fault)
    {
        ArgumentNullException.ThrowIfNull(fault);

        return fault is SqliteException { SqliteErrorCode: raw.SQLITE_CORRUPT or raw.SQLITE_NOTADB };
    }

    public static bool LDoctorBusyCheck(Exception fault)
    {
        ArgumentNullException.ThrowIfNull(fault);

        return fault is SqliteException { SqliteErrorCode: raw.SQLITE_BUSY or raw.SQLITE_LOCKED };
    }

    private static string LDoctorDatabaseSave(string file)
    {
        string backup = LDoctorBackupResolve(file);

        SqliteConnection.ClearAllPools();

        foreach (string companion in LDoctorCompanion)
        {
            string source = file + companion;
            if (!File.Exists(source))
            {
                continue;
            }

            File.Move(source, backup + companion);
        }

        return backup;
    }

    private static string LDoctorBackupResolve(string file)
    {
        string folder = Path.GetDirectoryName(file) ?? string.Empty;
        string name = Path.GetFileNameWithoutExtension(file);
        string extension = Path.GetExtension(file);
        string stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        for (int round = 0; ; round++)
        {
            string mark = round == 0 ? LDoctorMark : $"{LDoctorMark}{round}";
            string candidate = Path.Combine(folder, $"{name}.{stamp}.{mark}{extension}");
            if (!File.Exists(candidate))
            {
                return candidate;
            }
        }
    }
}
