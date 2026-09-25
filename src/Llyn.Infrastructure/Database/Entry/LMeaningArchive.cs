using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LMeaningArchive : LMeaningVault
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
                $"""
                INSERT INTO sense (
                    sense_id, entry_parent, sense_parent, position, title_state, title,
                    definition_state, definition)
                VALUES (
                    {LSchemaCollocation.LSchemaCollocationSequence}, $entry, $parent, $position, $titleState, $title,
                    $definitionState, $definition)
                RETURNING sense_id;
                """;
            command.Parameters.AddWithValue("$entry", stored.LMeaningEntryId);
            command.Parameters.AddWithValue("$parent", (object?)stored.LMeaningParentId ?? DBNull.Value);
            command.Parameters.AddWithValue("$position", stored.LMeaningPosition);
            LStateColumn.LStateColumnApply(command, "title", stored.LMeaningTitle);
            LStateColumn.LStateColumnApply(command, "definition", stored.LMeaningDefinition);
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
            SELECT sense_id, entry_parent, sense_parent, position, title_state, title,
                   definition_state, definition
            FROM sense WHERE entry_parent = $entry
            ORDER BY ifnull(sense_parent, ''), position;
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
            SELECT sense_id, entry_parent, sense_parent, position, title_state, title,
                   definition_state, definition
            FROM sense WHERE sense_id = $id;
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
                SET title_state = $titleState, title = $title,
                    definition_state = $definitionState, definition = $definition
                WHERE sense_id = $id;
                """;
            LStateColumn.LStateColumnApply(command, "title", meaning.LMeaningTitle);
            LStateColumn.LStateColumnApply(command, "definition", meaning.LMeaningDefinition);
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
                "UPDATE sense SET sense_parent = $parent, position = $position WHERE sense_id = $id;";
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

    public void LMeaningOrderSet(long entryId, long? parentId, IReadOnlyList<long> order)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(order);

        using LDatabaseSession session = _lMeaningArchiveDatabase.LDatabaseSessionStart();
        LMeaningSiblingNormalize(session.LDatabaseSessionConnection, entryId, parentId, order);
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
            command.CommandText = "DELETE FROM sense WHERE sense_id = $id;";
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
        WITH RECURSIVE subtree(sense_id) AS (
            SELECT sense_id FROM sense WHERE sense_id = $id
            UNION ALL
            SELECT child.sense_id FROM sense child JOIN subtree ON child.sense_parent = subtree.sense_id
        )
        """;

    private static (long? LMeaningEntry, long? LMeaningParent) LMeaningHolderRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT entry_parent, sense_parent FROM sense WHERE sense_id = $id;";
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
                connection, "sense", "entry_parent = $owner AND sense_parent IS NULL", entryId, "sense_id")
            : LDatabaseOrder.LDatabaseOrderRead(
                connection, "sense", "sense_parent = $owner", parentId, "sense_id");
    }

    private static void LMeaningSiblingNormalize(
        SqliteConnection connection, long? entryId, long? parentId, IReadOnlyList<long> order)
    {
        if (parentId is null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "sense", "entry_parent = $owner AND sense_parent IS NULL", entryId, "sense_id", order);
            return;
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "sense", "sense_parent = $owner", parentId, "sense_id", order);
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

            SELECT COUNT(*) FROM subtree WHERE sense_id = $parent;
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
        command.CommandText = "SELECT COUNT(*) FROM sense WHERE sense_id = $parent AND entry_parent = $entry;";
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
            LStateColumn.LStateColumnRead(reader, 6));
    }
}
