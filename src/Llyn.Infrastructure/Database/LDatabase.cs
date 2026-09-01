using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Owns the SQLite database that lives as <c>llyn.db</c> inside the user's workspace folder — the
/// only place it ever lives (<see cref="LWorkspaceRoot.LWorkspaceDatabaseRead"/>). Hands out ready
/// connections with the workspace pragmas already applied, and initializes the file's schema on first
/// use. Every later data store reaches the file through here so no code reaches past the workspace for
/// the database.
/// <para>
/// It also holds the <em>ambient session</em>: the unit of work an operation runs in. A store asks for
/// a session rather than a connection (<see cref="LDatabaseSessionStart"/>), so several stores called
/// one after another share one connection and one transaction and either all land or none do. The
/// ambient session is a plain field, so a single <see cref="LDatabase"/> instance belongs to one
/// thread — which is what the shell engine does with it.
/// </para>
/// </summary>
public sealed class LDatabase
{
    private readonly string _lDatabaseFile;

    private LDatabaseSession? _lDatabaseSession;

    /// <summary>
    /// Binds this database to <paramref name="root"/>, the resolved workspace folder that owns the
    /// database file. The file is not opened until a connection or a session is asked for.
    /// </summary>
    public LDatabase(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lDatabaseFile = LWorkspaceRoot.LWorkspaceDatabaseRead(root);
    }

    /// <summary>
    /// Opens the database (creating the file if absent), applies the workspace pragmas, and returns the
    /// connection open and ready to use. The caller owns the connection and disposes it. Stores use
    /// <see cref="LDatabaseSessionStart"/> instead, so this is for the schema runner and for tests.
    /// <para>
    /// Write-ahead logging lets a reader run while a writer holds the file; the busy timeout gives a
    /// blocked statement time to wait rather than failing at once; <c>synchronous = NORMAL</c> is the
    /// mode write-ahead logging is designed for. Foreign keys are per connection and must be enabled on
    /// every one of them.
    /// </para>
    /// </summary>
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

        // SQLite's own lower() folds ASCII only, so a query typed as "Ä" would never find a headword
        // stored as "ä" — unusable in an application whose subject is other languages. lfold() hands
        // the fold to .NET, which knows the whole of Unicode. It is registered on every connection
        // because a user-defined function lives on the connection that declared it.
        connection.CreateFunction<string?, string?>(
            "lfold", text => text?.ToLowerInvariant(), isDeterministic: true);

        return connection;
    }

    /// <summary>
    /// Starts the unit of work a store operation runs in. With no session open this opens a connection,
    /// begins a transaction, and becomes the ambient session; while one is already open this returns a
    /// nested session that shares that connection and transaction, whose commit and disposal do nothing
    /// so the outermost session alone decides whether the work lands.
    /// </summary>
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

    /// <summary>
    /// Initializes the database on first use: opens a connection and runs the schema so the file, its
    /// tables, and its indexes exist and its version is current. Idempotent — running it against an
    /// already-initialized database creates nothing and migrates only what is behind.
    /// </summary>
    public void LDatabaseCreate()
    {
        using SqliteConnection connection = LDatabaseConnectionRead();
        LSchema.LSchemaCreate(connection);
    }

    /// <summary>Clears the ambient session when the session that owns it ends.</summary>
    internal void LDatabaseSessionClear(LDatabaseSession session)
    {
        if (ReferenceEquals(_lDatabaseSession, session))
        {
            _lDatabaseSession = null;
        }
    }
}
