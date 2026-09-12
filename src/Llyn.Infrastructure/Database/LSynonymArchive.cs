using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LSynonymArchive
{
    private readonly LDatabase _lSynonymArchiveDatabase;

    public LSynonymArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSynonymArchiveDatabase = database;
    }

    public LSynonym LSynonymCreate(LSynonym synonym)
    {
        ArgumentNullException.ThrowIfNull(synonym);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(synonym.LSynonymCollocationId);
        LSynonymTargetValidate(synonym);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LSynonym stored = synonym with
        {
            LSynonymPosition = LSynonymSiblingRead(connection, synonym.LSynonymCollocationId).Count,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO collocation_synonym (collocation_id, position, target_entry_id, target_sense_id)
                VALUES ($collocation, $position, $entry, $sense)
                RETURNING id;
                """;
            command.Parameters.AddWithValue("$collocation", stored.LSynonymCollocationId);
            command.Parameters.AddWithValue("$position", stored.LSynonymPosition);
            command.Parameters.AddWithValue("$entry", LSynonymAnchorRead(stored.LSynonymTargetEntry));
            command.Parameters.AddWithValue("$sense", LSynonymAnchorRead(stored.LSynonymTargetMeaning));
            stored = stored with { LSynonymId = (long)command.ExecuteScalar()! };
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LSynonym> LSynonymRead(long collocationId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(collocationId);

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
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt32(2),
                LStateAnchor.LStateAnchorRead(reader.IsDBNull(3) ? null : reader.GetInt64(3)),
                LStateAnchor.LStateAnchorRead(reader.IsDBNull(4) ? null : reader.GetInt64(4))));
        }

        return synonyms;
    }

    public void LSynonymUpdate(LSynonym synonym)
    {
        ArgumentNullException.ThrowIfNull(synonym);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(synonym.LSynonymId);
        LSynonymTargetValidate(synonym);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE collocation_synonym
                SET target_entry_id = $entry, target_sense_id = $sense
                WHERE id = $id;
                """;
            command.Parameters.AddWithValue("$entry", LSynonymAnchorRead(synonym.LSynonymTargetEntry));
            command.Parameters.AddWithValue("$sense", LSynonymAnchorRead(synonym.LSynonymTargetMeaning));
            command.Parameters.AddWithValue("$id", synonym.LSynonymId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No synonym carries the id '{synonym.LSynonymId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LSynonymMove(long id, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        long? collocationId = LSynonymHolderRead(connection, id);
        if (collocationId is null)
        {
            return;
        }

        IReadOnlyList<long> order = LDatabaseOrder.LDatabaseOrderInsert(
            LSynonymSiblingRead(connection, collocationId), id, position);
        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "collocation_synonym", "collocation_id = $owner", collocationId, "id", order);

        session.LDatabaseSessionCommit();
    }

    public void LSynonymDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        long? collocationId = LSynonymHolderRead(connection, id);

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

    private static long? LSynonymHolderRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT collocation_id FROM collocation_synonym WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteScalar() as long?;
    }

    private static IReadOnlyList<long> LSynonymSiblingRead(SqliteConnection connection, long? collocationId)
    {
        return LDatabaseOrder.LDatabaseOrderRead(
            connection, "collocation_synonym", "collocation_id = $owner", collocationId, "id");
    }

    private static object LSynonymAnchorRead(LStateAnchor anchor)
    {
        return anchor.LStateAnchorEmpty ? DBNull.Value : anchor.LStateAnchorShow();
    }

    private static void LSynonymTargetValidate(LSynonym synonym)
    {
        bool hasEntry = !synonym.LSynonymTargetEntry.LStateAnchorEmpty;
        bool hasMeaning = !synonym.LSynonymTargetMeaning.LStateAnchorEmpty;
        if (hasEntry == hasMeaning)
        {
            throw new InvalidOperationException(
                "A collocation synonym must have exactly one target: a target Entry id XOR a target Meaning id.");
        }
    }
}
