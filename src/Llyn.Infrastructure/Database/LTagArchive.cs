using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists Tags — independent data no Entry, Meaning, or Collocation owns. A Tag is created once with
/// an opaque id, then <em>referenced</em> by any number of Meanings and Collocations through the
/// association tables, each carrying the position the Tag takes for that referrer alone. Attaching and
/// detaching therefore only ever write association rows: detaching leaves the Tag and its other
/// references untouched, updating rewrites the visible text and never the id, and
/// <see cref="LTagDelete"/> refuses to run while any reference remains.
/// <para>
/// A referrer's order is a unique index, so attaching and detaching renumber that referrer's whole set
/// through <see cref="LDatabaseOrder"/>: a caller names the index it wants and never has to find a free
/// position or leave a gap behind.
/// </para>
/// </summary>
public sealed class LTagArchive
{
    private readonly LDatabase _lTagArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens sessions through.</summary>
    public LTagArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lTagArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="tag"/> with a fresh opaque id and returns the stored Tag with that id
    /// filled in. The new Tag is referenced by nothing until it is attached to a referrer.
    /// </summary>
    public LTag LTagCreate(LTag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentException.ThrowIfNullOrWhiteSpace(tag.LTagText);

        LTag stored = tag with { LTagId = LIdentity.LIdentityCreate() };

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "INSERT INTO tag (id, text) VALUES ($id, $text);";
            command.Parameters.AddWithValue("$id", stored.LTagId);
            command.Parameters.AddWithValue("$text", stored.LTagText);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    /// <summary>Reads the Tag identified by <paramref name="id"/>, or <c>null</c> when no such Tag exists.</summary>
    public LTag? LTagRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        return LTagSingleRead(session.LDatabaseSessionConnection, id);
    }

    /// <summary>Reads the Tags a Meaning references, in the order that Meaning gives them.</summary>
    public IReadOnlyList<LTag> LTagSenseRead(string senseId)
    {
        return LTagReferrerRead("sense_tag", "sense_id", senseId);
    }

    /// <summary>Reads the Tags a Collocation references, in the order that Collocation gives them.</summary>
    public IReadOnlyList<LTag> LTagCollocationRead(string collocationId)
    {
        return LTagReferrerRead("collocation_tag", "collocation_id", collocationId);
    }

    /// <summary>
    /// Rewrites the visible text of the Tag identified by <paramref name="tag"/>'s id. The id and every
    /// reference pointing at it are untouched, so an update never changes where the Tag appears or in
    /// what order. Throws when no Tag carries that id.
    /// </summary>
    public void LTagUpdate(LTag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentException.ThrowIfNullOrWhiteSpace(tag.LTagId);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "UPDATE tag SET text = $text WHERE id = $id;";
            command.Parameters.AddWithValue("$text", tag.LTagText);
            command.Parameters.AddWithValue("$id", tag.LTagId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Tag carries the id '{tag.LTagId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Counts the references that still point at the Tag identified by <paramref name="id"/> — the
    /// number <see cref="LTagDelete"/> refuses a delete over. A caller that has just detached one
    /// reference reads this to learn whether the row it detached from was the last one, without a store
    /// of its own having to know which association tables exist.
    /// </summary>
    public int LTagReferenceRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        return LTagReferenceRead(session.LDatabaseSessionConnection, id);
    }

    // The same count on a connection the caller already holds, so a guard and the delete it guards run
    // in one transaction and nothing can attach the row between them.
    private static int LTagReferenceRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM sense_tag WHERE tag_id = $id)
                + (SELECT COUNT(*) FROM collocation_tag WHERE tag_id = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>
    /// Deletes the Tag identified by <paramref name="id"/>. Guarded: while any Meaning or Collocation
    /// still references the Tag, nothing is deleted and an <see cref="InvalidOperationException"/> is
    /// thrown — detach every reference first. Deleting a Tag never deletes the rows that referenced it.
    /// The guard and the delete share one transaction, so nothing can attach the Tag between them.
    /// </summary>
    public void LTagDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        int references = LTagReferenceRead(connection, id);
        if (references > 0)
        {
            throw new InvalidOperationException(
                $"Tag {id} is still referenced {references} time(s); detach every reference before deleting it.");
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM tag WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    /// <summary>References an existing Tag from a Meaning at <paramref name="position"/> in that Meaning's order.</summary>
    public void LTagSenseAttach(string senseId, string tagId, int position)
    {
        LTagReferenceAttach("sense_tag", "sense_id", senseId, tagId, position);
    }

    /// <summary>References an existing Tag from a Collocation at <paramref name="position"/> in that Collocation's order.</summary>
    public void LTagCollocationAttach(string collocationId, string tagId, int position)
    {
        LTagReferenceAttach("collocation_tag", "collocation_id", collocationId, tagId, position);
    }

    /// <summary>Removes a Meaning's reference to a Tag. The Tag and its other references survive.</summary>
    public void LTagSenseDetach(string senseId, string tagId)
    {
        LTagReferenceDetach("sense_tag", "sense_id", senseId, tagId);
    }

    /// <summary>Removes a Collocation's reference to a Tag. The Tag and its other references survive.</summary>
    public void LTagCollocationDetach(string collocationId, string tagId)
    {
        LTagReferenceDetach("collocation_tag", "collocation_id", collocationId, tagId);
    }

    // The two association tables differ only in their name and their referrer column, so the reference
    // operations share one implementation each. Both identifiers are store-owned literals chosen by the
    // methods above, never caller input, so composing them into the statement text opens no injection
    // seam; every value still travels as a parameter.
    //
    // The row goes in beyond the end of the set and the whole set is then renumbered around it, so the
    // requested index is honoured and an occupied position is no longer a unique-index failure.
    private void LTagReferenceAttach(string table, string column, string referrerId, string tagId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tagId);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{column} = $owner";
        IReadOnlyList<string> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, referrerId, "tag_id");

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, tag_id, position)
                VALUES ($referrer, $tag, $position)
                ON CONFLICT ({column}, tag_id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$tag", tagId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "tag_id",
            LDatabaseOrder.LDatabaseOrderInsert(current, tagId, position));

        session.LDatabaseSessionCommit();
    }

    private void LTagReferenceDetach(string table, string column, string referrerId, string tagId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tagId);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{column} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE {column} = $referrer AND tag_id = $tag;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$tag", tagId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "tag_id",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrerId, "tag_id"));

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LTag> LTagReferrerRead(string table, string column, string referrerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT tag.id, tag.text
            FROM {table} link
            JOIN tag ON tag.id = link.tag_id
            WHERE link.{column} = $referrer
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LTag> tags = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            tags.Add(new LTag(reader.GetString(0), reader.GetString(1)));
        }

        return tags;
    }

    private static LTag? LTagSingleRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT text FROM tag WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LTag(id, reader.GetString(0));
    }
}
