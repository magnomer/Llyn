using Llyn.Infrastructure;
using Microsoft.Data.Sqlite;

namespace Llyn.Database.Tests;

/// <summary>
/// A throwaway workspace folder with its own <c>llyn.db</c>, so every test runs against a database
/// nothing else has touched. Disposing removes the folder; the connection pool is cleared first, since
/// a pooled connection would otherwise keep the file open and the delete would fail on Windows.
/// </summary>
internal sealed class TWorkspace : IDisposable
{
    private readonly string _tWorkspaceRoot;

    private TWorkspace(string root)
    {
        _tWorkspaceRoot = root;
        TWorkspaceDatabase = new LDatabase(root);
    }

    /// <summary>The database bound to this workspace folder.</summary>
    public LDatabase TWorkspaceDatabase { get; }

    /// <summary>The folder itself, for a test that has to reach the file directly.</summary>
    public string TWorkspaceFolder => _tWorkspaceRoot;

    /// <summary>Creates an empty workspace folder whose database has not been initialized yet.</summary>
    public static TWorkspace TWorkspaceCreate()
    {
        string root = Path.Combine(Path.GetTempPath(), "llyn-test-" + Guid.NewGuid().ToString("n"));
        Directory.CreateDirectory(root);
        return new TWorkspace(root);
    }

    /// <summary>Creates a workspace folder whose database is initialized and ready to use.</summary>
    public static TWorkspace TWorkspacePrepare()
    {
        TWorkspace workspace = TWorkspaceCreate();
        workspace.TWorkspaceDatabase.LDatabaseCreate();
        return workspace;
    }

    /// <summary>Opens a raw connection to the workspace database, bypassing the store layer.</summary>
    public SqliteConnection TWorkspaceConnectionRead()
    {
        return TWorkspaceDatabase.LDatabaseConnectionRead();
    }

    /// <summary>Runs one statement against the workspace database and returns its first column.</summary>
    public long TWorkspaceCountRead(string sql)
    {
        using SqliteConnection connection = TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        return Convert.ToInt64(command.ExecuteScalar());
    }

    /// <summary>
    /// Runs one statement and returns its first column for every row, for a test that has to see a
    /// column the store layer does not hand out — an association's position, say.
    /// </summary>
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

    /// <summary>Runs statements that set the database up in a shape the store layer would not produce.</summary>
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
            // A file the operating system has not released yet is not a test failure.
        }
    }
}
