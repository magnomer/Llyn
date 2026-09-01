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
        ArgumentException.ThrowIfNullOrWhiteSpace(collocation.LCollocationEntryId);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LCollocation stored = collocation with
        {
            LCollocationId = LIdentity.LIdentityCreate(),
            LCollocationPosition = LCollocationSiblingRead(connection, collocation.LCollocationEntryId).Count,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO collocation (id, entry_id, position, title, expression, meaning)
                VALUES ($id, $entry, $position, $title, $expression, $meaning);
                """;
            command.Parameters.AddWithValue("$id", stored.LCollocationId);
            command.Parameters.AddWithValue("$entry", stored.LCollocationEntryId);
            command.Parameters.AddWithValue("$position", stored.LCollocationPosition);
            command.Parameters.AddWithValue("$title", (object?)stored.LCollocationTitle ?? DBNull.Value);
            command.Parameters.AddWithValue("$expression", (object?)stored.LCollocationExpression ?? DBNull.Value);
            command.Parameters.AddWithValue("$meaning", (object?)stored.LCollocationMeaning ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LCollocation> LCollocationRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id, entry_id, position, title, expression, meaning
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
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5)));
        }

        return collocations;
    }

    public void LCollocationUpdate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        ArgumentException.ThrowIfNullOrWhiteSpace(collocation.LCollocationId);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE collocation SET title = $title, expression = $expression, meaning = $meaning "
                + "WHERE id = $id;";
            command.Parameters.AddWithValue("$title", (object?)collocation.LCollocationTitle ?? DBNull.Value);
            command.Parameters.AddWithValue("$expression", (object?)collocation.LCollocationExpression ?? DBNull.Value);
            command.Parameters.AddWithValue("$meaning", (object?)collocation.LCollocationMeaning ?? DBNull.Value);
            command.Parameters.AddWithValue("$id", collocation.LCollocationId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException(
                    $"No collocation carries the id '{collocation.LCollocationId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LCollocationMove(string id, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string? entryId = LCollocationHolderRead(connection, id);
        if (entryId is null)
        {
            return;
        }

        IReadOnlyList<string> order = LDatabaseOrder.LDatabaseOrderInsert(
            LCollocationSiblingRead(connection, entryId), id, position);
        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "collocation", "entry_id = $owner", entryId, "id", order);

        session.LDatabaseSessionCommit();
    }

    public void LCollocationDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lCollocationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string? entryId = LCollocationHolderRead(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM collocation WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        if (entryId is not null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "collocation", "entry_id = $owner", entryId,
                "id", LCollocationSiblingRead(connection, entryId));
        }

        session.LDatabaseSessionCommit();
    }

    private static string? LCollocationHolderRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT entry_id FROM collocation WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteScalar() as string;
    }

    private static IReadOnlyList<string> LCollocationSiblingRead(SqliteConnection connection, string entryId)
    {
        return LDatabaseOrder.LDatabaseOrderRead(
            connection, "collocation", "entry_id = $owner", entryId, "id");
    }
}
