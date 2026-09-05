using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Microsoft.Data.Sqlite;

using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Llyn.Tests;

internal sealed class TWorkspace : IDisposable
{
    private readonly string _tWorkspaceRoot;

    private TWorkspace(string root)
    {
        _tWorkspaceRoot = root;
        TWorkspaceDatabase = new LDatabase(root);
    }

    public LDatabase TWorkspaceDatabase { get; }

    public string TWorkspaceFolder => _tWorkspaceRoot;

    public static TWorkspace TWorkspaceCreate()
    {
        string root = Path.Combine(Path.GetTempPath(), "llyn-test-" + Guid.NewGuid().ToString("n"));
        Directory.CreateDirectory(root);
        return new TWorkspace(root);
    }

    public static TWorkspace TWorkspacePrepare()
    {
        TWorkspace workspace = TWorkspaceCreate();
        workspace.TWorkspaceDatabase.LDatabaseCreate();
        return workspace;
    }

    public LEngine TWorkspaceEngineStart()
    {
        return new LEngine(_tWorkspaceRoot);
    }

    public SqliteConnection TWorkspaceConnectionRead()
    {
        return TWorkspaceDatabase.LDatabaseConnectionRead();
    }

    public long TWorkspaceCountRead(string sql)
    {
        using SqliteConnection connection = TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        return Convert.ToInt64(command.ExecuteScalar());
    }

    public IReadOnlyList<long> TWorkspaceColumnRead(string sql)
    {
        using SqliteConnection connection = TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        List<long> values = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            values.Add(reader.GetInt64(0));
        }

        return values;
    }

    public void TWorkspaceScriptRun(string sql)
    {
        using SqliteConnection connection = TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        try
        {
            Directory.Delete(_tWorkspaceRoot, true);
        }
        catch (IOException)
        {
        }
    }
}
