using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed partial class LSituationArchive
{
    private static void LSituationMediaDelete(SqliteConnection connection, string table, long situationId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {table} WHERE situation_parent = $situation;";
        command.Parameters.AddWithValue("$situation", situationId);
        command.ExecuteNonQuery();
    }

    private IReadOnlyList<LSituation> LSituationMediaRead(List<LSituation> situations)
    {
        if (situations.Count == 0)
        {
            return situations;
        }

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        Dictionary<long, List<LImageDraft>> images = LSituationImageRead(connection);
        Dictionary<long, List<LVideoDraft>> videos = LSituationVideoRead(connection);

        for (int index = 0; index < situations.Count; index++)
        {
            LSituation situation = situations[index];
            situations[index] = situation with
            {
                LSituationImage = images.TryGetValue(situation.LSituationId, out List<LImageDraft>? imageRows)
                    ? imageRows
                    : [],
                LSituationVideo = videos.TryGetValue(situation.LSituationId, out List<LVideoDraft>? videoRows)
                    ? videoRows
                    : [],
            };
        }

        return situations;
    }

    private LSituation LSituationMediaRead(LSituation situation)
    {
        LImageArchive images = new(_lSituationArchiveDatabase);
        LVideoArchive videos = new(_lSituationArchiveDatabase);

        List<LImageDraft> imageRows = [];
        foreach (LImage image in images.LImageSituationRead(situation.LSituationId))
        {
            imageRows.Add(new LImageDraft(image.LImageLocation, image.LImageId));
        }

        List<LVideoDraft> videoRows = [];
        foreach (LVideo video in videos.LVideoSituationRead(situation.LSituationId))
        {
            videoRows.Add(new LVideoDraft(video.LVideoLocation, video.LVideoSpan, video.LVideoId));
        }

        return situation with { LSituationImage = imageRows, LSituationVideo = videoRows };
    }

    private static Dictionary<long, List<LImageDraft>> LSituationImageRead(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT link.situation_parent, image.image_id, image.location_state, image.location
            FROM situation_image link
            JOIN image ON image.image_id = link.image_ref
            ORDER BY link.situation_parent, link.position;
            """;

        Dictionary<long, List<LImageDraft>> grouped = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            long parent = reader.GetInt64(0);
            if (!grouped.TryGetValue(parent, out List<LImageDraft>? rows))
            {
                rows = [];
                grouped.Add(parent, rows);
            }

            rows.Add(new LImageDraft(LStateColumn.LStateColumnRead(reader, 2), reader.GetInt64(1)));
        }

        return grouped;
    }

    private static Dictionary<long, List<LVideoDraft>> LSituationVideoRead(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT link.situation_parent, video.video_id,
                   video.location_state, video.location, video.span_state, video.span
            FROM situation_video link
            JOIN video ON video.video_id = link.video_ref
            ORDER BY link.situation_parent, link.position;
            """;

        Dictionary<long, List<LVideoDraft>> grouped = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            long parent = reader.GetInt64(0);
            if (!grouped.TryGetValue(parent, out List<LVideoDraft>? rows))
            {
                rows = [];
                grouped.Add(parent, rows);
            }

            rows.Add(new LVideoDraft(
                LStateColumn.LStateColumnRead(reader, 2),
                LStateColumn.LStateColumnRead(reader, 4),
                reader.GetInt64(1)));
        }

        return grouped;
    }
}
