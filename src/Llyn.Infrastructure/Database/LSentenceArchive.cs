using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LSentenceArchive
{
    private const long LSentenceShelf = 1_000_000_000L;

    private readonly LDatabase _lSentenceArchiveDatabase;

    public LSentenceArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSentenceArchiveDatabase = database;
    }

    public IReadOnlyList<LSentence> LSentenceMeaningRead(long meaningId)
    {
        return LSentenceOwnerRead("sense_example", "sense_parent", meaningId);
    }

    public IReadOnlyList<LSentence> LSentenceCollocationRead(long collocationId)
    {
        return LSentenceOwnerRead("collocation_example", "collocation_parent", collocationId);
    }

    public IReadOnlyList<long> LSentenceMeaningSave(long meaningId, IReadOnlyList<LSentence> sentences)
    {
        return LSentenceOwnerSave("sense_example", "sense_parent", meaningId, sentences);
    }

    public IReadOnlyList<long> LSentenceCollocationSave(long collocationId, IReadOnlyList<LSentence> sentences)
    {
        return LSentenceOwnerSave("collocation_example", "collocation_parent", collocationId, sentences);
    }

    public void LSentenceMeaningAttach(long meaningId, long exampleId, int position)
    {
        LSentenceOwnerAttach("sense_example", "sense_parent", meaningId, exampleId, position);
    }

    public void LSentenceCollocationAttach(long collocationId, long exampleId, int position)
    {
        LSentenceOwnerAttach("collocation_example", "collocation_parent", collocationId, exampleId, position);
    }

    public void LSentenceMeaningDetach(long meaningId, long exampleId)
    {
        LSentenceOwnerDetach("sense_example", "sense_parent", meaningId, exampleId);
    }

    public void LSentenceCollocationDetach(long collocationId, long exampleId)
    {
        LSentenceOwnerDetach("collocation_example", "collocation_parent", collocationId, exampleId);
    }

    public IReadOnlyList<string> LSentenceParticleRead(string language)
    {
        return LSentenceFrameRead("particle", language);
    }

    public IReadOnlyList<string> LSentenceDependenceRead(string language)
    {
        return LSentenceFrameRead("dependence", language);
    }

    internal static void LSentenceExampleClear(SqliteConnection connection, long exampleId)
    {
        LSentenceTableClear(connection, "sense_example", "sense_parent", exampleId);
        LSentenceTableClear(connection, "collocation_example", "collocation_parent", exampleId);
    }

    private IReadOnlyList<string> LSentenceFrameRead(string column, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT DISTINCT link.{column}
            FROM (
                SELECT hold.{column}, hold.{column}_state, sense.entry_parent
                FROM sense_example hold
                JOIN sense ON sense.sense_id = hold.sense_parent
                UNION ALL
                SELECT hold.{column}, hold.{column}_state, collocation.entry_parent
                FROM collocation_example hold
                JOIN collocation ON collocation.collocation_id = hold.collocation_parent
            ) link
            JOIN entry ON entry.entry_id = link.entry_parent
            WHERE entry.language = $language
              AND link.{column}_state = 'specified'
              AND link.{column} IS NOT NULL
              AND trim(link.{column}) <> ''
            ORDER BY link.{column};
            """;
        command.Parameters.AddWithValue("$language", language);

        List<string> values = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            values.Add(reader.GetString(0));
        }

        return values;
    }

    private static string LSentenceScopeCreate(string column)
    {
        return $"{column} = $owner";
    }

    private static IReadOnlyList<LSentence> LSentenceOwnerRead(
        SqliteConnection connection, string table, string column, long ownerId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            SELECT link.{table}_id, link.{column}, link.position,
                   example.example_id, example.language, example.text_state, example.text,
                   example.reference_state, example.reference_ref,
                   link.particle_state, link.particle,
                   link.dependence_state, link.dependence
            FROM {table} link
            LEFT JOIN example ON example.example_id = link.example_ref
            WHERE link.{column} = $owner
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$owner", ownerId);

        List<LSentence> sentences = [];
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                sentences.Add(new LSentence(
                    reader.GetInt64(0),
                    reader.GetInt64(1),
                    reader.GetInt32(2),
                    reader.IsDBNull(3) ? null : LSentenceExampleRead(reader, 3),
                    LStateColumn.LStateColumnRead(reader, 9),
                    LStateColumn.LStateColumnRead(reader, 11)));
            }
        }

        return LSentenceListLoad(connection, sentences);
    }

    private static IReadOnlyList<LSentence> LSentenceListLoad(
        SqliteConnection connection, IReadOnlyList<LSentence> sentences)
    {
        List<LExample> examples = [];
        foreach (LSentence sentence in sentences)
        {
            if (sentence.LSentenceExample is not null)
            {
                examples.Add(sentence.LSentenceExample);
            }
        }

        if (examples.Count == 0)
        {
            return sentences;
        }

        Dictionary<long, LExample> filled = [];
        foreach (LExample example in LExampleArchive.LExampleListLoad(connection, examples))
        {
            filled[example.LExampleId] = example;
        }

        List<LSentence> result = new(sentences.Count);
        foreach (LSentence sentence in sentences)
        {
            result.Add(sentence.LSentenceExample is null
                ? sentence
                : sentence with { LSentenceExample = filled[sentence.LSentenceExample.LExampleId] });
        }

        return result;
    }

    private static void LSentenceTableClear(
        SqliteConnection connection, string table, string column, long exampleId)
    {
        List<long> owners = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"SELECT DISTINCT {column} FROM {table} WHERE example_ref = $example;";
            command.Parameters.AddWithValue("$example", exampleId);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                owners.Add(reader.GetInt64(0));
            }
        }

        if (owners.Count == 0)
        {
            return;
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE example_ref = $example;";
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        string scope = LSentenceScopeCreate(column);
        foreach (long owner in owners)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, table, scope, owner, $"{table}_id",
                LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, owner, $"{table}_id"));
        }
    }

    private static long LSentenceSave(
        SqliteConnection connection, string table, string column, long ownerId, LSentence sentence, int position)
    {
        ArgumentNullException.ThrowIfNull(sentence);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            INSERT INTO {table} (
                {column}, example_ref, position,
                particle_state, particle,
                dependence_state, dependence)
            VALUES (
                $owner, $example, $position,
                $particleState, $particle,
                $dependenceState, $dependence)
            RETURNING {table}_id;
            """;
        command.Parameters.AddWithValue("$owner", ownerId);
        command.Parameters.AddWithValue(
            "$example", (object?)sentence.LSentenceExample?.LExampleId ?? DBNull.Value);
        command.Parameters.AddWithValue("$position", position);
        LStateColumn.LStateColumnApply(command, "particle", sentence.LSentenceParticle);
        LStateColumn.LStateColumnApply(command, "dependence", sentence.LSentenceDependence);
        return (long)command.ExecuteScalar()!;
    }

    private static bool LSentenceChange(
        SqliteConnection connection, string table, string column, long ownerId, LSentence sentence, int position)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            UPDATE {table}
            SET example_ref = $example, position = $position,
                particle_state = $particleState, particle = $particle,
                dependence_state = $dependenceState, dependence = $dependence
            WHERE {table}_id = $id AND {column} = $owner;
            """;
        command.Parameters.AddWithValue("$id", sentence.LSentenceId);
        command.Parameters.AddWithValue("$owner", ownerId);
        command.Parameters.AddWithValue(
            "$example", (object?)sentence.LSentenceExample?.LExampleId ?? DBNull.Value);
        command.Parameters.AddWithValue("$position", position);
        LStateColumn.LStateColumnApply(command, "particle", sentence.LSentenceParticle);
        LStateColumn.LStateColumnApply(command, "dependence", sentence.LSentenceDependence);
        return command.ExecuteNonQuery() == 1;
    }

    private static void LSentencePositionAdjust(
        SqliteConnection connection, string table, string column, long ownerId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"UPDATE {table} SET position = position + $shift WHERE {column} = $owner;";
        command.Parameters.AddWithValue("$shift", LSentenceShelf);
        command.Parameters.AddWithValue("$owner", ownerId);
        command.ExecuteNonQuery();
    }

    private static void LSentenceOwnerClear(
        SqliteConnection connection, string table, string column, long ownerId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"DELETE FROM {table} WHERE {column} = $owner AND position >= $shift;";
        command.Parameters.AddWithValue("$shift", LSentenceShelf);
        command.Parameters.AddWithValue("$owner", ownerId);
        command.ExecuteNonQuery();
    }

    private static LExample LSentenceExampleRead(SqliteDataReader reader, int start)
    {
        return new LExample(
            reader.GetInt64(start),
            reader.GetString(start + 1),
            LStateColumn.LStateColumnRead(reader, start + 2),
            LStateColumn.LStateColumnResolve(reader, start + 4));
    }

    private IReadOnlyList<LSentence> LSentenceOwnerRead(string table, string column, long ownerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        return LSentenceOwnerRead(session.LDatabaseSessionConnection, table, column, ownerId);
    }

    private IReadOnlyList<long> LSentenceOwnerSave(
        string table, string column, long ownerId, IReadOnlyList<LSentence> sentences)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
        ArgumentNullException.ThrowIfNull(sentences);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LSentencePositionAdjust(connection, table, column, ownerId);
        List<long> written = new(sentences.Count);
        for (int position = 0; position < sentences.Count; position++)
        {
            LSentence sentence = sentences[position];
            bool kept = sentence.LSentenceId > 0
                && !written.Contains(sentence.LSentenceId)
                && LSentenceChange(connection, table, column, ownerId, sentence, position);
            written.Add(kept
                ? sentence.LSentenceId
                : LSentenceSave(connection, table, column, ownerId, sentence, position));
        }

        LSentenceOwnerClear(connection, table, column, ownerId);

        session.LDatabaseSessionCommit();
        return written;
    }

    private void LSentenceOwnerAttach(string table, string column, long ownerId, long exampleId, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = LSentenceScopeCreate(column);
        IReadOnlyList<long> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, ownerId, $"{table}_id");

        long id;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, example_ref, position)
                VALUES ($owner, $example, $position)
                RETURNING {table}_id;
                """;
            command.Parameters.AddWithValue("$owner", ownerId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.Parameters.AddWithValue("$position", current.Count);
            id = (long)command.ExecuteScalar()!;
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, ownerId, $"{table}_id",
            LDatabaseOrder.LDatabaseOrderInsert(current, id, position));

        session.LDatabaseSessionCommit();
    }

    private void LSentenceOwnerDetach(string table, string column, long ownerId, long exampleId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = LSentenceScopeCreate(column);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"DELETE FROM {table} WHERE {column} = $owner AND example_ref = $example;";
            command.Parameters.AddWithValue("$owner", ownerId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, ownerId, $"{table}_id",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, ownerId, $"{table}_id"));

        session.LDatabaseSessionCommit();
    }
}
