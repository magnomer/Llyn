using Llyn.Core;
using Llyn.Infrastructure;
using Microsoft.Data.Sqlite;
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
    public void DoctorDatabaseCreate_NewerBuild_StartsCleanDatabase()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        workspace.TWorkspaceScriptRun(
            $"UPDATE schema_version SET version = {LSchemaMigration.LSchemaMigrationVersion + 5};");

        LDoctorRescue rescue = TInterface.TDoctorDatabaseCreate(workspace.TWorkspaceDatabase);

        Assert.True(rescue.LDoctorRescueDone);
        Assert.NotNull(rescue.LDoctorRescueBackup);
        Assert.True(File.Exists(rescue.LDoctorRescueBackup));
        Assert.Equal(
            LSchemaMigration.LSchemaMigrationVersion,
            workspace.TWorkspaceCountRead("SELECT version FROM schema_version;"));
    }

    [Fact]
    public void DoctorDatabaseCreate_UnreadableFile_StartsCleanDatabase()
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

    private static string TDoctorFileRead(TWorkspace workspace)
    {
        return Path.Combine(workspace.TWorkspaceFolder, "llyn.db");
    }

    private static string[] TDoctorBackupRead(TWorkspace workspace)
    {
        return Directory.GetFiles(workspace.TWorkspaceFolder, "*.broken*.db");
    }
}
