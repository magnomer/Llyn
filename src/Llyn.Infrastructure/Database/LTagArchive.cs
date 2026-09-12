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

    public IReadOnlyList<LTag> LTagMeaningRead(long meaningId)
    {
        return LTagReferrerRead("sense_tag", "sense_id", meaningId);
    }

    public IReadOnlyList<LTag> LTagCollocationRead(long collocationId)
    {
        return LTagReferrerRead("collocation_tag", "collocation_id", collocationId);
    }

    public IReadOnlyList<long> LTagMeaningSave(long meaningId, IReadOnlyList<LTag> tags)
    {
        return LTagReferrerSave("sense_tag", "sense_id", meaningId, tags);
    }

    public IReadOnlyList<long> LTagCollocationSave(long collocationId, IReadOnlyList<LTag> tags)
    {
        return LTagReferrerSave("collocation_tag", "collocation_id", collocationId, tags);
    }

    public LTag? LTagRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT id, text FROM tag WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LTagRowRead(reader) : null;
    }

    public long LTagResolve(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        long id = LTagResolve(session.LDatabaseSessionConnection, text.Trim());
        session.LDatabaseSessionCommit();
        return id;
    }

    public IReadOnlyList<LTag> LTagCatalogRead()
    {
        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT id, text FROM tag ORDER BY text;";

        List<LTag> tags = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            tags.Add(LTagRowRead(reader));
        }

        return tags;
    }

    public int LTagReferenceRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM sense_tag WHERE tag_id = $id)
                + (SELECT COUNT(*) FROM collocation_tag WHERE tag_id = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void LTagChange(long id, string renamed)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(renamed);

        string text = renamed.Trim();

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        long clash = LTagFind(connection, text);
        if (clash == id)
        {
            return;
        }

        if (clash == 0)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE tag SET text = $text WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$text", text);
            command.ExecuteNonQuery();
            session.LDatabaseSessionCommit();
            return;
        }

        LTagLinkMove(connection, "sense_tag", "sense_id", id, clash);
        LTagLinkMove(connection, "collocation_tag", "collocation_id", id, clash);
        LTagRowDelete(connection, id);

        session.LDatabaseSessionCommit();
    }

    public void LTagDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LTagLinkDelete(connection, "sense_tag", "sense_id", id);
        LTagLinkDelete(connection, "collocation_tag", "collocation_id", id);
        LTagRowDelete(connection, id);

        session.LDatabaseSessionCommit();
    }

    private static LTag LTagRowRead(SqliteDataReader reader)
    {
        return new LTag(reader.GetInt64(0), reader.GetString(1));
    }

    private static long LTagFind(SqliteConnection connection, string text)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id FROM tag WHERE text = $text;";
        command.Parameters.AddWithValue("$text", text);
        return command.ExecuteScalar() is long id ? id : 0;
    }

    private static bool LTagExist(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM tag WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    private static long LTagResolve(SqliteConnection connection, string text)
    {
        long found = LTagFind(connection, text);
        if (found != 0)
        {
            return found;
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO tag (text) VALUES ($text) RETURNING id;";
        command.Parameters.AddWithValue("$text", text);
        return (long)command.ExecuteScalar()!;
    }

    private static void LTagRowDelete(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM tag WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    private static IReadOnlyList<long> LTagOwnerRead(
        SqliteConnection connection, string table, string column, long tagId)
    {
        List<long> owners = [];
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"SELECT {column} FROM {table} WHERE tag_id = $tag;";
        command.Parameters.AddWithValue("$tag", tagId);
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            owners.Add(reader.GetInt64(0));
        }

        return owners;
    }

    private static void LTagLinkMove(
        SqliteConnection connection, string table, string column, long id, long clash)
    {
        IReadOnlyList<long> owners = LTagOwnerRead(connection, table, column, id);

        using (SqliteCommand carried = connection.CreateCommand())
        {
            carried.CommandText =
                $"""
                DELETE FROM {table}
                WHERE tag_id = $id
                  AND EXISTS (
                      SELECT 1 FROM {table} kept
                      WHERE kept.{column} = {table}.{column} AND kept.tag_id = $clash);
                """;
            carried.Parameters.AddWithValue("$id", id);
            carried.Parameters.AddWithValue("$clash", clash);
            carried.ExecuteNonQuery();
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"UPDATE {table} SET tag_id = $clash WHERE tag_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$clash", clash);
            command.ExecuteNonQuery();
        }

        LTagOwnerNormalize(connection, table, column, owners);
    }

    private static void LTagLinkDelete(
        SqliteConnection connection, string table, string column, long tagId)
    {
        IReadOnlyList<long> owners = LTagOwnerRead(connection, table, column, tagId);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE tag_id = $tag;";
            command.Parameters.AddWithValue("$tag", tagId);
            command.ExecuteNonQuery();
        }

        LTagOwnerNormalize(connection, table, column, owners);
    }

    private static void LTagOwnerNormalize(
        SqliteConnection connection, string table, string column, IReadOnlyList<long> owners)
    {
        string scope = $"{column} = $owner";
        foreach (long owner in owners)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, table, scope, owner, "tag_id",
                LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, owner, "tag_id"));
        }
    }

    private IReadOnlyList<long> LTagReferrerSave(
        string table, string column, long referrerId, IReadOnlyList<LTag> tags)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);
        ArgumentNullException.ThrowIfNull(tags);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand cleared = connection.CreateCommand())
        {
            cleared.CommandText = $"DELETE FROM {table} WHERE {column} = $referrer;";
            cleared.Parameters.AddWithValue("$referrer", referrerId);
            cleared.ExecuteNonQuery();
        }

        HashSet<long> written = [];
        List<long> resolved = new(tags.Count);
        int position = 0;
        foreach (LTag tag in tags)
        {
            ArgumentNullException.ThrowIfNull(tag);

            long id = tag.LTagId;
            if (id > 0 && !LTagExist(connection, id))
            {
                throw new LRefusal(LRefusal.LRefusalLink);
            }

            if (id <= 0)
            {
                string text = tag.LTagText.Trim();
                if (text.Length == 0)
                {
                    resolved.Add(0);
                    continue;
                }

                id = LTagResolve(connection, text);
            }

            resolved.Add(id);
            if (!written.Add(id))
            {
                continue;
            }

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                $"INSERT INTO {table} ({column}, tag_id, position) VALUES ($owner, $tag, $position);";
            command.Parameters.AddWithValue("$owner", referrerId);
            command.Parameters.AddWithValue("$tag", id);
            command.Parameters.AddWithValue("$position", position);
            command.ExecuteNonQuery();
            position++;
        }

        session.LDatabaseSessionCommit();
        return resolved;
    }

    private IReadOnlyList<LTag> LTagReferrerRead(string table, string column, long referrerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);

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
            tags.Add(LTagRowRead(reader));
        }

        return tags;
    }
}
