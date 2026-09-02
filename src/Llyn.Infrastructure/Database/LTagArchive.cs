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

    public IReadOnlyList<LTag> LTagSenseRead(string senseId)
    {
        return LTagReferrerRead("sense_tag", "sense_id", senseId);
    }

    public IReadOnlyList<LTag> LTagCollocationRead(string collocationId)
    {
        return LTagReferrerRead("collocation_tag", "collocation_id", collocationId);
    }

    public void LTagSenseSave(string senseId, IReadOnlyList<LTag> tags)
    {
        LTagReferrerSave("sense_tag", "sense_id", senseId, tags);
    }

    public void LTagCollocationSave(string collocationId, IReadOnlyList<LTag> tags)
    {
        LTagReferrerSave("collocation_tag", "collocation_id", collocationId, tags);
    }

    public IReadOnlyList<LTag> LTagCatalogRead()
    {
        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT text FROM sense_tag
            UNION
            SELECT text FROM collocation_tag
            ORDER BY text;
            """;

        List<LTag> tags = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            tags.Add(new LTag(reader.GetString(0)));
        }

        return tags;
    }

    public int LTagReferenceRead(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM sense_tag WHERE text = $text)
                + (SELECT COUNT(*) FROM collocation_tag WHERE text = $text);
            """;
        command.Parameters.AddWithValue("$text", text);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void LTagChange(string text, string renamed)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ArgumentException.ThrowIfNullOrWhiteSpace(renamed);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LTagChange(connection, "sense_tag", "sense_id", text.Trim(), renamed.Trim());
        LTagChange(connection, "collocation_tag", "collocation_id", text.Trim(), renamed.Trim());

        session.LDatabaseSessionCommit();
    }

    public void LTagDelete(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LTagDelete(connection, "sense_tag", "sense_id", text.Trim());
        LTagDelete(connection, "collocation_tag", "collocation_id", text.Trim());

        session.LDatabaseSessionCommit();
    }

    private static void LTagChange(
        SqliteConnection connection, string table, string column, string text, string renamed)
    {
        using (SqliteCommand carried = connection.CreateCommand())
        {
            carried.CommandText =
                $"""
                DELETE FROM {table}
                WHERE text = $text
                  AND EXISTS (
                      SELECT 1 FROM {table} kept
                      WHERE kept.{column} = {table}.{column} AND kept.text = $renamed);
                """;
            carried.Parameters.AddWithValue("$text", text);
            carried.Parameters.AddWithValue("$renamed", renamed);
            carried.ExecuteNonQuery();
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"UPDATE {table} SET text = $renamed WHERE text = $text;";
            command.Parameters.AddWithValue("$text", text);
            command.Parameters.AddWithValue("$renamed", renamed);
            command.ExecuteNonQuery();
        }

        LTagPositionNormalize(connection, table, column, renamed);
    }

    private static void LTagDelete(
        SqliteConnection connection, string table, string column, string text)
    {
        List<string> owners = [];
        using (SqliteCommand named = connection.CreateCommand())
        {
            named.CommandText = $"SELECT {column} FROM {table} WHERE text = $text;";
            named.Parameters.AddWithValue("$text", text);
            using SqliteDataReader reader = named.ExecuteReader();
            while (reader.Read())
            {
                owners.Add(reader.GetString(0));
            }
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE text = $text;";
            command.Parameters.AddWithValue("$text", text);
            command.ExecuteNonQuery();
        }

        foreach (string owner in owners)
        {
            LTagOwnerNormalize(connection, table, column, owner);
        }
    }

    private static void LTagPositionNormalize(
        SqliteConnection connection, string table, string column, string text)
    {
        List<string> owners = [];
        using (SqliteCommand named = connection.CreateCommand())
        {
            named.CommandText = $"SELECT {column} FROM {table} WHERE text = $text;";
            named.Parameters.AddWithValue("$text", text);
            using SqliteDataReader reader = named.ExecuteReader();
            while (reader.Read())
            {
                owners.Add(reader.GetString(0));
            }
        }

        foreach (string owner in owners)
        {
            LTagOwnerNormalize(connection, table, column, owner);
        }
    }

    private static void LTagOwnerNormalize(
        SqliteConnection connection, string table, string column, string owner)
    {
        List<string> texts = [];
        using (SqliteCommand named = connection.CreateCommand())
        {
            named.CommandText =
                $"SELECT text FROM {table} WHERE {column} = $owner ORDER BY position;";
            named.Parameters.AddWithValue("$owner", owner);
            using SqliteDataReader reader = named.ExecuteReader();
            while (reader.Read())
            {
                texts.Add(reader.GetString(0));
            }
        }

        using (SqliteCommand cleared = connection.CreateCommand())
        {
            cleared.CommandText = $"DELETE FROM {table} WHERE {column} = $owner;";
            cleared.Parameters.AddWithValue("$owner", owner);
            cleared.ExecuteNonQuery();
        }

        LTagOwnerSave(connection, table, column, owner, texts);
    }

    private static void LTagOwnerSave(
        SqliteConnection connection, string table, string column, string owner, IReadOnlyList<string> texts)
    {
        int position = 0;
        foreach (string text in texts)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                $"INSERT INTO {table} ({column}, text, position) VALUES ($owner, $text, $position);";
            command.Parameters.AddWithValue("$owner", owner);
            command.Parameters.AddWithValue("$text", text);
            command.Parameters.AddWithValue("$position", position);
            command.ExecuteNonQuery();
            position++;
        }
    }

    private void LTagReferrerSave(
        string table, string column, string referrerId, IReadOnlyList<LTag> tags)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentNullException.ThrowIfNull(tags);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand cleared = connection.CreateCommand())
        {
            cleared.CommandText = $"DELETE FROM {table} WHERE {column} = $referrer;";
            cleared.Parameters.AddWithValue("$referrer", referrerId);
            cleared.ExecuteNonQuery();
        }

        List<string> texts = [];
        HashSet<string> written = new(StringComparer.Ordinal);
        foreach (LTag tag in tags)
        {
            ArgumentNullException.ThrowIfNull(tag);
            string text = tag.LTagText.Trim();
            if (text.Length == 0 || !written.Add(text))
            {
                continue;
            }

            texts.Add(text);
        }

        LTagOwnerSave(connection, table, column, referrerId, texts);

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LTag> LTagReferrerRead(string table, string column, string referrerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);

        using LDatabaseSession session = _lTagArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"SELECT text FROM {table} WHERE {column} = $referrer ORDER BY position;";
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LTag> tags = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            tags.Add(new LTag(reader.GetString(0)));
        }

        return tags;
    }
}
