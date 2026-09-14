using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LDatabaseSession : IDisposable
{
    private readonly LDatabase? _lDatabaseSessionOwner;
    private readonly SqliteConnection _lDatabaseSessionLink;
    private readonly SqliteTransaction? _lDatabaseSessionScope;

    private bool _lDatabaseSessionDone;

    internal LDatabaseSession(LDatabase owner, SqliteConnection connection)
    {
        _lDatabaseSessionOwner = owner;
        _lDatabaseSessionLink = connection;
        _lDatabaseSessionScope = connection.BeginTransaction();
    }

    internal LDatabaseSession(LDatabaseSession session)
    {
        _lDatabaseSessionOwner = null;
        _lDatabaseSessionLink = session._lDatabaseSessionLink;
        _lDatabaseSessionScope = null;
    }

    public SqliteConnection LDatabaseSessionConnection => _lDatabaseSessionLink;

    public void LDatabaseSessionCommit()
    {
        if (_lDatabaseSessionDone || _lDatabaseSessionScope is null)
        {
            return;
        }

        _lDatabaseSessionScope.Commit();
        _lDatabaseSessionDone = true;
    }

    public void Dispose()
    {
        if (_lDatabaseSessionScope is null)
        {
            return;
        }

        try
        {
            if (!_lDatabaseSessionDone)
            {
                _lDatabaseSessionDone = true;
                _lDatabaseSessionScope.Rollback();
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (SqliteException)
        {
        }
        finally
        {
            _lDatabaseSessionScope.Dispose();
            _lDatabaseSessionLink.Dispose();
            _lDatabaseSessionOwner?.LDatabaseSessionClear(this);
        }
    }
}
