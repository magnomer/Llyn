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
/// TODO: this is the storage seam only — the precise targeting rules for a collocation synonym are not
/// finalized (see <see cref="LSynonym"/>). Beyond the XOR, no rule about which targets are legal is
/// enforced yet.
/// </para>
/// </summary>
public sealed class LSynonymArchive
{
    private readonly LDatabase _lSynonymArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LSynonymArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSynonymArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="synonym"/> with a fresh opaque id and returns it with that id filled in.
    /// Exactly one of the target ids must be set; naming both or neither throws an
    /// <see cref="InvalidOperationException"/> before anything is written.
    /// </summary>
    public LSynonym LSynonymCreate(LSynonym synonym)
    {
        ArgumentNullException.ThrowIfNull(synonym);
        ArgumentException.ThrowIfNullOrWhiteSpace(synonym.LSynonymCollocationId);
        LSynonymTargetValidate(synonym);

        string id = LIdentity.LIdentityCreate();
        LSynonym stored = synonym with { LSynonymId = id };

        using SqliteConnection connection = _lSynonymArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
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

        using SqliteConnection connection = _lSynonymArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
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
    /// Deletes the synonym interlink identified by <paramref name="id"/>. The Entry/Meaning it pointed at
    /// is untouched.
    /// </summary>
    public void LSynonymDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lSynonymArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM collocation_synonym WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
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
