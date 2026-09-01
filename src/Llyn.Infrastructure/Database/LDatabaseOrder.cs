using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Rewrites the <c>position</c> column of an ordered row set so it reads <c>0, 1, 2, …</c> again after
/// an insert, a move, or a removal. Every ordered table in the schema carries a unique index over
/// <c>(owner, position)</c>, which is what makes a one-row-at-a-time reorder impossible: swapping two
/// neighbours collides on the first statement. So the whole set is rewritten in two passes — every
/// position is first shifted clear of the range by a constant (a constant shift keeps the set unique
/// against itself and out of the way of the final values), then written back as its index in the
/// intended order.
/// <para>
/// A set is named by a <em>scope</em>: a store-owned SQL predicate over <c>$owner</c>, such as
/// <c>entry_id = $owner</c> or <c>entry_id = $owner AND parent_id IS NULL</c>. Scopes and column names
/// are literals the calling store chooses, never caller input; every value still travels as a
/// parameter.
/// </para>
/// </summary>
public static class LDatabaseOrder
{
    /// <summary>The shift that shelves the current positions while the final ones are written.</summary>
    private const long LDatabaseOrderShift = 1_000_000_000L;

    /// <summary>
    /// Reads the members of an ordered set in their stored order — the <c>id</c> column of a stable-id
    /// table, or the member column of an association table. The list an insert or a move rearranges
    /// before handing it back to a normalize call.
    /// </summary>
    public static IReadOnlyList<string> LDatabaseOrderRead(
        SqliteConnection connection,
        string table,
        string scope,
        string owner,
        string memberColumn)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"SELECT {memberColumn} FROM {table} WHERE {scope} ORDER BY position;";
        command.Parameters.AddWithValue("$owner", owner);

        List<string> identifiers = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            identifiers.Add(reader.GetString(0));
        }

        return identifiers;
    }

    /// <summary>
    /// Rewrites the positions of an ordered set so the members listed in
    /// <paramref name="identifiers"/> take positions <c>0 … n-1</c> in that order. Every row in the
    /// scope must appear in the list, and <paramref name="memberColumn"/> is the column that names a
    /// member — <c>id</c> for a stable-id table, the member column for an association table.
    /// </summary>
    public static void LDatabaseOrderNormalize(
        SqliteConnection connection,
        string table,
        string scope,
        string owner,
        string memberColumn,
        IReadOnlyList<string> identifiers)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(identifiers);

        using (SqliteCommand shelve = connection.CreateCommand())
        {
            shelve.CommandText = $"UPDATE {table} SET position = position + $shift WHERE {scope};";
            shelve.Parameters.AddWithValue("$shift", LDatabaseOrderShift);
            shelve.Parameters.AddWithValue("$owner", owner);
            shelve.ExecuteNonQuery();
        }

        for (int position = 0; position < identifiers.Count; position++)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                $"UPDATE {table} SET position = $position WHERE {scope} AND {memberColumn} = $member;";
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$owner", owner);
            command.Parameters.AddWithValue("$member", identifiers[position]);
            command.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Moves the member at <paramref name="from"/> in <paramref name="identifiers"/> to
    /// <paramref name="target"/>, clamping the target into the list, and returns the rearranged order
    /// for a normalize call. The order is returned unchanged when the source index is outside the list.
    /// </summary>
    public static IReadOnlyList<string> LDatabaseOrderMove(
        IReadOnlyList<string> identifiers,
        int from,
        int target)
    {
        ArgumentNullException.ThrowIfNull(identifiers);

        if (from < 0 || from >= identifiers.Count)
        {
            return identifiers;
        }

        List<string> moved = new(identifiers);
        string member = moved[from];
        moved.RemoveAt(from);
        moved.Insert(Math.Clamp(target, 0, moved.Count), member);
        return moved;
    }

    /// <summary>
    /// Places <paramref name="member"/> at <paramref name="target"/> in <paramref name="identifiers"/>,
    /// clamping the target into the list, and returns the resulting order for a normalize call. Used
    /// when a row has just been inserted and the whole set must be renumbered around it.
    /// </summary>
    public static IReadOnlyList<string> LDatabaseOrderInsert(
        IReadOnlyList<string> identifiers,
        string member,
        int target)
    {
        ArgumentNullException.ThrowIfNull(identifiers);
        ArgumentException.ThrowIfNullOrWhiteSpace(member);

        List<string> placed = new(identifiers);
        placed.Remove(member);
        placed.Insert(Math.Clamp(target, 0, placed.Count), member);
        return placed;
    }
}
