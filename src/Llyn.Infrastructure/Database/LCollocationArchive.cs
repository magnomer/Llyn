using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LCollocationArchive
{
    private readonly LDatabase _lCollocationArchiveDatabase;

    public LCollocationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lCollocationArchiveDatabase = database;
    }

    public LCollocation LCollocationCreate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(collocation.LCollocationEntryId);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LCollocation stored = collocation with
        {
            LCollocationPosition = LCollocationSiblingRead(connection, collocation.LCollocationEntryId).Count,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO collocation (
                    entry_parent, position, title_state, title,
                    expression_state, expression, meaning_state, meaning)
                VALUES (
                    $entry, $position, $titleState, $title,
                    $expressionState, $expression, $meaningState, $meaning)
                RETURNING collocation_id;
                """;
            command.Parameters.AddWithValue("$entry", stored.LCollocationEntryId);
            command.Parameters.AddWithValue("$position", stored.LCollocationPosition);
            LStateColumn.LStateColumnApply(command, "title", stored.LCollocationTitle);
            LStateColumn.LStateColumnApply(command, "expression", stored.LCollocationExpression);
            LStateColumn.LStateColumnApply(command, "meaning", stored.LCollocationMeaning);
            stored = stored with { LCollocationId = (long)command.ExecuteScalar()! };
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LCollocation> LCollocationRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT collocation_id, entry_parent, position, title_state, title,
                   expression_state, expression, meaning_state, meaning
            FROM collocation WHERE entry_parent = $entry
            ORDER BY position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LCollocation> collocations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            collocations.Add(new LCollocation(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt32(2),
                LStateColumn.LStateColumnRead(reader, 3),
                LStateColumn.LStateColumnRead(reader, 5),
                LStateColumn.LStateColumnRead(reader, 7)));
        }

        return collocations;
    }

    public void LCollocationUpdate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(collocation.LCollocationId);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE collocation
                SET title_state = $titleState, title = $title,
                    expression_state = $expressionState, expression = $expression,
                    meaning_state = $meaningState, meaning = $meaning
                WHERE collocation_id = $id;
                """;
            LStateColumn.LStateColumnApply(command, "title", collocation.LCollocationTitle);
            LStateColumn.LStateColumnApply(command, "expression", collocation.LCollocationExpression);
            LStateColumn.LStateColumnApply(command, "meaning", collocation.LCollocationMeaning);
            command.Parameters.AddWithValue("$id", collocation.LCollocationId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException(
                    $"No collocation carries the id '{collocation.LCollocationId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LCollocationMove(long id, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        long? entryId = LCollocationHolderRead(connection, id);
        if (entryId is null)
        {
            return;
        }

        IReadOnlyList<long> order = LDatabaseOrder.LDatabaseOrderInsert(
            LCollocationSiblingRead(connection, entryId), id, position);
        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "collocation", "entry_parent = $owner", entryId, "collocation_id", order);

        session.LDatabaseSessionCommit();
    }

    public void LCollocationDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        long? entryId = LCollocationHolderRead(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM collocation WHERE collocation_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        if (entryId is not null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "collocation", "entry_parent = $owner", entryId,
                "collocation_id", LCollocationSiblingRead(connection, entryId));
        }

        session.LDatabaseSessionCommit();
    }

    public long? LCollocationHolderRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        return LCollocationHolderRead(session.LDatabaseSessionConnection, id);
    }

    private static long? LCollocationHolderRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT entry_parent FROM collocation WHERE collocation_id = $id;";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteScalar() as long?;
    }

    private static IReadOnlyList<long> LCollocationSiblingRead(SqliteConnection connection, long? entryId)
    {
        return LDatabaseOrder.LDatabaseOrderRead(
            connection, "collocation", "entry_parent = $owner", entryId, "collocation_id");
    }
}
