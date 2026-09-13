using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LDatabase
{
    private readonly string _lDatabaseFile;

    private readonly object _lDatabaseGate = new();

    private LDatabaseSession? _lDatabaseSession;

    private int _lDatabaseThread;

    public LDatabase(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lDatabaseFile = LWorkspaceRoot.LWorkspaceDatabaseRead(root);
    }

    public string LDatabaseFile => _lDatabaseFile;

    public SqliteConnection LDatabaseConnectionRead()
    {
        return LDatabaseConnectionRead(_lDatabaseFile);
    }

    public static SqliteConnection LDatabaseConnectionRead(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = file,
            Mode = SqliteOpenMode.ReadWriteCreate,
        };

        SqliteConnection connection = new(builder.ConnectionString);
        try
        {
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
            connection.CreateFunction<string?, string?, bool>(
                "lmatch",
                (text, query) => LCatalog.LCatalogTextMatch(text, query ?? string.Empty),
                isDeterministic: true);
        }
        catch
        {
            connection.Dispose();
            throw;
        }

        return connection;
    }

    public LDatabaseSession LDatabaseSessionStart()
    {
        lock (_lDatabaseGate)
        {
            if (_lDatabaseSession is not null)
            {
                if (_lDatabaseThread != Environment.CurrentManagedThreadId)
                {
                    throw new InvalidOperationException(
                        "This database already has a session open on another thread.");
                }

                return new LDatabaseSession(_lDatabaseSession);
            }

            LDatabaseSession session = new(this, LDatabaseConnectionRead());
            _lDatabaseSession = session;
            _lDatabaseThread = Environment.CurrentManagedThreadId;
            return session;
        }
    }

    public void LDatabaseCreate()
    {
        bool stale;
        using (SqliteConnection connection = LDatabaseConnectionRead())
        {
            stale = LSchemaMigration.LSchemaMigrationCheck(connection);
        }

        if (stale)
        {
            LSchemaMigration.LSchemaMigrationRun(_lDatabaseFile);
        }

        using SqliteConnection fresh = LDatabaseConnectionRead();
        LSchema.LSchemaCreate(fresh);
    }

    internal void LDatabaseSessionClear(LDatabaseSession session)
    {
        lock (_lDatabaseGate)
        {
            if (ReferenceEquals(_lDatabaseSession, session))
            {
                _lDatabaseSession = null;
                _lDatabaseThread = 0;
            }
        }
    }
}
