using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LDatabase
{
    private readonly string _lDatabaseFile;

    private LDatabaseSession? _lDatabaseSession;

    public LDatabase(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lDatabaseFile = LWorkspaceRoot.LWorkspaceDatabaseRead(root);
    }

    public SqliteConnection LDatabaseConnectionRead()
    {
        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = _lDatabaseFile,
            Mode = SqliteOpenMode.ReadWriteCreate,
        };

        SqliteConnection connection = new(builder.ConnectionString);
        connection.Open();

        using SqliteCommand pragma = connection.CreateCommand();
        pragma.CommandText =
            """
            PRAGMA journal_mode = WAL;
            PRAGMA busy_timeout = 5000;
            PRAGMA synchronous = NORMAL;
            PRAGMA foreign_keys = ON;
            """;
        pragma.ExecuteNonQuery();

        connection.CreateFunction<string?, string?>(
            "lfold", text => text?.ToLowerInvariant(), isDeterministic: true);

        return connection;
    }

    public LDatabaseSession LDatabaseSessionStart()
    {
        if (_lDatabaseSession is not null)
        {
            return new LDatabaseSession(_lDatabaseSession);
        }

        LDatabaseSession session = new(this, LDatabaseConnectionRead());
        _lDatabaseSession = session;
        return session;
    }

    public void LDatabaseCreate()
    {
        using SqliteConnection connection = LDatabaseConnectionRead();
        LSchema.LSchemaCreate(connection);
    }

    internal void LDatabaseSessionClear(LDatabaseSession session)
    {
        if (ReferenceEquals(_lDatabaseSession, session))
        {
            _lDatabaseSession = null;
        }
    }
}
