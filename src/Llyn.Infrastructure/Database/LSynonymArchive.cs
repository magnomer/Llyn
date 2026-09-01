using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists the synonym interlinks a Collocation owns. A synonym is a stable-id row assigned an opaque id
/// here on creation, hanging from its origin collocation and pointing at exactly one target — an Entry or
/// a Meaning — through the job05 discriminated target model. The single-target rule is enforced before
/// anything is written, and again by the table's check constraint. Deleting the origin collocation removes
/// its synonyms through the foreign-key cascade; the referenced Entry/Meaning is left intact.
/// <para>
/// A new synonym is appended to the end of its collocation's order and <see cref="LSynonymMove"/>
/// renumbers the whole set, so the caller never writes a position into a set that has to stay contiguous.
/// </para>
/// <para>
/// TODO: this is the storage seam only — the precise targeting rules for a collocation synonym are not
/// finalized (see <see cref="LSynonym"/>). Beyond the XOR, no rule about which targets are legal is
/// enforced yet.
/// </para>
/// </summary>
public sealed class LSynonymArchive
{
    private readonly LDatabase _lSynonymArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens sessions through.</summary>
    public LSynonymArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSynonymArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="synonym"/> with a fresh opaque id at the end of its collocation's order and
    /// returns it with that id and its assigned position filled in. Exactly one of the target ids must be
    /// set; naming both or neither throws an <see cref="InvalidOperationException"/> before anything is
    /// written.
    /// </summary>
    public LSynonym LSynonymCreate(LSynonym synonym)
    {
        ArgumentNullException.ThrowIfNull(synonym);
        ArgumentException.ThrowIfNullOrWhiteSpace(synonym.LSynonymCollocationId);
        LSynonymTargetValidate(synonym);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LSynonym stored = synonym with
        {
            LSynonymId = LIdentity.LIdentityCreate(),
            LSynonymPosition = LSynonymSiblingRead(connection, synonym.LSynonymCollocationId).Count,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO collocation_synonym (id, collocation_id, position, target_entry_id, target_sense_id)
                VALUES ($id, $collocation, $position, $entry, $sense);
                """;
            command.Parameters.AddWithValue("$id", stored.LSynonymId);
            command.Parameters.AddWithValue("$collocation", stored.LSynonymCollocationId);
            command.Parameters.AddWithValue("$position", stored.LSynonymPosition);
            command.Parameters.AddWithValue("$entry", (object?)stored.LSynonymTargetEntry ?? DBNull.Value);
            command.Parameters.AddWithValue("$sense", (object?)stored.LSynonymTargetSense ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    /// <summary>
    /// Reads the synonym interlinks hanging from the collocation identified by
    /// <paramref name="collocationId"/>, ordered by position, each with its single target resolved to
    /// either a target Entry id or a target Meaning id.
    /// </summary>
    public IReadOnlyList<LSynonym> LSynonymRead(string collocationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(collocationId);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, collocation_id, position, target_entry_id, target_sense_id
            FROM collocation_synonym WHERE collocation_id = $collocation
            ORDER BY position;
            """;
        command.Parameters.AddWithValue("$collocation", collocationId);

        List<LSynonym> synonyms = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            synonyms.Add(new LSynonym(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4)));
        }

        return synonyms;
    }

    /// <summary>
    /// Moves the synonym identified by <paramref name="id"/> to <paramref name="position"/> in its
    /// collocation's order, renumbering the whole set so positions stay <c>0 … n-1</c>. A position outside
    /// the set is clamped into it, and nothing moves when no synonym carries that id.
    /// </summary>
    public void LSynonymMove(string id, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string? collocationId = LSynonymHolderRead(connection, id);
        if (collocationId is null)
        {
            return;
        }

        IReadOnlyList<string> order = LDatabaseOrder.LDatabaseOrderInsert(
            LSynonymSiblingRead(connection, collocationId), id, position);
        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "collocation_synonym", "collocation_id = $owner", collocationId, "id", order);

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Deletes the synonym interlink identified by <paramref name="id"/>. The Entry/Meaning it pointed at
    /// is untouched, and the synonyms left under the same collocation are renumbered so their positions
    /// stay contiguous.
    /// </summary>
    public void LSynonymDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string? collocationId = LSynonymHolderRead(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM collocation_synonym WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        if (collocationId is not null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "collocation_synonym", "collocation_id = $owner", collocationId,
                "id", LSynonymSiblingRead(connection, collocationId));
        }

        session.LDatabaseSessionCommit();
    }

    // The collocation a synonym hangs from, or null when no synonym carries the id.
    private static string? LSynonymHolderRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT collocation_id FROM collocation_synonym WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteScalar() as string;
    }

    // The synonyms of one collocation, in order.
    private static IReadOnlyList<string> LSynonymSiblingRead(SqliteConnection connection, string collocationId)
    {
        return LDatabaseOrder.LDatabaseOrderRead(
            connection, "collocation_synonym", "collocation_id = $owner", collocationId, "id");
    }

    private static void LSynonymTargetValidate(LSynonym synonym)
    {
        bool hasEntry = !string.IsNullOrWhiteSpace(synonym.LSynonymTargetEntry);
        bool hasSense = !string.IsNullOrWhiteSpace(synonym.LSynonymTargetSense);
        if (hasEntry == hasSense)
        {
            throw new InvalidOperationException(
                "A collocation synonym must have exactly one target: a target Entry id XOR a target Meaning id.");
        }
    }
}
