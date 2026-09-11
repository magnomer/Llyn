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

    public IReadOnlyList<LSentence> LSentenceMeaningRead(long meaningId)
    {
        return LSentenceOwnerRead("sense_example", "sense_id", meaningId);
    }

    public IReadOnlyList<LSentence> LSentenceCollocationRead(long collocationId)
    {
        return LSentenceOwnerRead("collocation_example", "collocation_id", collocationId);
    }

    public void LSentenceMeaningSave(long meaningId, IReadOnlyList<LSentence> sentences)
    {
        LSentenceOwnerSave("sense_example", "sense_id", meaningId, sentences);
    }

    public void LSentenceCollocationSave(long collocationId, IReadOnlyList<LSentence> sentences)
    {
        LSentenceOwnerSave("collocation_example", "collocation_id", collocationId, sentences);
    }

    public void LSentenceMeaningAttach(long meaningId, long exampleId, int position)
    {
        LSentenceOwnerAttach("sense_example", "sense_id", meaningId, exampleId, position);
    }

    public void LSentenceCollocationAttach(long collocationId, long exampleId, int position)
    {
        LSentenceOwnerAttach("collocation_example", "collocation_id", collocationId, exampleId, position);
    }

    public void LSentenceMeaningDetach(long meaningId, long exampleId)
    {
        LSentenceOwnerDetach("sense_example", "sense_id", meaningId, exampleId);
    }

    public void LSentenceCollocationDetach(long collocationId, long exampleId)
    {
        LSentenceOwnerDetach("collocation_example", "collocation_id", collocationId, exampleId);
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
        LSentenceTableClear(connection, "sense_example", "sense_id", exampleId);
        LSentenceTableClear(connection, "collocation_example", "collocation_id", exampleId);
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
                SELECT hold.{column}, hold.{column}_state, sense.entry_id
                FROM sense_example hold
                JOIN sense ON sense.id = hold.sense_id
                UNION ALL
                SELECT hold.{column}, hold.{column}_state, collocation.entry_id
                FROM collocation_example hold
                JOIN collocation ON collocation.id = hold.collocation_id
            ) link
            JOIN entry ON entry.id = link.entry_id
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
            SELECT link.id, link.{column}, link.position,
                   example.id, example.language, example.text_state, example.text,
                   example.translation_state, example.translation,
                   example.source_state, example.source_id,
                   link.particle_state, link.particle,
                   link.dependence_state, link.dependence
            FROM {table} link
            LEFT JOIN example ON example.id = link.example_id
            WHERE link.{column} = $owner
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$owner", ownerId);

        List<LSentence> sentences = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            sentences.Add(new LSentence(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt32(2),
                reader.IsDBNull(3) ? null : LSentenceExampleRead(reader, 3),
                LStateColumn.LStateColumnRead(reader, 11),
                LStateColumn.LStateColumnRead(reader, 13)));
        }

        return sentences;
    }

    private static void LSentenceOwnerClear(
        SqliteConnection connection, string table, string column, long ownerId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {table} WHERE {column} = $owner;";
        command.Parameters.AddWithValue("$owner", ownerId);
        command.ExecuteNonQuery();
    }

    private static void LSentenceTableClear(
        SqliteConnection connection, string table, string column, long exampleId)
    {
        List<long> owners = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"SELECT DISTINCT {column} FROM {table} WHERE example_id = $example;";
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
            command.CommandText = $"DELETE FROM {table} WHERE example_id = $example;";
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        string scope = LSentenceScopeCreate(column);
        foreach (long owner in owners)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, table, scope, owner, "id",
                LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, owner, "id"));
        }
    }

    private static void LSentenceSave(
        SqliteConnection connection, string table, string column, long ownerId, LSentence sentence, int position)
    {
        ArgumentNullException.ThrowIfNull(sentence);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            INSERT INTO {table} (
                {column}, example_id, position,
                particle_state, particle,
                dependence_state, dependence)
            VALUES (
                $owner, $example, $position,
                $particleState, $particle,
                $dependenceState, $dependence);
            """;
        command.Parameters.AddWithValue("$owner", ownerId);
        command.Parameters.AddWithValue(
            "$example", (object?)sentence.LSentenceExample?.LExampleId ?? DBNull.Value);
        command.Parameters.AddWithValue("$position", position);
        LStateColumn.LStateColumnApply(command, "particle", sentence.LSentenceParticle);
        LStateColumn.LStateColumnApply(command, "dependence", sentence.LSentenceDependence);
        command.ExecuteNonQuery();
    }

    private static LExample LSentenceExampleRead(SqliteDataReader reader, int start)
    {
        return new LExample(
            reader.GetInt64(start),
            reader.GetString(start + 1),
            LStateColumn.LStateColumnRead(reader, start + 2),
            LStateColumn.LStateColumnRead(reader, start + 4),
            LStateColumn.LStateColumnAnchorRead(reader, start + 6));
    }

    private IReadOnlyList<LSentence> LSentenceOwnerRead(string table, string column, long ownerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        return LSentenceOwnerRead(session.LDatabaseSessionConnection, table, column, ownerId);
    }

    private void LSentenceOwnerSave(string table, string column, long ownerId, IReadOnlyList<LSentence> sentences)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
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

    private void LSentenceOwnerAttach(string table, string column, long ownerId, long exampleId, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);

        using LDatabaseSession session = _lSentenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = LSentenceScopeCreate(column);
        IReadOnlyList<long> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, ownerId, "id");

        long id;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, example_id, position)
                VALUES ($owner, $example, $position)
                RETURNING id;
                """;
            command.Parameters.AddWithValue("$owner", ownerId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.Parameters.AddWithValue("$position", current.Count);
            id = (long)command.ExecuteScalar()!;
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, ownerId, "id",
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
