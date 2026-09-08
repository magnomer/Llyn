using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LVideoArchive
{
    private readonly LDatabase _lVideoArchiveDatabase;

    public LVideoArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lVideoArchiveDatabase = database;
    }

    public LVideo LVideoCreate(LVideo video)
    {
        ArgumentNullException.ThrowIfNull(video);

        LVideo stored = video with { LVideoId = LIdentity.LIdentityCreate() };

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO video (id, location_state, location, span_state, span)
                VALUES ($id, $locationState, $location, $spanState, $span);
                """;
            command.Parameters.AddWithValue("$id", stored.LVideoId);
            LStateColumn.LStateColumnApply(command, "location", stored.LVideoLocation);
            LStateColumn.LStateColumnApply(command, "span", stored.LVideoSpan);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LVideo? LVideoRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        return LVideoSingleRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyList<LVideo> LVideoMeaningRead(string meaningId)
    {
        return LVideoReferrerRead("sense_video", "sense_id", meaningId);
    }

    public IReadOnlyList<LVideo> LVideoCollocationRead(string collocationId)
    {
        return LVideoReferrerRead("collocation_video", "collocation_id", collocationId);
    }

    public void LVideoUpdate(LVideo video)
    {
        ArgumentNullException.ThrowIfNull(video);
        ArgumentException.ThrowIfNullOrWhiteSpace(video.LVideoId);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE video
                SET location_state = $locationState, location = $location,
                    span_state = $spanState, span = $span
                WHERE id = $id;
                """;
            LStateColumn.LStateColumnApply(command, "location", video.LVideoLocation);
            LStateColumn.LStateColumnApply(command, "span", video.LVideoSpan);
            command.Parameters.AddWithValue("$id", video.LVideoId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Video carries the id '{video.LVideoId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public int LVideoReferenceRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        return LVideoReferenceRead(session.LDatabaseSessionConnection, id);
    }

    private static int LVideoReferenceRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM sense_video WHERE video_id = $id)
                + (SELECT COUNT(*) FROM collocation_video WHERE video_id = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void LVideoDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        int references = LVideoReferenceRead(connection, id);
        if (references > 0)
        {
            throw new InvalidOperationException(
                $"Video {id} is still referenced {references} time(s); detach every reference before deleting it.");
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM video WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LVideoMeaningAttach(string meaningId, string videoId, int position)
    {
        LVideoReferenceAttach("sense_video", "sense_id", meaningId, videoId, position);
    }

    public void LVideoCollocationAttach(string collocationId, string videoId, int position)
    {
        LVideoReferenceAttach("collocation_video", "collocation_id", collocationId, videoId, position);
    }

    public void LVideoMeaningDetach(string meaningId, string videoId)
    {
        LVideoReferenceDetach("sense_video", "sense_id", meaningId, videoId);
    }

    public void LVideoCollocationDetach(string collocationId, string videoId)
    {
        LVideoReferenceDetach("collocation_video", "collocation_id", collocationId, videoId);
    }

    private void LVideoReferenceAttach(string table, string column, string referrerId, string videoId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(videoId);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{column} = $owner";
        IReadOnlyList<string> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, referrerId, "video_id");

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, video_id, position)
                VALUES ($referrer, $video, $position)
                ON CONFLICT ({column}, video_id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$video", videoId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "video_id",
            LDatabaseOrder.LDatabaseOrderInsert(current, videoId, position));

        session.LDatabaseSessionCommit();
    }

    private void LVideoReferenceDetach(string table, string column, string referrerId, string videoId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(videoId);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{column} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE {column} = $referrer AND video_id = $video;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$video", videoId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "video_id",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrerId, "video_id"));

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LVideo> LVideoReferrerRead(string table, string column, string referrerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT video.id, video.location_state, video.location, video.span_state, video.span
            FROM {table} link
            JOIN video ON video.id = link.video_id
            WHERE link.{column} = $referrer
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LVideo> videos = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            videos.Add(new LVideo(
                reader.GetString(0),
                LStateColumn.LStateColumnRead(reader, 1),
                LStateColumn.LStateColumnRead(reader, 3)));
        }

        return videos;
    }

    private static LVideo? LVideoSingleRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT location_state, location, span_state, span FROM video WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LVideo(
            id, LStateColumn.LStateColumnRead(reader, 0), LStateColumn.LStateColumnRead(reader, 2));
    }
}
