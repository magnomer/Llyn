using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LExampleLink
{
    private readonly LDatabase _lExampleLinkDatabase;

    public LExampleLink(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lExampleLinkDatabase = database;
    }

    public IReadOnlyList<LExample> LExampleEntryRead(string entryId)
    {
        return LExampleReferrerRead("entry_example", "entry_id", entryId);
    }

    public IReadOnlyList<LExample> LExampleMeaningRead(string meaningId)
    {
        return LExampleReferrerRead("sense_example", "sense_id", meaningId);
    }

    public IReadOnlyList<LExample> LExampleCollocationRead(string collocationId)
    {
        return LExampleReferrerRead("collocation_example", "collocation_id", collocationId);
    }

    public void LExampleEntryAttach(string entryId, string exampleId, int position)
    {
        LExampleReferenceAttach("entry_example", "entry_id", entryId, exampleId, position);
    }

    public void LExampleMeaningAttach(string meaningId, string exampleId, int position)
    {
        LExampleReferenceAttach("sense_example", "sense_id", meaningId, exampleId, position);
    }

    public void LExampleCollocationAttach(string collocationId, string exampleId, int position)
    {
        LExampleReferenceAttach("collocation_example", "collocation_id", collocationId, exampleId, position);
    }

    public void LExampleEntryDetach(string entryId, string exampleId)
    {
        LExampleReferenceDetach("entry_example", "entry_id", entryId, exampleId);
    }

    public void LExampleMeaningDetach(string meaningId, string exampleId)
    {
        LExampleReferenceDetach("sense_example", "sense_id", meaningId, exampleId);
    }

    public void LExampleCollocationDetach(string collocationId, string exampleId)
    {
        LExampleReferenceDetach("collocation_example", "collocation_id", collocationId, exampleId);
    }

    public IReadOnlyList<LUsage> LExampleUsageRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lExampleLinkDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LUsage> usages = [];
        usages.AddRange(LExampleUsageRead(
            connection,
            id,
            LOwner.LOwnerEntry,
            """
            SELECT link.entry_id, entry.id, entry.headword, entry.language,
                   'unspecified', NULL,
                   COALESCE((
                       SELECT CASE WHEN sense.gloss IS NOT NULL THEN 'specified' ELSE sense.definition_state END
                       FROM sense WHERE sense.entry_id = entry.id ORDER BY sense.position LIMIT 1), 'unspecified'),
                   (SELECT COALESCE(sense.gloss, sense.definition)
                    FROM sense WHERE sense.entry_id = entry.id ORDER BY sense.position LIMIT 1)
            FROM entry_example link
            JOIN entry ON entry.id = link.entry_id
            WHERE link.example_id = $id
            ORDER BY entry.headword;
            """));
        usages.AddRange(LExampleUsageRead(
            connection,
            id,
            LOwner.LOwnerMeaning,
            """
            SELECT link.sense_id, sense.entry_id, entry.headword, entry.language,
                   sense.title_state, sense.title,
                   CASE WHEN sense.gloss IS NOT NULL THEN 'specified' ELSE sense.definition_state END,
                   COALESCE(sense.gloss, sense.definition)
            FROM sense_example link
            JOIN sense ON sense.id = link.sense_id
            JOIN entry ON entry.id = sense.entry_id
            WHERE link.example_id = $id
            ORDER BY entry.headword, sense.position;
            """));
        usages.AddRange(LExampleUsageRead(
            connection,
            id,
            LOwner.LOwnerCollocation,
            """
            SELECT link.collocation_id, collocation.entry_id, entry.headword, entry.language,
                   collocation.title_state, collocation.title,
                   collocation.expression_state, collocation.expression
            FROM collocation_example link
            JOIN collocation ON collocation.id = link.collocation_id
            JOIN entry ON entry.id = collocation.entry_id
            WHERE link.example_id = $id
            ORDER BY entry.headword, collocation.position;
            """));

        return usages;
    }

    private static IReadOnlyList<LUsage> LExampleUsageRead(
        SqliteConnection connection,
        string id,
        LOwner owner,
        string statement)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = statement;
        command.Parameters.AddWithValue("$id", id);

        List<LUsage> usages = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            LStateValue title = LStateColumn.LStateColumnRead(reader, 4);
            if (title.LStateValueEmpty)
            {
                title = LStateColumn.LStateColumnRead(reader, 6);
            }

            usages.Add(new LUsage(
                reader.GetString(0),
                owner,
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                title));
        }

        return usages;
    }

    internal static void LExampleLinkClear(SqliteConnection connection, string exampleId)
    {
        LExampleLinkClear(connection, "entry_example", "entry_id", exampleId);
        LExampleLinkClear(connection, "sense_example", "sense_id", exampleId);
        LExampleLinkClear(connection, "collocation_example", "collocation_id", exampleId);
    }

    private static void LExampleLinkClear(
        SqliteConnection connection, string table, string column, string exampleId)
    {
        List<string> referrers = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"SELECT {column} FROM {table} WHERE example_id = $example;";
            command.Parameters.AddWithValue("$example", exampleId);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                referrers.Add(reader.GetString(0));
            }
        }

        if (referrers.Count == 0)
        {
            return;
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE example_id = $example;";
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        string scope = $"{column} = $owner";
        foreach (string referrer in referrers)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, table, scope, referrer, "example_id",
                LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrer, "example_id"));
        }
    }

    private void LExampleReferenceAttach(
        string table, string column, string referrerId, string exampleId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lExampleLinkDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{column} = $owner";
        IReadOnlyList<string> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, referrerId, "example_id");

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, example_id, position)
                VALUES ($referrer, $example, $position)
                ON CONFLICT ({column}, example_id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "example_id",
            LDatabaseOrder.LDatabaseOrderInsert(current, exampleId, position));

        session.LDatabaseSessionCommit();
    }

    private void LExampleReferenceDetach(string table, string column, string referrerId, string exampleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lExampleLinkDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{column} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"DELETE FROM {table} WHERE {column} = $referrer AND example_id = $example;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$example", exampleId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "example_id",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrerId, "example_id"));

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LExample> LExampleReferrerRead(string table, string column, string referrerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);

        using LDatabaseSession session = _lExampleLinkDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            SELECT example.id, example.language, example.text_state, example.text,
                   example.translation_state, example.translation,
                   example.source_state, example.source_id
            FROM {table} link
            JOIN example ON example.id = link.example_id
            WHERE link.{column} = $referrer
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LExample> examples = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            examples.Add(new LExample(
                reader.GetString(0),
                reader.GetString(1),
                LStateColumn.LStateColumnRead(reader, 2),
                LStateColumn.LStateColumnRead(reader, 4),
                LStateColumn.LStateColumnRead(reader, 6)));
        }

        return examples;
    }
}
