using System.Net;
using System.Net.Http;
using Llyn.Core;
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

    public string TWorkspaceMarkupSave(string text)
    {
        string path = Path.Combine(_tWorkspaceRoot, "import.llx");
        File.WriteAllText(path, text);
        return path;
    }

    public LEngine TWorkspaceEngineStart()
    {
        return TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(string.Empty, HttpStatusCode.NotFound));
    }

    public LEngine TWorkspaceEngineStart(HttpClient client)
    {
        return new LEngine(LRigFactory.LRigFactoryBuild(_tWorkspaceRoot, client) with { LRigClock = new TClockFake() });
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

    public IReadOnlyList<string> TWorkspaceRowRead(string sql)
    {
        using SqliteConnection connection = TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        List<string> rows = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            string[] fields = new string[reader.FieldCount];
            for (int index = 0; index < reader.FieldCount; index++)
            {
                fields[index] = reader.IsDBNull(index)
                    ? "\0"
                    : reader.GetValue(index).ToString() ?? string.Empty;
            }

            rows.Add(string.Join('\u001F', fields));
        }

        return rows;
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
