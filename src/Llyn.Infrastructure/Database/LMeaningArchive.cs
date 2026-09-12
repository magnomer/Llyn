using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LMeaningArchive
{
    private readonly LDatabase _lMeaningArchiveDatabase;

    public LMeaningArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lMeaningArchiveDatabase = database;
    }

    public LMeaning LMeaningCreate(LMeaning meaning)
    {
        ArgumentNullException.ThrowIfNull(meaning);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(meaning.LMeaningEntryId);

        using LDatabaseSession session = _lMeaningArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LMeaningParentValidate(connection, meaning.LMeaningEntryId, meaning.LMeaningParentId);

        LMeaning stored = meaning with
        {
            LMeaningPosition = LMeaningSiblingRead(connection, meaning.LMeaningEntryId, meaning.LMeaningParentId).Count,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO sense (
                    entry_id, parent_id, position, title_state, title, gloss,
                    definition_language, definition_state, definition, labels)
                VALUES (
                    $entry, $parent, $position, $titleState, $title, $gloss,
                    $language, $definitionState, $definition, $labels)
                RETURNING id;
                """;
            command.Parameters.AddWithValue("$entry", stored.LMeaningEntryId);
            command.Parameters.AddWithValue("$parent", (object?)stored.LMeaningParentId ?? DBNull.Value);
            command.Parameters.AddWithValue("$position", stored.LMeaningPosition);
            LStateColumn.LStateColumnApply(command, "title", stored.LMeaningTitle);
            command.Parameters.AddWithValue("$gloss", (object?)stored.LMeaningGloss ?? DBNull.Value);
            command.Parameters.AddWithValue("$language", (object?)stored.LMeaningDefinitionLanguage ?? DBNull.Value);
            LStateColumn.LStateColumnApply(command, "definition", stored.LMeaningDefinition);
            command.Parameters.AddWithValue("$labels", stored.LMeaningLabels);
            stored = stored with { LMeaningId = (long)command.ExecuteScalar()! };
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LMeaning> LMeaningRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lMeaningArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, entry_id, parent_id, position, title_state, title, gloss,
                   definition_language, definition_state, definition, labels
            FROM sense WHERE entry_id = $entry
            ORDER BY ifnull(parent_id, ''), position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LMeaning> meanings = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            meanings.Add(LMeaningRowRead(reader));
        }

        return meanings;
    }

    public LMeaning? LMeaningSingleRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lMeaningArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, entry_id, parent_id, position, title_state, title, gloss,
                   definition_language, definition_state, definition, labels
            FROM sense WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LMeaningRowRead(reader) : null;
    }

    public void LMeaningUpdate(LMeaning meaning)
    {
        ArgumentNullException.ThrowIfNull(meaning);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(meaning.LMeaningId);

        using LDatabaseSession session = _lMeaningArchiveDatabase.LDatabaseSessionStart();
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
            LStateColumn.LStateColumnApply(command, "title", meaning.LMeaningTitle);
            command.Parameters.AddWithValue("$gloss", (object?)meaning.LMeaningGloss ?? DBNull.Value);
            command.Parameters.AddWithValue("$language", (object?)meaning.LMeaningDefinitionLanguage ?? DBNull.Value);
            LStateColumn.LStateColumnApply(command, "definition", meaning.LMeaningDefinition);
            command.Parameters.AddWithValue("$labels", meaning.LMeaningLabels);
            command.Parameters.AddWithValue("$id", meaning.LMeaningId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Meaning carries the id '{meaning.LMeaningId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LMeaningParentUpdate(long id, long? parentId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lMeaningArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        (long? entryId, long? held) = LMeaningHolderRead(connection, id);
        if (entryId is null)
        {
            throw new InvalidOperationException($"No Meaning carries the id '{id}'.");
        }

        if (held == parentId)
        {
            return;
        }

        LMeaningParentValidate(connection, entryId, parentId);
        LMeaningCycleValidate(connection, id, parentId);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "UPDATE sense SET parent_id = $parent, position = $position WHERE id = $id;";
            command.Parameters.AddWithValue("$parent", (object?)parentId ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "$position", LMeaningSiblingRead(connection, entryId, parentId).Count);
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        LMeaningSiblingNormalize(
            connection, entryId, held, LMeaningSiblingRead(connection, entryId, held));

        session.LDatabaseSessionCommit();
    }

    public void LMeaningMove(long id, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lMeaningArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        (long? entryId, long? parentId) = LMeaningHolderRead(connection, id);
        if (entryId is null)
        {
            return;
        }

        IReadOnlyList<long> siblings = LMeaningSiblingRead(connection, entryId, parentId);
        IReadOnlyList<long> moved = LDatabaseOrder.LDatabaseOrderInsert(siblings, id, position);
        LMeaningSiblingNormalize(connection, entryId, parentId, moved);

        session.LDatabaseSessionCommit();
    }

    public void LMeaningDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lMeaningArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        (long? entryId, long? parentId) = LMeaningHolderRead(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM sense WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        if (entryId is not null)
        {
            LMeaningSiblingNormalize(connection, entryId, parentId, LMeaningSiblingRead(connection, entryId, parentId));
        }

        session.LDatabaseSessionCommit();
    }

    private const string LMeaningSubtreeQuery =
        """
        WITH RECURSIVE subtree(id) AS (
            SELECT id FROM sense WHERE id = $id
            UNION ALL
            SELECT child.id FROM sense child JOIN subtree ON child.parent_id = subtree.id
        )
        """;

    private static (long? LMeaningEntry, long? LMeaningParent) LMeaningHolderRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT entry_id, parent_id FROM sense WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return (null, null);
        }

        return (reader.GetInt64(0), reader.IsDBNull(1) ? null : reader.GetInt64(1));
    }

    private static IReadOnlyList<long> LMeaningSiblingRead(
        SqliteConnection connection, long? entryId, long? parentId)
    {
        return parentId is null
            ? LDatabaseOrder.LDatabaseOrderRead(
                connection, "sense", "entry_id = $owner AND parent_id IS NULL", entryId, "id")
            : LDatabaseOrder.LDatabaseOrderRead(
                connection, "sense", "parent_id = $owner", parentId, "id");
    }

    private static void LMeaningSiblingNormalize(
        SqliteConnection connection, long? entryId, long? parentId, IReadOnlyList<long> order)
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

    private static void LMeaningCycleValidate(
        SqliteConnection connection, long id, long? parentId)
    {
        if (parentId is null)
        {
            return;
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            LMeaningSubtreeQuery +
            """

            SELECT COUNT(*) FROM subtree WHERE id = $parent;
            """;
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$parent", parentId);
        if (Convert.ToInt64(command.ExecuteScalar()) > 0)
        {
            throw new InvalidOperationException(
                $"Meaning '{parentId}' sits inside meaning '{id}' and cannot become its parent.");
        }
    }

    private static void LMeaningParentValidate(SqliteConnection connection, long? entryId, long? parentId)
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
                $"Parent meaning '{parentId}' does not exist in entry '{entryId}'.");
        }
    }

    private static LMeaning LMeaningRowRead(SqliteDataReader reader)
    {
        return new LMeaning(
            reader.GetInt64(0),
            reader.GetInt64(1),
            reader.IsDBNull(2) ? null : reader.GetInt64(2),
            reader.GetInt32(3),
            LStateColumn.LStateColumnRead(reader, 4),
            reader.IsDBNull(6) ? null : reader.GetString(6),
            reader.IsDBNull(7) ? null : reader.GetString(7),
            LStateColumn.LStateColumnRead(reader, 8),
            reader.GetString(10));
    }
}
