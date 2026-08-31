using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Owns the SQLite database that lives as <c>llyn.db</c> inside the user's workspace folder — the
/// only place it ever lives (<see cref="LWorkspaceRoot.LWorkspaceDatabaseRead"/>). Hands out ready
/// connections with <c>PRAGMA foreign_keys = ON</c> already applied, and initializes the file's
/// schema on first use. Every later data store opens its connections through here so no code reaches
/// past the workspace for the database.
/// </summary>
public sealed class LDatabase
{
    private readonly string _lDatabaseFile;

    /// <summary>
    /// Binds this database to <paramref name="root"/>, the resolved workspace folder that owns the
    /// database file. The file is not opened until <see cref="LDatabaseRead"/> or
    /// <see cref="LDatabaseCreate"/> is called.
    /// </summary>
    public LDatabase(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lDatabaseFile = LWorkspaceRoot.LWorkspaceDatabaseRead(root);
    }

    /// <summary>
    /// Opens the database (creating the file if absent), enforces foreign keys on the connection, and
    /// returns it open and ready to use. The caller owns the connection and disposes it.
    /// </summary>
    public SqliteConnection LDatabaseRead()
    {
        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = _lDatabaseFile,
            Mode = SqliteOpenMode.ReadWriteCreate,
        };

        SqliteConnection connection = new(builder.ConnectionString);
        connection.Open();

        using SqliteCommand pragma = connection.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON;";
        pragma.ExecuteNonQuery();

        return connection;
    }

    /// <summary>
    /// Initializes the database on first use: opens a connection and runs the schema so the file and
    /// its tables exist. Idempotent — running it against an already-initialized database is a no-op.
    /// </summary>
    public void LDatabaseCreate()
    {
        using SqliteConnection connection = LDatabaseRead();
        LSchema.LSchemaCreate(connection);
    }
}
