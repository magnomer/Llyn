using Llyn.Core;
using Llyn.Infrastructure;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using Xunit;

namespace Llyn.Tests;

public sealed class TDoctor
{
    [Fact]
    public void DoctorDatabaseCreate_HealthyDatabase_RescuesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();

        LDoctorRescue rescue = TInterface.TDoctorDatabaseCreate(workspace.TWorkspaceDatabase);

        Assert.False(rescue.LDoctorRescueDone);
        Assert.Null(rescue.LDoctorRescueBackup);
        Assert.Null(rescue.LDoctorRescueReason);
        Assert.Empty(TDoctorBackupRead(workspace));
    }

    [Fact]
    public void DoctorDatabaseCreate_OtherVersion_RebuildsWithoutRescue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        workspace.TWorkspaceScriptRun(
            $"UPDATE schema_version SET version = {LSchemaMigration.LSchemaMigrationVersion + 5};");

        LDoctorRescue rescue = TInterface.TDoctorDatabaseCreate(workspace.TWorkspaceDatabase);

        Assert.False(rescue.LDoctorRescueDone);
        Assert.Empty(TDoctorBackupRead(workspace));
        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
    }

    [Fact]
    public void DoctorDatabaseCreate_UnknownFile_StartsCleanDatabase()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(TDoctorFileRead(workspace), "this is not a database");

        LDoctorRescue rescue = TInterface.TDoctorDatabaseCreate(workspace.TWorkspaceDatabase);

        Assert.True(rescue.LDoctorRescueDone);
        Assert.Equal("this is not a database", File.ReadAllText(rescue.LDoctorRescueBackup!));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM schema_version;"));
    }

    [Fact]
    public void DoctorDatabaseCreate_RescuedTwice_KeepsEveryBackup()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        for (int round = 0; round < 2; round++)
        {
            SqliteConnection.ClearAllPools();
            File.WriteAllText(TDoctorFileRead(workspace), $"broken {round}");
            Assert.True(TInterface.TDoctorDatabaseCreate(workspace.TWorkspaceDatabase).LDoctorRescueDone);
        }

        Assert.Equal(2, TDoctorBackupRead(workspace).Length);
    }

    [Fact]
    public void DoctorDatabaseCreate_FileCannotOpen_RethrowsWithoutRescue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        Directory.CreateDirectory(TDoctorFileRead(workspace));

        SqliteException fault = Assert.Throws<SqliteException>(
            () => TInterface.TDoctorDatabaseCreate(workspace.TWorkspaceDatabase));

        Assert.Equal(raw.SQLITE_CANTOPEN, fault.SqliteErrorCode);
        Assert.Empty(TDoctorBackupRead(workspace));
    }

    [Fact]
    public void DoctorRescueCheck_BusyOrFull_RefusesToRescue()
    {
        Assert.False(TInterface.TDoctorRescueCheck(new SqliteException("busy", raw.SQLITE_BUSY)));
        Assert.False(TInterface.TDoctorRescueCheck(new SqliteException("full", raw.SQLITE_FULL)));
        Assert.False(TInterface.TDoctorRescueCheck(new SqliteException("io", raw.SQLITE_IOERR)));
        Assert.False(TInterface.TDoctorRescueCheck(new InvalidOperationException("version")));
        Assert.True(TInterface.TDoctorRescueCheck(new SqliteException("corrupt", raw.SQLITE_CORRUPT)));
        Assert.True(TInterface.TDoctorRescueCheck(new SqliteException("not a db", raw.SQLITE_NOTADB)));
    }

    [Fact]
    public void DoctorBusyCheck_BusyOrLocked_ReportsBusy()
    {
        Assert.True(TInterface.TDoctorBusyCheck(new SqliteException("busy", raw.SQLITE_BUSY)));
        Assert.True(TInterface.TDoctorBusyCheck(new SqliteException("locked", raw.SQLITE_LOCKED)));
        Assert.False(TInterface.TDoctorBusyCheck(new SqliteException("corrupt", raw.SQLITE_CORRUPT)));
        Assert.False(TInterface.TDoctorBusyCheck(new InvalidOperationException("version")));
    }

    private static string TDoctorFileRead(TWorkspace workspace)
    {
        return Path.Combine(workspace.TWorkspaceFolder, "llyn.db");
    }

    private static string[] TDoctorBackupRead(TWorkspace workspace)
    {
        return Directory.GetFiles(workspace.TWorkspaceFolder, "*.broken*.db");
    }
}
