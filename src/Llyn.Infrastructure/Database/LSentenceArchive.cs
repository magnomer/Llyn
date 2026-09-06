using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LSentenceArchive
{
    private const string LSentenceArchiveScope = "sense_id = $owner";

    private readonly LDatabase _lSentenceArchiveDatabase;

    public LSentenceArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSentenceArchiveDatabase = database;
    }

    public IReadOnlyList<LSentence> LSentenceMeaningRead(string meaningId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(meaningId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        return LSentenceMeaningRead(session.LDatabaseSessionConnection, meaningId);
    }

    public void LSentenceMeaningSave(string meaningId, IReadOnlyList<LSentence> sentences)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(meaningId);
        ArgumentNullException.ThrowIfNull(sentences);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LSentenceMeaningClear(connection, meaningId);
        for (int position = 0; position < sentences.Count; position++)
        {
            LSentenceSave(connection, meaningId, sentences[position], position);
        }

        session.LDatabaseSessionCommit();
    }

    public void LSentenceMeaningAttach(string meaningId, string exampleId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(meaningId);
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        IReadOnlyList<string> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, "sense_example", LSentenceArchiveScope, meaningId, "id");

        string id = LIdentity.LIdentityCreate();
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO sense_example (id, sense_id, example_id, position)
                VALUES ($id, $meaning, $example, $position);
                """;
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$meaning", meaningId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "sense_example", LSentenceArchiveScope, meaningId, "id",
            LDatabaseOrder.LDatabaseOrderInsert(current, id, position));

        session.LDatabaseSessionCommit();
    }

    public void LSentenceMeaningDetach(string meaningId, string exampleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(meaningId);
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "DELETE FROM sense_example WHERE sense_id = $meaning AND example_id = $example;";
            command.Parameters.AddWithValue("$meaning", meaningId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "sense_example", LSentenceArchiveScope, meaningId, "id",
            LDatabaseOrder.LDatabaseOrderRead(
                connection, "sense_example", LSentenceArchiveScope, meaningId, "id"));

        session.LDatabaseSessionCommit();
    }

    internal static IReadOnlyList<LSentence> LSentenceMeaningRead(SqliteConnection connection, string meaningId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT link.id, link.sense_id, link.position,
                   example.id, example.language, example.text_state, example.text,
                   example.translation_state, example.translation,
                   example.source_state, example.source_id,
                   link.revision_state, revision.id, revision.language,
                   revision.text_state, revision.text,
                   revision.translation_state, revision.translation,
                   revision.source_state, revision.source_id,
                   link.particle_state, link.particle,
                   link.dependence_state, link.dependence
            FROM sense_example link
            JOIN example ON example.id = link.example_id
            LEFT JOIN example revision ON revision.id = link.revision_id
            WHERE link.sense_id = $meaning
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$meaning", meaningId);

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

    internal static void LSentenceMeaningClear(SqliteConnection connection, string meaningId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM sense_example WHERE sense_id = $meaning;";
        command.Parameters.AddWithValue("$meaning", meaningId);
        command.ExecuteNonQuery();
    }

    internal static void LSentenceExampleClear(SqliteConnection connection, string exampleId)
    {
        List<string> owners = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "SELECT DISTINCT sense_id FROM sense_example WHERE example_id = $example OR revision_id = $example;";
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
                """
                UPDATE sense_example SET revision_state = 'unspecified', revision_id = NULL
                WHERE revision_id = $example;
                """;
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM sense_example WHERE example_id = $example;";
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        foreach (string owner in owners)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "sense_example", LSentenceArchiveScope, owner, "id",
                LDatabaseOrder.LDatabaseOrderRead(
                    connection, "sense_example", LSentenceArchiveScope, owner, "id"));
        }
    }

    private static void LSentenceSave(
        SqliteConnection connection, string meaningId, LSentence sentence, int position)
    {
        ArgumentNullException.ThrowIfNull(sentence);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO sense_example (
                id, sense_id, example_id, position,
                revision_state, revision_id,
                particle_state, particle,
                dependence_state, dependence)
            VALUES (
                $id, $meaning, $example, $position,
                $revisionState, $revision,
                $particleState, $particle,
                $dependenceState, $dependence);
            """;
        command.Parameters.AddWithValue(
            "$id",
            string.IsNullOrWhiteSpace(sentence.LSentenceId) ? LIdentity.LIdentityCreate() : sentence.LSentenceId);
        command.Parameters.AddWithValue("$meaning", meaningId);
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
}
