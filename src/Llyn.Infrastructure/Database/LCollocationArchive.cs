using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists the collocations an entry owns. A collocation is a stable-id row assigned an opaque id here
/// on creation, ordered within its entry; updating rewrites its expression and position only, so ids
/// survive reordering. Its synonym interlinks are not owned text and live in their own store
/// (<see cref="LSynonymArchive"/>); they are removed with the collocation by the foreign-key cascade, and
/// deleting the entry removes its collocations and, with them, their synonyms.
/// </summary>
public sealed class LCollocationArchive
{
    private readonly LDatabase _lCollocationArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LCollocationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lCollocationArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="collocation"/> with a fresh opaque id and returns the stored collocation
    /// with that id filled in.
    /// </summary>
    public LCollocation LCollocationCreate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        ArgumentException.ThrowIfNullOrWhiteSpace(collocation.LCollocationEntryId);

        string id = LIdentity.LIdentityCreate();
        LCollocation stored = collocation with { LCollocationId = id };

        using SqliteConnection connection = _lCollocationArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO collocation (id, entry_id, position, expression)
            VALUES ($id, $entry, $position, $expression);
            """;
        command.Parameters.AddWithValue("$id", stored.LCollocationId);
        command.Parameters.AddWithValue("$entry", stored.LCollocationEntryId);
        command.Parameters.AddWithValue("$position", stored.LCollocationPosition);
        command.Parameters.AddWithValue("$expression", (object?)stored.LCollocationExpression ?? DBNull.Value);
        command.ExecuteNonQuery();

        return stored;
    }

    /// <summary>Reads the entry's collocations in card order.</summary>
    public IReadOnlyList<LCollocation> LCollocationRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using SqliteConnection connection = _lCollocationArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT id, entry_id, position, expression
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
                reader.IsDBNull(3) ? null : reader.GetString(3)));
        }

        return collocations;
    }

    /// <summary>
    /// Updates the expression and position of the collocation identified by
    /// <paramref name="collocation"/>'s id. The id and owning entry are untouched, so this covers both
    /// editing a collocation's expression and reordering it among its siblings.
    /// </summary>
    public void LCollocationUpdate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        ArgumentException.ThrowIfNullOrWhiteSpace(collocation.LCollocationId);

        using SqliteConnection connection = _lCollocationArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE collocation
            SET expression = $expression, position = $position
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$expression", (object?)collocation.LCollocationExpression ?? DBNull.Value);
        command.Parameters.AddWithValue("$position", collocation.LCollocationPosition);
        command.Parameters.AddWithValue("$id", collocation.LCollocationId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Deletes the collocation identified by <paramref name="id"/>. Its synonym interlinks go with it
    /// through the foreign-key cascade, and so will the example/tag/situation association rows job08 and
    /// job09 add — they hang from the collocation id with the same cascade, so this method needs no change
    /// when they arrive.
    /// </summary>
    public void LCollocationDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lCollocationArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM collocation WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
