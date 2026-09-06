using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LSentenceArchive
{
    private readonly LDatabase _lSentenceArchiveDatabase;

    public LSentenceArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSentenceArchiveDatabase = database;
    }

    public IReadOnlyList<LSentence> LSentenceMeaningRead(string meaningId)
    {
        return LSentenceOwnerRead("sense_example", "sense_id", meaningId);
    }

    public IReadOnlyList<LSentence> LSentenceCollocationRead(string collocationId)
    {
        return LSentenceOwnerRead("collocation_example", "collocation_id", collocationId);
    }

    public void LSentenceMeaningSave(string meaningId, IReadOnlyList<LSentence> sentences)
    {
        LSentenceOwnerSave("sense_example", "sense_id", meaningId, sentences);
    }

    public void LSentenceCollocationSave(string collocationId, IReadOnlyList<LSentence> sentences)
    {
        LSentenceOwnerSave("collocation_example", "collocation_id", collocationId, sentences);
    }

    public void LSentenceMeaningAttach(string meaningId, string exampleId, int position)
    {
        LSentenceOwnerAttach("sense_example", "sense_id", meaningId, exampleId, position);
    }

    public void LSentenceCollocationAttach(string collocationId, string exampleId, int position)
    {
        LSentenceOwnerAttach("collocation_example", "collocation_id", collocationId, exampleId, position);
    }

    public void LSentenceMeaningDetach(string meaningId, string exampleId)
    {
        LSentenceOwnerDetach("sense_example", "sense_id", meaningId, exampleId);
    }

    public void LSentenceCollocationDetach(string collocationId, string exampleId)
    {
        LSentenceOwnerDetach("collocation_example", "collocation_id", collocationId, exampleId);
    }

    internal static void LSentenceExampleClear(SqliteConnection connection, string exampleId)
    {
        LSentenceTableClear(connection, "sense_example", "sense_id", exampleId);
        LSentenceTableClear(connection, "collocation_example", "collocation_id", exampleId);
    }

    private static string LSentenceScopeCreate(string column)
    {
        return $"{column} = $owner";
    }

    private static IReadOnlyList<LSentence> LSentenceOwnerRead(
        SqliteConnection connection, string table, string column, string ownerId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            SELECT link.id, link.{column}, link.position,
                   example.id, example.language, example.text_state, example.text,
                   example.translation_state, example.translation,
                   example.source_state, example.source_id,
                   link.revision_state, revision.id, revision.language,
                   revision.text_state, revision.text,
                   revision.translation_state, revision.translation,
                   revision.source_state, revision.source_id,
                   link.particle_state, link.particle,
                   link.dependence_state, link.dependence
            FROM {table} link
            JOIN example ON example.id = link.example_id
            LEFT JOIN example revision ON revision.id = link.revision_id
            WHERE link.{column} = $owner
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$owner", ownerId);

        List<LSentence> sentences = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            sentences.Add(new LSentence(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt32(2),
                LSentenceExampleRead(reader, 3),
                reader.IsDBNull(12) ? null : LSentenceExampleRead(reader, 12),
                LStateColumn.LStateColumnRead(reader, 20),
                LStateColumn.LStateColumnRead(reader, 22)));
        }

        return sentences;
    }

    private static void LSentenceOwnerClear(
        SqliteConnection connection, string table, string column, string ownerId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {table} WHERE {column} = $owner;";
        command.Parameters.AddWithValue("$owner", ownerId);
        command.ExecuteNonQuery();
    }

    private static void LSentenceTableClear(
        SqliteConnection connection, string table, string column, string exampleId)
    {
        List<string> owners = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"SELECT DISTINCT {column} FROM {table} WHERE example_id = $example OR revision_id = $example;";
            command.Parameters.AddWithValue("$example", exampleId);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                owners.Add(reader.GetString(0));
            }
        }

        if (owners.Count == 0)
        {
            return;
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                UPDATE {table} SET revision_state = 'unspecified', revision_id = NULL
                WHERE revision_id = $example;
                """;
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE example_id = $example;";
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        string scope = LSentenceScopeCreate(column);
        foreach (string owner in owners)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, table, scope, owner, "id",
                LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, owner, "id"));
        }
    }

    private static void LSentenceSave(
        SqliteConnection connection, string table, string column, string ownerId, LSentence sentence, int position)
    {
        ArgumentNullException.ThrowIfNull(sentence);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            INSERT INTO {table} (
                id, {column}, example_id, position,
                revision_state, revision_id,
                particle_state, particle,
                dependence_state, dependence)
            VALUES (
                $id, $owner, $example, $position,
                $revisionState, $revision,
                $particleState, $particle,
                $dependenceState, $dependence);
            """;
        command.Parameters.AddWithValue(
            "$id",
            string.IsNullOrWhiteSpace(sentence.LSentenceId) ? LIdentity.LIdentityCreate() : sentence.LSentenceId);
        command.Parameters.AddWithValue("$owner", ownerId);
        command.Parameters.AddWithValue("$example", sentence.LSentenceExample.LExampleId);
        command.Parameters.AddWithValue("$position", position);
        command.Parameters.AddWithValue(
            "$revisionState", sentence.LSentenceRevision is null ? "unspecified" : "specified");
        command.Parameters.AddWithValue(
            "$revision", (object?)sentence.LSentenceRevision?.LExampleId ?? DBNull.Value);
        LStateColumn.LStateColumnApply(command, "particle", sentence.LSentenceParticle);
        LStateColumn.LStateColumnApply(command, "dependence", sentence.LSentenceDependence);
        command.ExecuteNonQuery();
    }

    private static LExample LSentenceExampleRead(SqliteDataReader reader, int start)
    {
        return new LExample(
            reader.GetString(start),
            reader.GetString(start + 1),
            LStateColumn.LStateColumnRead(reader, start + 2),
            LStateColumn.LStateColumnRead(reader, start + 4),
            LStateColumn.LStateColumnRead(reader, start + 6));
    }

    private IReadOnlyList<LSentence> LSentenceOwnerRead(string table, string column, string ownerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        return LSentenceOwnerRead(session.LDatabaseSessionConnection, table, column, ownerId);
    }

    private void LSentenceOwnerSave(string table, string column, string ownerId, IReadOnlyList<LSentence> sentences)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
        ArgumentNullException.ThrowIfNull(sentences);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LSentenceOwnerClear(connection, table, column, ownerId);
        for (int position = 0; position < sentences.Count; position++)
        {
            LSentenceSave(connection, table, column, ownerId, sentences[position], position);
        }

        session.LDatabaseSessionCommit();
    }

    private void LSentenceOwnerAttach(string table, string column, string ownerId, string exampleId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = LSentenceScopeCreate(column);
        IReadOnlyList<string> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, ownerId, "id");

        string id = LIdentity.LIdentityCreate();
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} (id, {column}, example_id, position)
                VALUES ($id, $owner, $example, $position);
                """;
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$owner", ownerId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, ownerId, "id",
            LDatabaseOrder.LDatabaseOrderInsert(current, id, position));

        session.LDatabaseSessionCommit();
    }

    private void LSentenceOwnerDetach(string table, string column, string ownerId, string exampleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = LSentenceScopeCreate(column);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"DELETE FROM {table} WHERE {column} = $owner AND example_id = $example;";
            command.Parameters.AddWithValue("$owner", ownerId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, ownerId, "id",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, ownerId, "id"));

        session.LDatabaseSessionCommit();
    }
}
