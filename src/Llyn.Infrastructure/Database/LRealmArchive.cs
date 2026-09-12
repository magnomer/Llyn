using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LRealmArchive
{
    private readonly LDatabase _lRealmArchiveDatabase;

    public LRealmArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lRealmArchiveDatabase = database;
    }

    public LRealm LRealmRead()
    {
        using LDatabaseSession session = _lRealmArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT value FROM realm WHERE realm_id = $id;";
        command.Parameters.AddWithValue("$id", LSchemaRealm.LSchemaRealmRow);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            throw new InvalidOperationException(
                "The workspace realm row is missing; the database was not created by this build.");
        }

        return new LRealm(reader.GetGuid(0));
    }
}
