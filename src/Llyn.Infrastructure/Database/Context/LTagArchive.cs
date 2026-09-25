using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LTagArchive : LTagVault
{
    private readonly LDatabase _lTagArchiveDatabase;

    public LTagArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lTagArchiveDatabase = database;
    }

    public IReadOnlyList<LTag> LTagMeaningRead(long meaningId)
    {
        return LTagReferrerRead("sense_tag", "sense_parent", meaningId);
    }

    public IReadOnlyList<LTag> LTagCollocationRead(long collocationId)
    {
        return LTagReferrerRead("collocation_tag", "collocation_parent", collocationId);
    }

    public IReadOnlyList<long> LTagMeaningSave(long meaningId, IReadOnlyList<LTag> tags)
    {
        return LTagReferrerSave("sense_tag", "sense_parent", meaningId, tags);
    }

    public IReadOnlyList<long> LTagCollocationSave(long collocationId, IReadOnlyList<LTag> tags)
    {
        return LTagReferrerSave("collocation_tag", "collocation_parent", collocationId, tags);
    }

    public LTag? LTagRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT tag_id, text FROM tag WHERE tag_id = $id;";
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
        command.CommandText = "SELECT tag_id, text FROM tag ORDER BY text;";

        List<LTag> tags = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            tags.Add(LTagRowRead(reader));
        }

        return tags;
    }

    private static LTag LTagRowRead(SqliteDataReader reader)
    {
        return new LTag(reader.GetInt64(0), reader.GetString(1));
    }

    private static long LTagFind(SqliteConnection connection, string text)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT tag_id FROM tag WHERE text = $text;";
        command.Parameters.AddWithValue("$text", text);
        return command.ExecuteScalar() is long id ? id : 0;
    }

    private static bool LTagExist(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM tag WHERE tag_id = $id;";
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
        command.CommandText = "INSERT INTO tag (text) VALUES ($text) RETURNING tag_id;";
        command.Parameters.AddWithValue("$text", text);
        return (long)command.ExecuteScalar()!;
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
                $"INSERT INTO {table} ({column}, tag_ref, position) VALUES ($owner, $tag, $position);";
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
            SELECT tag.tag_id, tag.text
            FROM {table} link
            JOIN tag ON tag.tag_id = link.tag_ref
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
