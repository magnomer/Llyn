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

    public IReadOnlyList<LExample> LExampleSenseRead(string senseId)
    {
        return LExampleReferrerRead("sense_example", "sense_id", senseId);
    }

    public IReadOnlyList<LExample> LExampleCollocationRead(string collocationId)
    {
        return LExampleReferrerRead("collocation_example", "collocation_id", collocationId);
    }

    public void LExampleEntryAttach(string entryId, string exampleId, int position)
    {
        LExampleReferenceAttach("entry_example", "entry_id", entryId, exampleId, position);
    }

    public void LExampleSenseAttach(string senseId, string exampleId, int position)
    {
        LExampleReferenceAttach("sense_example", "sense_id", senseId, exampleId, position);
    }

    public void LExampleCollocationAttach(string collocationId, string exampleId, int position)
    {
        LExampleReferenceAttach("collocation_example", "collocation_id", collocationId, exampleId, position);
    }

    public void LExampleEntryDetach(string entryId, string exampleId)
    {
        LExampleReferenceDetach("entry_example", "entry_id", entryId, exampleId);
    }

    public void LExampleSenseDetach(string senseId, string exampleId)
    {
        LExampleReferenceDetach("sense_example", "sense_id", senseId, exampleId);
    }

    public void LExampleCollocationDetach(string collocationId, string exampleId)
    {
        LExampleReferenceDetach("collocation_example", "collocation_id", collocationId, exampleId);
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

        IReadOnlyDictionary<string, IReadOnlyList<LTranslation>> translations =
            LExampleArchive.LExampleTranslationRead(connection, table, column, referrerId);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            SELECT example.id, example.language, example.text_state, example.text, example.local,
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
            string id = reader.GetString(0);
            examples.Add(new LExample(
                id,
                reader.GetString(1),
                LStateColumn.LStateColumnRead(reader, 2),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                LStateColumn.LStateColumnRead(reader, 5),
                translations.TryGetValue(id, out IReadOnlyList<LTranslation>? found) ? found : []));
        }

        return examples;
    }
}
