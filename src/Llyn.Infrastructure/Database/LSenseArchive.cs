using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists the Meaning tree an entry owns. Each sense is a stable-id node assigned an opaque id here on
/// creation; it may nest under another sense in the <em>same</em> entry, and that parent link is
/// validated before the row is written. Reordering rewrites a sense's <c>position</c> only, so ids never
/// change. Deleting a sense removes its subordinate senses through the foreign-key cascade, and deleting
/// the entry removes the whole tree.
/// </summary>
public sealed class LSenseArchive
{
    private readonly LDatabase _lSenseArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LSenseArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSenseArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="sense"/> with a fresh opaque id and returns the stored sense with that id
    /// filled in. When it names a parent, the parent must be an existing sense in the same entry;
    /// otherwise an <see cref="InvalidOperationException"/> is thrown before anything is written.
    /// </summary>
    public LSense LSenseCreate(LSense sense)
    {
        ArgumentNullException.ThrowIfNull(sense);
        ArgumentException.ThrowIfNullOrWhiteSpace(sense.LSenseEntryId);

        string id = LIdentity.LIdentityCreate();
        LSense stored = sense with { LSenseId = id };

        using SqliteConnection connection = _lSenseArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();

        LSenseParentValidate(connection, stored.LSenseEntryId, stored.LSenseParentId);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO sense (id, entry_id, parent_id, position, gloss, definition_language, definition, labels)
                VALUES ($id, $entry, $parent, $position, $gloss, $language, $definition, $labels);
                """;
            command.Parameters.AddWithValue("$id", stored.LSenseId);
            command.Parameters.AddWithValue("$entry", stored.LSenseEntryId);
            command.Parameters.AddWithValue("$parent", (object?)stored.LSenseParentId ?? DBNull.Value);
            command.Parameters.AddWithValue("$position", stored.LSensePosition);
            command.Parameters.AddWithValue("$gloss", (object?)stored.LSenseGloss ?? DBNull.Value);
            command.Parameters.AddWithValue("$language", (object?)stored.LSenseDefinitionLanguage ?? DBNull.Value);
            command.Parameters.AddWithValue("$definition", (object?)stored.LSenseDefinition ?? DBNull.Value);
            command.Parameters.AddWithValue("$labels", stored.LSenseLabels);
            command.ExecuteNonQuery();
        }

        transaction.Commit();
        return stored;
    }

    /// <summary>
    /// Reads the entry's whole Meaning tree as a flat list ordered by parent then position, so a caller
    /// rebuilds the hierarchy from each sense's <see cref="LSense.LSenseParentId"/> with siblings already
    /// in order.
    /// </summary>
    public IReadOnlyList<LSense> LSenseRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using SqliteConnection connection = _lSenseArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT id, entry_id, parent_id, position, gloss, definition_language, definition, labels
            FROM sense WHERE entry_id = $entry
            ORDER BY ifnull(parent_id, ''), position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LSense> senses = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            senses.Add(new LSense(
                reader.GetString(0),
                reader.GetString(1),
                reader.IsDBNull(2) ? null : reader.GetString(2),
                reader.GetInt32(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.IsDBNull(6) ? null : reader.GetString(6),
                reader.GetString(7)));
        }

        return senses;
    }

    /// <summary>
    /// Updates the gloss, definition (with its language), labels, and position of the sense identified by
    /// <paramref name="sense"/>'s id. The id, entry, and parent link are untouched, so this covers both
    /// editing a Meaning's definition and reordering it among its siblings.
    /// </summary>
    public void LSenseUpdate(LSense sense)
    {
        ArgumentNullException.ThrowIfNull(sense);
        ArgumentException.ThrowIfNullOrWhiteSpace(sense.LSenseId);

        using SqliteConnection connection = _lSenseArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE sense
            SET gloss = $gloss, definition_language = $language, definition = $definition,
                labels = $labels, position = $position
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$gloss", (object?)sense.LSenseGloss ?? DBNull.Value);
        command.Parameters.AddWithValue("$language", (object?)sense.LSenseDefinitionLanguage ?? DBNull.Value);
        command.Parameters.AddWithValue("$definition", (object?)sense.LSenseDefinition ?? DBNull.Value);
        command.Parameters.AddWithValue("$labels", sense.LSenseLabels);
        command.Parameters.AddWithValue("$position", sense.LSensePosition);
        command.Parameters.AddWithValue("$id", sense.LSenseId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Deletes the sense identified by <paramref name="id"/> together with everything it owns: its
    /// inline definition field (columns of the row itself), the relations originating from it, its
    /// subordinate senses with the same treatment applied down the tree, and its example, tag, and
    /// situation association rows. Sibling and ancestor senses are untouched, and the independent
    /// Examples, Tags, and Situations it referenced are left standing — only the links go.
    /// <para>
    /// Guarded where a cascade must not decide alone: while a relation outside the deleted subtree or
    /// any collocation synonym still points at one of these senses, nothing is deleted and an
    /// <see cref="InvalidOperationException"/> is thrown. Links originating inside the subtree are
    /// cleared as part of the delete, since they belong to rows that are going anyway.
    /// </para>
    /// </summary>
    public void LSenseDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lSenseArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();

        LSenseLinkValidate(connection, id);
        LSenseLinkClear(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM sense WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    // The senses this delete removes: the named one and every sense beneath it, walked with a
    // recursive term over parent_id. Both link statements below start from this set.
    private const string LSenseSubtreeQuery =
        """
        WITH RECURSIVE subtree(id) AS (
            SELECT id FROM sense WHERE id = $id
            UNION ALL
            SELECT child.id FROM sense child JOIN subtree ON child.parent_id = subtree.id
        )
        """;

    // Counts the links reaching the subtree from outside it: a relation held by a sense that survives,
    // and any collocation synonym — a collocation is not deleted when a sense is, so every synonym
    // pointing here counts as an outside link. The schema keeps those columns cascade-free on purpose;
    // this turns the foreign-key error they would raise into a message that names the reason.
    private static void LSenseLinkValidate(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            LSenseSubtreeQuery +
            """

            SELECT
                (SELECT COUNT(*) FROM relation_sense target
                    JOIN relation origin ON origin.id = target.relation_id
                 WHERE target.sense_id IN (SELECT id FROM subtree)
                   AND origin.sense_id NOT IN (SELECT id FROM subtree))
                + (SELECT COUNT(*) FROM collocation_synonym link
                   WHERE link.target_sense_id IN (SELECT id FROM subtree));
            """;
        command.Parameters.AddWithValue("$id", id);
        long links = Convert.ToInt64(command.ExecuteScalar());
        if (links > 0)
        {
            throw new InvalidOperationException(
                $"Meaning {id} is still the target of {links} lexical link(s) from outside it; remove those links before deleting it.");
        }
    }

    // Clears the target rows of the relations the subtree owns before the senses go. They would
    // cascade with their relation anyway, but a relation inside the subtree pointing at another sense
    // in the same subtree would be checked against a row already being deleted, and the order the
    // cascade visits tables in is not ours to rely on.
    private static void LSenseLinkClear(SqliteConnection connection, string id)
    {
        string[] tables = ["relation_entry", "relation_sense"];
        foreach (string table in tables)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                LSenseSubtreeQuery +
                $"""

                DELETE FROM {table} WHERE relation_id IN
                    (SELECT id FROM relation WHERE sense_id IN (SELECT id FROM subtree));
                """;
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }

    private static void LSenseParentValidate(SqliteConnection connection, string entryId, string? parentId)
    {
        if (parentId is null)
        {
            return;
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sense WHERE id = $parent AND entry_id = $entry;";
        command.Parameters.AddWithValue("$parent", parentId);
        command.Parameters.AddWithValue("$entry", entryId);
        long found = Convert.ToInt64(command.ExecuteScalar());
        if (found == 0)
        {
            throw new InvalidOperationException(
                $"Parent sense '{parentId}' does not exist in entry '{entryId}'.");
        }
    }
}
