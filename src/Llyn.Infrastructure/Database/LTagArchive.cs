using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LTagArchive
{
    private readonly LDatabase _lTagArchiveDatabase;

    public LTagArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lTagArchiveDatabase = database;
    }

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

    public LTag? LTagRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        return LTagSingleRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyList<LTag> LTagSenseRead(string senseId)
    {
        return LTagReferrerRead("sense_tag", "sense_id", senseId);
    }

    public IReadOnlyList<LTag> LTagCollocationRead(string collocationId)
    {
        return LTagReferrerRead("collocation_tag", "collocation_id", collocationId);
    }

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

    public int LTagReferenceRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        return LTagReferenceRead(session.LDatabaseSessionConnection, id);
    }

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

    public void LTagSenseAttach(string senseId, string tagId, int position)
    {
        LTagReferenceAttach("sense_tag", "sense_id", senseId, tagId, position);
    }

    public void LTagCollocationAttach(string collocationId, string tagId, int position)
    {
        LTagReferenceAttach("collocation_tag", "collocation_id", collocationId, tagId, position);
    }

    public void LTagSenseDetach(string senseId, string tagId)
    {
        LTagReferenceDetach("sense_tag", "sense_id", senseId, tagId);
    }

    public void LTagCollocationDetach(string collocationId, string tagId)
    {
        LTagReferenceDetach("collocation_tag", "collocation_id", collocationId, tagId);
    }

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
