using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists Situations — independent data no Entry, Meaning, or Collocation owns. A Situation is created
/// once with an opaque id, then <em>referenced</em> by any number of Meanings and Collocations through
/// the association tables, each carrying the position the Situation takes for that referrer alone.
/// Attaching and detaching therefore only ever write association rows: detaching leaves the Situation and
/// its other references untouched, updating rewrites the visible title, description, and kind and never
/// the id, and <see cref="LSituationDelete"/> refuses to run while any reference remains.
/// <para>
/// A referrer's order is a unique index, so attaching and detaching renumber that referrer's whole set
/// through <see cref="LDatabaseOrder"/>: a caller names the index it wants and never has to find a free
/// position or leave a gap behind.
/// </para>
/// </summary>
public sealed class LSituationArchive
{
    private readonly LDatabase _lSituationArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens sessions through.</summary>
    public LSituationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSituationArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="situation"/> with a fresh opaque id and returns the stored Situation with
    /// that id filled in. The new Situation is referenced by nothing until it is attached to a referrer.
    /// </summary>
    public LSituation LSituationCreate(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);
        ArgumentException.ThrowIfNullOrWhiteSpace(situation.LSituationTitle);

        LSituation stored = situation with { LSituationId = LIdentity.LIdentityCreate() };

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO situation (id, title, description, kind)
                VALUES ($id, $title, $description, $kind);
                """;
            command.Parameters.AddWithValue("$id", stored.LSituationId);
            command.Parameters.AddWithValue("$title", stored.LSituationTitle);
            command.Parameters.AddWithValue("$description", (object?)stored.LSituationDescription ?? DBNull.Value);
            command.Parameters.AddWithValue("$kind", (object?)stored.LSituationKind ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    /// <summary>
    /// Reads the Situation identified by <paramref name="id"/>, or <c>null</c> when no such Situation
    /// exists.
    /// </summary>
    public LSituation? LSituationRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        return LSituationSingleRead(session.LDatabaseSessionConnection, id);
    }

    /// <summary>Reads the Situations a Meaning references, in the order that Meaning gives them.</summary>
    public IReadOnlyList<LSituation> LSituationSenseRead(string senseId)
    {
        return LSituationReferrerRead("sense_situation", "sense_id", senseId);
    }

    /// <summary>Reads the Situations a Collocation references, in the order that Collocation gives them.</summary>
    public IReadOnlyList<LSituation> LSituationCollocationRead(string collocationId)
    {
        return LSituationReferrerRead("collocation_situation", "collocation_id", collocationId);
    }

    /// <summary>
    /// Rewrites the visible title, description, and kind of the Situation identified by
    /// <paramref name="situation"/>'s id. The id and every reference pointing at it are untouched, so an
    /// update never changes where the Situation appears or in what order. Throws when no Situation
    /// carries that id.
    /// </summary>
    public void LSituationUpdate(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);
        ArgumentException.ThrowIfNullOrWhiteSpace(situation.LSituationId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE situation
                SET title = $title, description = $description, kind = $kind
                WHERE id = $id;
                """;
            command.Parameters.AddWithValue("$title", situation.LSituationTitle);
            command.Parameters.AddWithValue("$description", (object?)situation.LSituationDescription ?? DBNull.Value);
            command.Parameters.AddWithValue("$kind", (object?)situation.LSituationKind ?? DBNull.Value);
            command.Parameters.AddWithValue("$id", situation.LSituationId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Situation carries the id '{situation.LSituationId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Deletes the Situation identified by <paramref name="id"/>. Guarded: while any Meaning or
    /// Collocation still references the Situation, nothing is deleted and an
    /// <see cref="InvalidOperationException"/> is thrown — detach every reference first. Deleting a
    /// Situation never deletes the rows that referenced it. The guard and the delete share one
    /// transaction, so nothing can attach the Situation between them.
    /// </summary>
    public void LSituationDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand guard = connection.CreateCommand())
        {
            guard.CommandText =
                """
                SELECT
                    (SELECT COUNT(*) FROM sense_situation WHERE situation_id = $id)
                    + (SELECT COUNT(*) FROM collocation_situation WHERE situation_id = $id);
                """;
            guard.Parameters.AddWithValue("$id", id);
            long references = Convert.ToInt64(guard.ExecuteScalar());
            if (references > 0)
            {
                throw new InvalidOperationException(
                    $"Situation {id} is still referenced {references} time(s); detach every reference before deleting it.");
            }
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM situation WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    /// <summary>References an existing Situation from a Meaning at <paramref name="position"/> in that Meaning's order.</summary>
    public void LSituationSenseAttach(string senseId, string situationId, int position)
    {
        LSituationReferenceAttach("sense_situation", "sense_id", senseId, situationId, position);
    }

    /// <summary>References an existing Situation from a Collocation at <paramref name="position"/> in that Collocation's order.</summary>
    public void LSituationCollocationAttach(string collocationId, string situationId, int position)
    {
        LSituationReferenceAttach("collocation_situation", "collocation_id", collocationId, situationId, position);
    }

    /// <summary>Removes a Meaning's reference to a Situation. The Situation and its other references survive.</summary>
    public void LSituationSenseDetach(string senseId, string situationId)
    {
        LSituationReferenceDetach("sense_situation", "sense_id", senseId, situationId);
    }

    /// <summary>Removes a Collocation's reference to a Situation. The Situation and its other references survive.</summary>
    public void LSituationCollocationDetach(string collocationId, string situationId)
    {
        LSituationReferenceDetach("collocation_situation", "collocation_id", collocationId, situationId);
    }

    // The two association tables differ only in their name and their referrer column, so the reference
    // operations share one implementation each. Both identifiers are store-owned literals chosen by the
    // methods above, never caller input, so composing them into the statement text opens no injection
    // seam; every value still travels as a parameter.
    //
    // The row goes in beyond the end of the set and the whole set is then renumbered around it, so the
    // requested index is honoured and an occupied position is no longer a unique-index failure.
    private void LSituationReferenceAttach(
        string table,
        string column,
        string referrerId,
        string situationId,
        int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(situationId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{column} = $owner";
        IReadOnlyList<string> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, referrerId, "situation_id");

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, situation_id, position)
                VALUES ($referrer, $situation, $position)
                ON CONFLICT ({column}, situation_id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$situation", situationId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "situation_id",
            LDatabaseOrder.LDatabaseOrderInsert(current, situationId, position));

        session.LDatabaseSessionCommit();
    }

    private void LSituationReferenceDetach(string table, string column, string referrerId, string situationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(situationId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{column} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"DELETE FROM {table} WHERE {column} = $referrer AND situation_id = $situation;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$situation", situationId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "situation_id",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrerId, "situation_id"));

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LSituation> LSituationReferrerRead(string table, string column, string referrerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT situation.id, situation.title, situation.description, situation.kind
            FROM {table} link
            JOIN situation ON situation.id = link.situation_id
            WHERE link.{column} = $referrer
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LSituation> situations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            situations.Add(new LSituation(
                reader.GetString(0),
                reader.GetString(1),
                reader.IsDBNull(2) ? null : reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3)));
        }

        return situations;
    }

    private static LSituation? LSituationSingleRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT title, description, kind FROM situation WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LSituation(
            id,
            reader.GetString(0),
            reader.IsDBNull(1) ? null : reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetString(2));
    }
}
