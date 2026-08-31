using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists the lexical relations a Meaning owns. Each relation is a stable-id row assigned an opaque id
/// here on creation, hanging from its origin sense and pointing at exactly one target — an Entry or
/// another Meaning — through a checked reference row in <c>relation_entry</c> XOR <c>relation_sense</c>.
/// The single-target rule is enforced before anything is written. Deleting the origin Meaning removes
/// its relations and their target rows through the foreign-key cascade; the referenced Entry/Meaning is
/// left intact.
/// </summary>
public sealed class LRelationArchive
{
    private readonly LDatabase _lRelationArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LRelationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lRelationArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="relation"/> with a fresh opaque id and its single target row, returning the
    /// stored relation with that id filled in. Exactly one of the target ids must be set; naming both or
    /// neither throws an <see cref="InvalidOperationException"/> before anything is written.
    /// </summary>
    public LRelation LRelationCreate(LRelation relation)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.LRelationSenseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.LRelationType);
        LRelationTargetValidate(relation);

        string id = LIdentity.LIdentityCreate();
        LRelation stored = relation with { LRelationId = id };

        using SqliteConnection connection = _lRelationArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO relation (id, sense_id, position, relation_type, label, labels)
                VALUES ($id, $sense, $position, $type, $label, $labels);
                """;
            command.Parameters.AddWithValue("$id", stored.LRelationId);
            command.Parameters.AddWithValue("$sense", stored.LRelationSenseId);
            command.Parameters.AddWithValue("$position", stored.LRelationPosition);
            command.Parameters.AddWithValue("$type", stored.LRelationType);
            command.Parameters.AddWithValue("$label", (object?)stored.LRelationLabel ?? DBNull.Value);
            command.Parameters.AddWithValue("$labels", (object?)stored.LRelationLabels ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        LRelationTargetInsert(connection, stored);

        transaction.Commit();
        return stored;
    }

    /// <summary>
    /// Reads the relations originating from the Meaning identified by <paramref name="senseId"/>, ordered
    /// by position, each with its single target resolved to either a target Entry id or a target Meaning
    /// id.
    /// </summary>
    public IReadOnlyList<LRelation> LRelationRead(string senseId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(senseId);

        using SqliteConnection connection = _lRelationArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT r.id, r.sense_id, r.position, r.relation_type, r.label, r.labels,
                   re.entry_id, rs.sense_id
            FROM relation r
            LEFT JOIN relation_entry re ON re.relation_id = r.id
            LEFT JOIN relation_sense rs ON rs.relation_id = r.id
            WHERE r.sense_id = $sense
            ORDER BY r.position;
            """;
        command.Parameters.AddWithValue("$sense", senseId);

        List<LRelation> relations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            relations.Add(new LRelation(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.IsDBNull(6) ? null : reader.GetString(6),
                reader.IsDBNull(7) ? null : reader.GetString(7)));
        }

        return relations;
    }

    /// <summary>
    /// Updates the type, label, labels, and position of the relation identified by
    /// <paramref name="relation"/>'s id. The id, origin Meaning, and target are untouched, so this covers
    /// retyping or relabelling a relation and reordering it among its siblings.
    /// </summary>
    public void LRelationUpdate(LRelation relation)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.LRelationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.LRelationType);

        using SqliteConnection connection = _lRelationArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE relation
            SET relation_type = $type, label = $label, labels = $labels, position = $position
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$type", relation.LRelationType);
        command.Parameters.AddWithValue("$label", (object?)relation.LRelationLabel ?? DBNull.Value);
        command.Parameters.AddWithValue("$labels", (object?)relation.LRelationLabels ?? DBNull.Value);
        command.Parameters.AddWithValue("$position", relation.LRelationPosition);
        command.Parameters.AddWithValue("$id", relation.LRelationId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Deletes the relation identified by <paramref name="id"/>. Its target row is removed by the
    /// foreign-key cascade; the referenced Entry/Meaning is untouched.
    /// </summary>
    public void LRelationDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lRelationArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM relation WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    private static void LRelationTargetValidate(LRelation relation)
    {
        bool hasEntry = !string.IsNullOrWhiteSpace(relation.LRelationTargetEntry);
        bool hasSense = !string.IsNullOrWhiteSpace(relation.LRelationTargetSense);
        if (hasEntry == hasSense)
        {
            throw new InvalidOperationException(
                "A relation must have exactly one target: a target Entry id XOR a target Meaning id.");
        }
    }

    private static void LRelationTargetInsert(SqliteConnection connection, LRelation relation)
    {
        using SqliteCommand command = connection.CreateCommand();
        if (!string.IsNullOrWhiteSpace(relation.LRelationTargetEntry))
        {
            command.CommandText =
                "INSERT INTO relation_entry (relation_id, entry_id) VALUES ($relation, $entry);";
            command.Parameters.AddWithValue("$relation", relation.LRelationId);
            command.Parameters.AddWithValue("$entry", relation.LRelationTargetEntry);
        }
        else
        {
            command.CommandText =
                "INSERT INTO relation_sense (relation_id, sense_id) VALUES ($relation, $sense);";
            command.Parameters.AddWithValue("$relation", relation.LRelationId);
            command.Parameters.AddWithValue("$sense", relation.LRelationTargetSense);
        }

        command.ExecuteNonQuery();
    }
}
