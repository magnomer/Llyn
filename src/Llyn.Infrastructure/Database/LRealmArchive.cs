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
        return LRealmSingleRead(session.LDatabaseSessionConnection, LSchemaRealm.LSchemaRealmOwn)
            ?? throw new InvalidOperationException(
                "The workspace realm row is missing; the database was not created by this build.");
    }

    public static LRealm? LRealmSingleRead(SqliteConnection connection, long id)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, value FROM realm WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LRealm(reader.GetInt64(0), reader.GetGuid(1));
    }

    public static long LRealmResolve(SqliteConnection connection, Guid value)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO realm (value)
                VALUES ($value)
                ON CONFLICT (value) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$value", value.ToByteArray());
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT id FROM realm WHERE value = $value;";
            command.Parameters.AddWithValue("$value", value.ToByteArray());
            return Convert.ToInt64(command.ExecuteScalar());
        }
    }
}
