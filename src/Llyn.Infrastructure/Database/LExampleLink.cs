using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Owns the three association tables that reach an Example — from an Entry, a Meaning, or a Collocation.
/// It lives beside <see cref="LExampleArchive"/> rather than inside it because attaching is a different
/// responsibility from storing: nothing here creates, changes, or deletes an Example, and every method
/// writes only the rows that link one to a referrer.
/// <para>
/// Each association carries the position the Example takes <em>for that referrer</em>, so the same
/// Example may be first under an Entry and third under a Meaning. That order is a unique index, so
/// attaching, detaching, and moving all renumber the referrer's whole set through
/// <see cref="LDatabaseOrder"/>; a caller therefore names the index it wants and never has to leave a
/// gap or find a free position.
/// </para>
/// </summary>
public sealed class LExampleLink
{
    private readonly LDatabase _lExampleLinkDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens sessions through.</summary>
    public LExampleLink(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lExampleLinkDatabase = database;
    }

    /// <summary>Reads the Examples an Entry references, in the order that Entry gives them.</summary>
    public IReadOnlyList<LExample> LExampleEntryRead(string entryId)
    {
        return LExampleReferrerRead("entry_example", "entry_id", entryId);
    }

    /// <summary>Reads the Examples a Meaning references, in the order that Meaning gives them.</summary>
    public IReadOnlyList<LExample> LExampleSenseRead(string senseId)
    {
        return LExampleReferrerRead("sense_example", "sense_id", senseId);
    }

    /// <summary>Reads the Examples a Collocation references, in the order that Collocation gives them.</summary>
    public IReadOnlyList<LExample> LExampleCollocationRead(string collocationId)
    {
        return LExampleReferrerRead("collocation_example", "collocation_id", collocationId);
    }

    /// <summary>References an existing Example from an Entry at <paramref name="position"/> in that Entry's order.</summary>
    public void LExampleEntryAttach(string entryId, string exampleId, int position)
    {
        LExampleReferenceAttach("entry_example", "entry_id", entryId, exampleId, position);
    }

    /// <summary>References an existing Example from a Meaning at <paramref name="position"/> in that Meaning's order.</summary>
    public void LExampleSenseAttach(string senseId, string exampleId, int position)
    {
        LExampleReferenceAttach("sense_example", "sense_id", senseId, exampleId, position);
    }

    /// <summary>References an existing Example from a Collocation at <paramref name="position"/> in that Collocation's order.</summary>
    public void LExampleCollocationAttach(string collocationId, string exampleId, int position)
    {
        LExampleReferenceAttach("collocation_example", "collocation_id", collocationId, exampleId, position);
    }

    /// <summary>Removes an Entry's reference to an Example. The Example and its other references survive.</summary>
    public void LExampleEntryDetach(string entryId, string exampleId)
    {
        LExampleReferenceDetach("entry_example", "entry_id", entryId, exampleId);
    }

    /// <summary>Removes a Meaning's reference to an Example. The Example and its other references survive.</summary>
    public void LExampleSenseDetach(string senseId, string exampleId)
    {
        LExampleReferenceDetach("sense_example", "sense_id", senseId, exampleId);
    }

    /// <summary>Removes a Collocation's reference to an Example. The Example and its other references survive.</summary>
    public void LExampleCollocationDetach(string collocationId, string exampleId)
    {
        LExampleReferenceDetach("collocation_example", "collocation_id", collocationId, exampleId);
    }

    // The three association tables differ only in their name and their referrer column, so the reference
    // operations share one implementation each. Both identifiers are store-owned literals chosen by the
    // methods above, never caller input, so composing them into the statement text opens no injection
    // seam; every value still travels as a parameter.
    //
    // The row is inserted beyond the end of the set and the whole set is then renumbered around it, so
    // the requested index is honoured — and a position that is already taken is no longer a unique-index
    // failure the caller has to work around.
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

    // One query for the referrer's Examples and one for all of their translations, rather than a
    // round-trip per Example.
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
            SELECT example.id, example.language, example.text, example.local, example.source_id
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
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                translations.TryGetValue(id, out IReadOnlyList<LTranslation>? found) ? found : []));
        }

        return examples;
    }
}
