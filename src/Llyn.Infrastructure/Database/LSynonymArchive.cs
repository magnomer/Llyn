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
        ArgumentException.ThrowIfNullOrWhiteSpace(synonym.LSynonymCollocationId);
        LSynonymTargetValidate(synonym);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LSynonym stored = synonym with
        {
            LSynonymId = LIdentity.LIdentityCreate(),
            LSynonymPosition = LSynonymSiblingRead(connection, synonym.LSynonymCollocationId).Count,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO collocation_synonym (id, collocation_id, position, target_entry_id, target_sense_id)
                VALUES ($id, $collocation, $position, $entry, $sense);
                """;
            command.Parameters.AddWithValue("$id", stored.LSynonymId);
            command.Parameters.AddWithValue("$collocation", stored.LSynonymCollocationId);
            command.Parameters.AddWithValue("$position", stored.LSynonymPosition);
            command.Parameters.AddWithValue("$entry", (object?)stored.LSynonymTargetEntry ?? DBNull.Value);
            command.Parameters.AddWithValue("$sense", (object?)stored.LSynonymTargetMeaning ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LSynonym> LSynonymRead(string collocationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(collocationId);

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
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4)));
        }

        return synonyms;
    }

    public void LSynonymUpdate(LSynonym synonym)
    {
        ArgumentNullException.ThrowIfNull(synonym);
        ArgumentException.ThrowIfNullOrWhiteSpace(synonym.LSynonymId);
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
            command.Parameters.AddWithValue("$entry", (object?)synonym.LSynonymTargetEntry ?? DBNull.Value);
            command.Parameters.AddWithValue("$sense", (object?)synonym.LSynonymTargetMeaning ?? DBNull.Value);
            command.Parameters.AddWithValue("$id", synonym.LSynonymId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No synonym carries the id '{synonym.LSynonymId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LSynonymMove(string id, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string? collocationId = LSynonymHolderRead(connection, id);
        if (collocationId is null)
        {
            return;
        }

        IReadOnlyList<string> order = LDatabaseOrder.LDatabaseOrderInsert(
            LSynonymSiblingRead(connection, collocationId), id, position);
        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "collocation_synonym", "collocation_id = $owner", collocationId, "id", order);

        session.LDatabaseSessionCommit();
    }

    public void LSynonymDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSynonymArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string? collocationId = LSynonymHolderRead(connection, id);

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

    private static string? LSynonymHolderRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT collocation_id FROM collocation_synonym WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteScalar() as string;
    }

    private static IReadOnlyList<string> LSynonymSiblingRead(SqliteConnection connection, string collocationId)
    {
        return LDatabaseOrder.LDatabaseOrderRead(
            connection, "collocation_synonym", "collocation_id = $owner", collocationId, "id");
    }

    private static void LSynonymTargetValidate(LSynonym synonym)
    {
        bool hasEntry = !string.IsNullOrWhiteSpace(synonym.LSynonymTargetEntry);
        bool hasMeaning = !string.IsNullOrWhiteSpace(synonym.LSynonymTargetMeaning);
        if (hasEntry == hasMeaning)
        {
            throw new InvalidOperationException(
                "A collocation synonym must have exactly one target: a target Entry id XOR a target Meaning id.");
        }
    }
}
