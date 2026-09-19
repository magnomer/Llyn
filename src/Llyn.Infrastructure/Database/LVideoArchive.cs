using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LVideoArchive : LVideoVault
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

        LVideo stored = video;

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO video (location_state, location, span_state, span)
                VALUES ($locationState, $location, $spanState, $span)
                RETURNING video_id;
                """;
            LStateColumn.LStateColumnApply(command, "location", stored.LVideoLocation);
            LStateColumn.LStateColumnApply(command, "span", stored.LVideoSpan);
            stored = stored with { LVideoId = (long)command.ExecuteScalar()! };
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LVideo? LVideoRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        return LVideoSingleRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyList<LVideo> LVideoMeaningRead(long meaningId)
    {
        return LVideoReferrerRead("sense_video", "sense_parent", meaningId);
    }

    public IReadOnlyList<LVideo> LVideoCollocationRead(long collocationId)
    {
        return LVideoReferrerRead("collocation_video", "collocation_parent", collocationId);
    }

    public IReadOnlyList<LVideo> LVideoSituationRead(long situationId)
    {
        return LVideoReferrerRead("situation_video", "situation_parent", situationId);
    }

    public void LVideoUpdate(LVideo video)
    {
        ArgumentNullException.ThrowIfNull(video);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(video.LVideoId);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE video
                SET location_state = $locationState, location = $location,
                    span_state = $spanState, span = $span
                WHERE video_id = $id;
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

    public int LVideoReferenceRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        return LVideoReferenceRead(session.LDatabaseSessionConnection, id);
    }

    private static int LVideoReferenceRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM sense_video WHERE video_ref = $id)
                + (SELECT COUNT(*) FROM collocation_video WHERE video_ref = $id)
                + (SELECT COUNT(*) FROM situation_video WHERE video_ref = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void LVideoDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

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
            command.CommandText = "DELETE FROM video WHERE video_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LVideoMeaningAttach(long meaningId, long videoId, int position)
    {
        LVideoReferenceAttach("sense_video", "sense_parent", meaningId, videoId, position);
    }

    public void LVideoCollocationAttach(long collocationId, long videoId, int position)
    {
        LVideoReferenceAttach("collocation_video", "collocation_parent", collocationId, videoId, position);
    }

    public void LVideoSituationAttach(long situationId, long videoId, int position)
    {
        LVideoReferenceAttach("situation_video", "situation_parent", situationId, videoId, position);
    }

    public void LVideoMeaningDetach(long meaningId, long videoId)
    {
        LVideoReferenceDetach("sense_video", "sense_parent", meaningId, videoId);
    }

    public void LVideoCollocationDetach(long collocationId, long videoId)
    {
        LVideoReferenceDetach("collocation_video", "collocation_parent", collocationId, videoId);
    }

    public void LVideoSituationDetach(long situationId, long videoId)
    {
        LVideoReferenceDetach("situation_video", "situation_parent", situationId, videoId);
    }

    private void LVideoReferenceAttach(string table, string column, long referrerId, long videoId, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(videoId);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{column} = $owner";
        IReadOnlyList<long> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, referrerId, "video_ref");

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, video_ref, position)
                VALUES ($referrer, $video, $position)
                ON CONFLICT ({column}, video_ref) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$video", videoId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "video_ref",
            LDatabaseOrder.LDatabaseOrderInsert(current, videoId, position));

        session.LDatabaseSessionCommit();
    }

    private void LVideoReferenceDetach(string table, string column, long referrerId, long videoId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(videoId);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{column} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE {column} = $referrer AND video_ref = $video;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$video", videoId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "video_ref",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrerId, "video_ref"));

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LVideo> LVideoReferrerRead(string table, string column, long referrerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);

        using LDatabaseSession session = _lVideoArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT video.video_id, video.location_state, video.location, video.span_state, video.span
            FROM {table} link
            JOIN video ON video.video_id = link.video_ref
            WHERE link.{column} = $referrer
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LVideo> videos = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            videos.Add(new LVideo(
                reader.GetInt64(0),
                LStateColumn.LStateColumnRead(reader, 1),
                LStateColumn.LStateColumnRead(reader, 3)));
        }

        return videos;
    }

    private static LVideo? LVideoSingleRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT location_state, location, span_state, span FROM video WHERE video_id = $id;";
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
