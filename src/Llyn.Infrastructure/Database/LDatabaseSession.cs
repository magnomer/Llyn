using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// One unit of work against the workspace database: a connection and the transaction every statement
/// in the unit runs in. A store asks <see cref="LDatabase.LDatabaseSessionStart"/> for one, runs its
/// statements against <see cref="LDatabaseSessionConnection"/>, and commits; disposal without a commit
/// rolls the whole unit back.
/// <para>
/// Sessions nest. The first session opens the connection and owns it; a session started while that one
/// is open shares its connection and transaction and owns neither — its commit and its disposal do
/// nothing. So a store method that commits its own work still lands inside a caller's larger unit, and
/// an operation spanning several stores is atomic without any store knowing about it. Only the
/// outermost session decides.
/// </para>
/// </summary>
public sealed class LDatabaseSession : IDisposable
{
    private readonly LDatabase? _lDatabaseSessionOwner;
    private readonly SqliteConnection _lDatabaseSessionLink;
    private readonly SqliteTransaction? _lDatabaseSessionScope;

    private bool _lDatabaseSessionDone;

    /// <summary>Opens the outermost session: the transaction the whole unit of work runs in.</summary>
    internal LDatabaseSession(LDatabase owner, SqliteConnection connection)
    {
        _lDatabaseSessionOwner = owner;
        _lDatabaseSessionLink = connection;
        _lDatabaseSessionScope = connection.BeginTransaction();
    }

    /// <summary>Opens a nested session over <paramref name="session"/>, owning neither of its resources.</summary>
    internal LDatabaseSession(LDatabaseSession session)
    {
        _lDatabaseSessionOwner = null;
        _lDatabaseSessionLink = session._lDatabaseSessionLink;
        _lDatabaseSessionScope = null;
    }

    /// <summary>The open connection every statement of this unit of work runs on.</summary>
    public SqliteConnection LDatabaseSessionConnection => _lDatabaseSessionLink;

    /// <summary>
    /// Finalizes the unit of work. On the outermost session the transaction is committed and everything
    /// written since it started becomes visible; on a nested session nothing happens, because the
    /// outermost session has not finished yet.
    /// </summary>
    public void LDatabaseSessionCommit()
    {
        if (_lDatabaseSessionDone || _lDatabaseSessionScope is null)
        {
            return;
        }

        _lDatabaseSessionScope.Commit();
        _lDatabaseSessionDone = true;
    }

    /// <summary>
    /// Ends the session. An outermost session that was never committed rolls back, so a store method
    /// that throws part-way leaves nothing behind; a nested session releases nothing, since it owns
    /// nothing.
    /// </summary>
    public void Dispose()
    {
        if (_lDatabaseSessionScope is null)
        {
            return;
        }

        if (!_lDatabaseSessionDone)
        {
            _lDatabaseSessionScope.Rollback();
            _lDatabaseSessionDone = true;
        }

        _lDatabaseSessionScope.Dispose();
        _lDatabaseSessionLink.Dispose();
        _lDatabaseSessionOwner?.LDatabaseSessionClear(this);
    }
}
