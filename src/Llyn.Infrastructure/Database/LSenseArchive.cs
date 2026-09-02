using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LSenseArchive
{
    private readonly LDatabase _lSenseArchiveDatabase;

    public LSenseArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSenseArchiveDatabase = database;
    }

    public LSense LSenseCreate(LSense sense)
    {
        ArgumentNullException.ThrowIfNull(sense);
        ArgumentException.ThrowIfNullOrWhiteSpace(sense.LSenseEntryId);

        using LDatabaseSession session = _lSenseArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LSenseParentValidate(connection, sense.LSenseEntryId, sense.LSenseParentId);

        LSense stored = sense with
        {
            LSenseId = LIdentity.LIdentityCreate(),
            LSensePosition = LSenseSiblingRead(connection, sense.LSenseEntryId, sense.LSenseParentId).Count,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO sense (
                    id, entry_id, parent_id, position, title_state, title, gloss,
                    definition_language, definition_state, definition, labels)
                VALUES (
                    $id, $entry, $parent, $position, $titleState, $title, $gloss,
                    $language, $definitionState, $definition, $labels);
                """;
            command.Parameters.AddWithValue("$id", stored.LSenseId);
            command.Parameters.AddWithValue("$entry", stored.LSenseEntryId);
            command.Parameters.AddWithValue("$parent", (object?)stored.LSenseParentId ?? DBNull.Value);
            command.Parameters.AddWithValue("$position", stored.LSensePosition);
            LStateColumn.LStateColumnApply(command, "title", stored.LSenseTitle);
            command.Parameters.AddWithValue("$gloss", (object?)stored.LSenseGloss ?? DBNull.Value);
            command.Parameters.AddWithValue("$language", (object?)stored.LSenseDefinitionLanguage ?? DBNull.Value);
            LStateColumn.LStateColumnApply(command, "definition", stored.LSenseDefinition);
            command.Parameters.AddWithValue("$labels", stored.LSenseLabels);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LSense> LSenseRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lSenseArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, entry_id, parent_id, position, title_state, title, gloss,
                   definition_language, definition_state, definition, labels
            FROM sense WHERE entry_id = $entry
            ORDER BY ifnull(parent_id, ''), position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LSense> senses = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            senses.Add(LSenseRowRead(reader));
        }

        return senses;
    }

    public LSense? LSenseSingleRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSenseArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, entry_id, parent_id, position, title_state, title, gloss,
                   definition_language, definition_state, definition, labels
            FROM sense WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LSenseRowRead(reader) : null;
    }

    public IReadOnlyList<LSense> LSenseFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);
        query = query.Trim();

        using LDatabaseSession session = _lSenseArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT s.id, s.entry_id, s.parent_id, s.position, s.title_state, s.title, s.gloss,
                   s.definition_language, s.definition_state, s.definition, s.labels
            FROM sense s
            JOIN entry e ON e.id = s.entry_id
            WHERE $query = '' OR instr(lfold(e.headword), lfold($query)) > 0
            ORDER BY e.headword, ifnull(s.parent_id, ''), s.position;
            """;
        command.Parameters.AddWithValue("$query", query);

        List<LSense> senses = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            senses.Add(LSenseRowRead(reader));
        }

        return senses;
    }

    public void LSenseUpdate(LSense sense)
    {
        ArgumentNullException.ThrowIfNull(sense);
        ArgumentException.ThrowIfNullOrWhiteSpace(sense.LSenseId);

        using LDatabaseSession session = _lSenseArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE sense
                SET title_state = $titleState, title = $title, gloss = $gloss,
                    definition_language = $language,
                    definition_state = $definitionState, definition = $definition, labels = $labels
                WHERE id = $id;
                """;
            LStateColumn.LStateColumnApply(command, "title", sense.LSenseTitle);
            command.Parameters.AddWithValue("$gloss", (object?)sense.LSenseGloss ?? DBNull.Value);
            command.Parameters.AddWithValue("$language", (object?)sense.LSenseDefinitionLanguage ?? DBNull.Value);
            LStateColumn.LStateColumnApply(command, "definition", sense.LSenseDefinition);
            command.Parameters.AddWithValue("$labels", sense.LSenseLabels);
            command.Parameters.AddWithValue("$id", sense.LSenseId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Meaning carries the id '{sense.LSenseId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LSenseMove(string id, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSenseArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        (string? entryId, string? parentId) = LSenseHolderRead(connection, id);
        if (entryId is null)
        {
            return;
        }

        IReadOnlyList<string> siblings = LSenseSiblingRead(connection, entryId, parentId);
        IReadOnlyList<string> moved = LDatabaseOrder.LDatabaseOrderInsert(siblings, id, position);
        LSenseSiblingNormalize(connection, entryId, parentId, moved);

        session.LDatabaseSessionCommit();
    }

    public void LSenseDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSenseArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        (string? entryId, string? parentId) = LSenseHolderRead(connection, id);

        LSenseLinkValidate(connection, id);
        LSenseLinkClear(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM sense WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        if (entryId is not null)
        {
            LSenseSiblingNormalize(connection, entryId, parentId, LSenseSiblingRead(connection, entryId, parentId));
        }

        session.LDatabaseSessionCommit();
    }

    private const string LSenseSubtreeQuery =
        """
        WITH RECURSIVE subtree(id) AS (
            SELECT id FROM sense WHERE id = $id
            UNION ALL
            SELECT child.id FROM sense child JOIN subtree ON child.parent_id = subtree.id
        )
        """;

    private static (string? Entry, string? Parent) LSenseHolderRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT entry_id, parent_id FROM sense WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return (null, null);
        }

        return (reader.GetString(0), reader.IsDBNull(1) ? null : reader.GetString(1));
    }

    private static IReadOnlyList<string> LSenseSiblingRead(
        SqliteConnection connection, string entryId, string? parentId)
    {
        return parentId is null
            ? LDatabaseOrder.LDatabaseOrderRead(
                connection, "sense", "entry_id = $owner AND parent_id IS NULL", entryId, "id")
            : LDatabaseOrder.LDatabaseOrderRead(
                connection, "sense", "parent_id = $owner", parentId, "id");
    }

    private static void LSenseSiblingNormalize(
        SqliteConnection connection, string entryId, string? parentId, IReadOnlyList<string> order)
    {
        if (parentId is null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "sense", "entry_id = $owner AND parent_id IS NULL", entryId, "id", order);
            return;
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "sense", "parent_id = $owner", parentId, "id", order);
    }

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

    private static LSense LSenseRowRead(SqliteDataReader reader)
    {
        return new LSense(
            reader.GetString(0),
            reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetString(2),
            reader.GetInt32(3),
            LStateColumn.LStateColumnRead(reader, 4),
            reader.IsDBNull(6) ? null : reader.GetString(6),
            reader.IsDBNull(7) ? null : reader.GetString(7),
            LStateColumn.LStateColumnRead(reader, 8),
            reader.GetString(10));
    }
}
