using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists the collocations an entry owns. A collocation is a stable-id row assigned an opaque id here
/// on creation, ordered within its entry; updating rewrites its title, expression and meaning, so ids survive
/// reordering.
/// Its synonym interlinks are not owned text and live in their own store
/// (<see cref="LSynonymArchive"/>); they are removed with the collocation by the foreign-key cascade, and
/// deleting the entry removes its collocations and, with them, their synonyms.
/// <para>
/// Order within the entry is a unique index, so a position is never written one row at a time: a new
/// collocation is appended to the end, and <see cref="LCollocationMove"/> renumbers the whole set through
/// <see cref="LDatabaseOrder"/>.
/// </para>
/// </summary>
public sealed class LCollocationArchive
{
    private readonly LDatabase _lCollocationArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens sessions through.</summary>
    public LCollocationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lCollocationArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="collocation"/> with a fresh opaque id at the end of its entry's order and
    /// returns the stored collocation with that id and its assigned position filled in.
    /// </summary>
    public LCollocation LCollocationCreate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        ArgumentException.ThrowIfNullOrWhiteSpace(collocation.LCollocationEntryId);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LCollocation stored = collocation with
        {
            LCollocationId = LIdentity.LIdentityCreate(),
            LCollocationPosition = LCollocationSiblingRead(connection, collocation.LCollocationEntryId).Count,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO collocation (id, entry_id, position, title, expression, meaning)
                VALUES ($id, $entry, $position, $title, $expression, $meaning);
                """;
            command.Parameters.AddWithValue("$id", stored.LCollocationId);
            command.Parameters.AddWithValue("$entry", stored.LCollocationEntryId);
            command.Parameters.AddWithValue("$position", stored.LCollocationPosition);
            command.Parameters.AddWithValue("$title", (object?)stored.LCollocationTitle ?? DBNull.Value);
            command.Parameters.AddWithValue("$expression", (object?)stored.LCollocationExpression ?? DBNull.Value);
            command.Parameters.AddWithValue("$meaning", (object?)stored.LCollocationMeaning ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    /// <summary>Reads the entry's collocations in card order.</summary>
    public IReadOnlyList<LCollocation> LCollocationRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, entry_id, position, title, expression, meaning
            FROM collocation WHERE entry_id = $entry
            ORDER BY position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LCollocation> collocations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            collocations.Add(new LCollocation(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5)));
        }

        return collocations;
    }

    /// <summary>
    /// Updates the title, expression, and meaning of the collocation identified by
    /// <paramref name="collocation"/>'s id. The id, owning entry, and position are untouched — card order is changed by
    /// <see cref="LCollocationMove"/>, which has to renumber the whole set. Throws when no collocation
    /// carries that id.
    /// </summary>
    public void LCollocationUpdate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        ArgumentException.ThrowIfNullOrWhiteSpace(collocation.LCollocationId);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE collocation SET title = $title, expression = $expression, meaning = $meaning "
                + "WHERE id = $id;";
            command.Parameters.AddWithValue("$title", (object?)collocation.LCollocationTitle ?? DBNull.Value);
            command.Parameters.AddWithValue("$expression", (object?)collocation.LCollocationExpression ?? DBNull.Value);
            command.Parameters.AddWithValue("$meaning", (object?)collocation.LCollocationMeaning ?? DBNull.Value);
            command.Parameters.AddWithValue("$id", collocation.LCollocationId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException(
                    $"No collocation carries the id '{collocation.LCollocationId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Moves the collocation identified by <paramref name="id"/> to <paramref name="position"/> in its
    /// entry's card order, renumbering the whole set so positions stay <c>0 … n-1</c>. A position outside
    /// the set is clamped into it, and nothing moves when no collocation carries that id.
    /// </summary>
    public void LCollocationMove(string id, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string? entryId = LCollocationHolderRead(connection, id);
        if (entryId is null)
        {
            return;
        }

        IReadOnlyList<string> order = LDatabaseOrder.LDatabaseOrderInsert(
            LCollocationSiblingRead(connection, entryId), id, position);
        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "collocation", "entry_id = $owner", entryId, "id", order);

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Deletes the collocation identified by <paramref name="id"/>. Its synonym interlinks and its
    /// example, tag, and situation association rows go with it through the foreign-key cascade; the
    /// independent Examples, Tags, and Situations they pointed at are left standing. The collocations
    /// left under the same entry are renumbered so their positions stay contiguous.
    /// </summary>
    public void LCollocationDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string? entryId = LCollocationHolderRead(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM collocation WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        if (entryId is not null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "collocation", "entry_id = $owner", entryId,
                "id", LCollocationSiblingRead(connection, entryId));
        }

        session.LDatabaseSessionCommit();
    }

    // The entry a collocation belongs to, or null when no collocation carries the id.
    private static string? LCollocationHolderRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT entry_id FROM collocation WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteScalar() as string;
    }

    // The collocations of one entry, in card order.
    private static IReadOnlyList<string> LCollocationSiblingRead(SqliteConnection connection, string entryId)
    {
        return LDatabaseOrder.LDatabaseOrderRead(
            connection, "collocation", "entry_id = $owner", entryId, "id");
    }
}
