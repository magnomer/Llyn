using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LAncestorArchive : LAncestorVault
{
    private readonly LDatabase _lAncestorArchiveDatabase;

    public LAncestorArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lAncestorArchiveDatabase = database;
    }

    public IReadOnlyList<LAncestor> LAncestorRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lAncestorArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT ancestor_id, child_ref, position, entry_ref,
                   text, language, tongue_ref, gloss, relation_ref
            FROM ancestor
            WHERE entry_parent = $entry
            ORDER BY ancestor_id;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LAncestor> ancestors = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            ancestors.Add(new LAncestor(
                reader.GetInt64(0),
                reader.IsDBNull(1) ? null : reader.GetInt64(1),
                reader.GetInt32(2),
                reader.IsDBNull(3) ? null : reader.GetInt64(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.IsDBNull(6) ? null : reader.GetInt64(6),
                reader.GetString(7),
                reader.IsDBNull(8) ? null : reader.GetInt64(8)));
        }

        return ancestors;
    }

    public IReadOnlyList<LAncestor> LAncestorSave(long entryId, IReadOnlyList<LAncestor> ancestors)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(ancestors);

        IReadOnlyList<LAncestor> ordered = LAncestorOrderResolve(entryId, ancestors);

        using LDatabaseSession session = _lAncestorArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand cleared = connection.CreateCommand())
        {
            cleared.CommandText = "DELETE FROM ancestor WHERE entry_parent = $entry;";
            cleared.Parameters.AddWithValue("$entry", entryId);
            cleared.ExecuteNonQuery();
        }

        Dictionary<long, long> stored = [];
        List<LAncestor> written = new(ordered.Count);
        foreach (LAncestor ancestor in ordered)
        {
            long? child = ancestor.LAncestorChildId is long key ? stored[key] : null;

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO ancestor (entry_parent, child_ref, position, entry_ref,
                                      text, language, tongue_ref, gloss, relation_ref)
                VALUES ($entry, $child, $position,
                        (SELECT entry_id FROM entry WHERE entry_id = $target),
                        $text, $language,
                        (SELECT ancestor_tongue_id FROM ancestor_tongue WHERE ancestor_tongue_id = $tongue),
                        $gloss,
                        (SELECT ancestor_relation_id FROM ancestor_relation WHERE ancestor_relation_id = $relation))
                RETURNING ancestor_id;
                """;
            command.Parameters.AddWithValue("$entry", entryId);
            command.Parameters.AddWithValue("$child", (object?)child ?? DBNull.Value);
            command.Parameters.AddWithValue("$position", ancestor.LAncestorPosition);
            command.Parameters.AddWithValue("$target", (object?)ancestor.LAncestorTargetId ?? DBNull.Value);
            command.Parameters.AddWithValue("$text", ancestor.LAncestorText);
            command.Parameters.AddWithValue("$language", ancestor.LAncestorLanguage);
            command.Parameters.AddWithValue("$tongue", (object?)ancestor.LAncestorTongueId ?? DBNull.Value);
            command.Parameters.AddWithValue("$gloss", ancestor.LAncestorGloss);
            command.Parameters.AddWithValue("$relation", (object?)ancestor.LAncestorRelationId ?? DBNull.Value);
            long id = (long)command.ExecuteScalar()!;

            stored[ancestor.LAncestorId] = id;
            written.Add(ancestor with { LAncestorId = id, LAncestorChildId = child });
        }

        session.LDatabaseSessionCommit();
        return written;
    }

    public IReadOnlyList<LAncestorRelation> LAncestorRelationRead()
    {
        using LDatabaseSession session = _lAncestorArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT relation.ancestor_relation_id, relation.name, relation.builtin, COUNT(ancestor.ancestor_id)
            FROM ancestor_relation relation
            LEFT JOIN ancestor ON ancestor.relation_ref = relation.ancestor_relation_id
            GROUP BY relation.ancestor_relation_id
            ORDER BY relation.builtin DESC, relation.ancestor_relation_id;
            """;

        List<LAncestorRelation> relations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            relations.Add(new LAncestorRelation(
                reader.GetInt64(0), reader.GetString(1), reader.GetInt64(2) != 0, reader.GetInt32(3)));
        }

        return relations;
    }

    public LAncestorRelation LAncestorRelationCreate(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string trimmed = name.Trim();
        LAncestorListCreate("ancestor_relation", trimmed);
        return LAncestorRelationRead().First(relation =>
            string.Equals(relation.LAncestorRelationName, trimmed, StringComparison.Ordinal));
    }

    public bool LAncestorRelationDelete(long relationId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(relationId);

        return LAncestorListDelete(
            "DELETE FROM ancestor_relation WHERE ancestor_relation_id = $id AND builtin = 0;", relationId);
    }

    public IReadOnlyList<LAncestorTongue> LAncestorTongueRead()
    {
        using LDatabaseSession session = _lAncestorArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT tongue.ancestor_tongue_id, tongue.name, COUNT(ancestor.ancestor_id)
            FROM ancestor_tongue tongue
            LEFT JOIN ancestor ON ancestor.tongue_ref = tongue.ancestor_tongue_id
            GROUP BY tongue.ancestor_tongue_id
            ORDER BY tongue.ancestor_tongue_id;
            """;

        List<LAncestorTongue> tongues = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            tongues.Add(new LAncestorTongue(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2)));
        }

        return tongues;
    }

    public LAncestorTongue LAncestorTongueCreate(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string trimmed = name.Trim();
        LAncestorListCreate("ancestor_tongue", trimmed);
        return LAncestorTongueRead().First(tongue =>
            string.Equals(tongue.LAncestorTongueName, trimmed, StringComparison.Ordinal));
    }

    public bool LAncestorTongueDelete(long tongueId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tongueId);

        return LAncestorListDelete("DELETE FROM ancestor_tongue WHERE ancestor_tongue_id = $id;", tongueId);
    }

    private void LAncestorListCreate(string table, string name)
    {
        using LDatabaseSession session = _lAncestorArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = $"INSERT INTO {table} (name) VALUES ($name) ON CONFLICT (name) DO NOTHING;";
            command.Parameters.AddWithValue("$name", name);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    private bool LAncestorListDelete(string sql, long id)
    {
        using LDatabaseSession session = _lAncestorArchiveDatabase.LDatabaseSessionStart();
        int removed;
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = sql;
            command.Parameters.AddWithValue("$id", id);
            removed = command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return removed > 0;
    }

    private static IReadOnlyList<LAncestor> LAncestorOrderResolve(long entryId, IReadOnlyList<LAncestor> ancestors)
    {
        HashSet<long> keys = [];
        foreach (LAncestor ancestor in ancestors)
        {
            if (ancestor.LAncestorId == 0 || !keys.Add(ancestor.LAncestorId))
            {
                throw new InvalidOperationException(
                    $"A box of the ancestry of Entry {entryId} carries no key or a key another box holds.");
            }
        }

        Dictionary<long, List<LAncestor>> sources = [];
        foreach (LAncestor ancestor in ancestors)
        {
            long child = ancestor.LAncestorChildId ?? 0;
            if (child != 0 && !keys.Contains(child))
            {
                throw new InvalidOperationException(
                    $"A box of the ancestry of Entry {entryId} feeds box {child}, which the ancestry lacks.");
            }

            if (!sources.TryGetValue(child, out List<LAncestor>? siblings))
            {
                siblings = [];
                sources[child] = siblings;
            }

            siblings.Add(ancestor);
        }

        List<LAncestor> ordered = new(ancestors.Count);
        Queue<long> pending = new([0L]);
        while (pending.Count > 0)
        {
            if (!sources.TryGetValue(pending.Dequeue(), out List<LAncestor>? siblings))
            {
                continue;
            }

            int position = 0;
            foreach (LAncestor ancestor in siblings.OrderBy(static sibling => sibling.LAncestorPosition))
            {
                ordered.Add(ancestor with { LAncestorPosition = position++ });
                pending.Enqueue(ancestor.LAncestorId);
            }
        }

        if (ordered.Count != ancestors.Count)
        {
            throw new InvalidOperationException(
                $"Some boxes of the ancestry of Entry {entryId} feed one another in a ring.");
        }

        return ordered;
    }
}
