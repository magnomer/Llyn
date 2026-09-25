using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LDatabaseOrder
{
    private const long LDatabaseOrderShift = 1_000_000_000L;

    public static IReadOnlyList<long> LDatabaseOrderRead(
        SqliteConnection connection,
        string table,
        string scope,
        long? owner,
        string memberColumn)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"SELECT {memberColumn} FROM {table} WHERE {scope} ORDER BY position;";
        command.Parameters.AddWithValue("$owner", (object?)owner ?? DBNull.Value);

        List<long> identifiers = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            identifiers.Add(reader.GetInt64(0));
        }

        return identifiers;
    }

    public static void LDatabaseOrderNormalize(
        SqliteConnection connection,
        string table,
        string scope,
        long? owner,
        string memberColumn,
        IReadOnlyList<long> identifiers)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(identifiers);

        using (SqliteCommand shelve = connection.CreateCommand())
        {
            shelve.CommandText = $"UPDATE {table} SET position = position + $shift WHERE {scope};";
            shelve.Parameters.AddWithValue("$shift", LDatabaseOrderShift);
            shelve.Parameters.AddWithValue("$owner", (object?)owner ?? DBNull.Value);
            shelve.ExecuteNonQuery();
        }

        for (int position = 0; position < identifiers.Count; position++)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                $"UPDATE {table} SET position = $position WHERE {scope} AND {memberColumn} = $member;";
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$owner", (object?)owner ?? DBNull.Value);
            command.Parameters.AddWithValue("$member", identifiers[position]);
            command.ExecuteNonQuery();
        }
    }

    public static IReadOnlyList<long> LDatabaseOrderInsert(
        IReadOnlyList<long> identifiers,
        long member,
        int target)
    {
        ArgumentNullException.ThrowIfNull(identifiers);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(member);

        List<long> placed = new(identifiers);
        placed.Remove(member);
        placed.Insert(Math.Clamp(target, 0, placed.Count), member);
        return placed;
    }
}
