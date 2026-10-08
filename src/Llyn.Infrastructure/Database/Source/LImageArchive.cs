using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LImageArchive : LImageVault
{
    private readonly LDatabase _lImageArchiveDatabase;
    private readonly LDatabaseLink _lImageMeaningLink;
    private readonly LDatabaseLink _lImageCollocationLink;
    private readonly LDatabaseLink _lImageSituationLink;

    public LImageArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lImageArchiveDatabase = database;
        _lImageMeaningLink = new LDatabaseLink(database, "sense_image", "sense_parent", "image_ref");
        _lImageCollocationLink = new LDatabaseLink(database, "collocation_image", "collocation_parent", "image_ref");
        _lImageSituationLink = new LDatabaseLink(database, "situation_image", "situation_parent", "image_ref");
    }

    public LImage LImageCreate(LImage image)
    {
        ArgumentNullException.ThrowIfNull(image);

        LImage stored = image;

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO image (location_state, location)
                VALUES ($locationState, $location)
                RETURNING image_id;
                """;
            LStateColumn.LStateColumnApply(command, "location", stored.LImageLocation);
            stored = stored with { LImageId = (long)command.ExecuteScalar()! };
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LImage? LImageRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        return LImageSingleRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyList<LImage> LImageMeaningRead(long meaningId)
    {
        return LImageReferrerRead("sense_image", "sense_parent", meaningId);
    }

    public IReadOnlyList<LImage> LImageCollocationRead(long collocationId)
    {
        return LImageReferrerRead("collocation_image", "collocation_parent", collocationId);
    }

    public IReadOnlyList<LImage> LImageSituationRead(long situationId)
    {
        return LImageReferrerRead("situation_image", "situation_parent", situationId);
    }

    public void LImageUpdate(LImage image)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(image.LImageId);

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE image SET location_state = $locationState, location = $location WHERE image_id = $id;";
            LStateColumn.LStateColumnApply(command, "location", image.LImageLocation);
            command.Parameters.AddWithValue("$id", image.LImageId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Image carries the id '{image.LImageId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LImageMeaningAttach(long meaningId, long imageId, int position)
    {
        _lImageMeaningLink.LDatabaseLinkAttach(meaningId, imageId, position);
    }

    public void LImageCollocationAttach(long collocationId, long imageId, int position)
    {
        _lImageCollocationLink.LDatabaseLinkAttach(collocationId, imageId, position);
    }

    public void LImageSituationAttach(long situationId, long imageId, int position)
    {
        _lImageSituationLink.LDatabaseLinkAttach(situationId, imageId, position);
    }

    public void LImageMeaningDetach(long meaningId, long imageId)
    {
        _lImageMeaningLink.LDatabaseLinkDetach(meaningId, imageId);
    }

    public void LImageCollocationDetach(long collocationId, long imageId)
    {
        _lImageCollocationLink.LDatabaseLinkDetach(collocationId, imageId);
    }

    public void LImageSituationDetach(long situationId, long imageId)
    {
        _lImageSituationLink.LDatabaseLinkDetach(situationId, imageId);
    }

    public Dictionary<long, List<LImageDraft>> LImageSituationScan()
    {
        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
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

    public void LImageSituationClear(long situationId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(situationId);

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "DELETE FROM situation_image WHERE situation_parent = $situation;";
            command.Parameters.AddWithValue("$situation", situationId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LImage> LImageReferrerRead(string table, string column, long referrerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT image.image_id, image.location_state, image.location
            FROM {table} link
            JOIN image ON image.image_id = link.image_ref
            WHERE link.{column} = $referrer
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LImage> images = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            images.Add(new LImage(reader.GetInt64(0), LStateColumn.LStateColumnRead(reader, 1)));
        }

        return images;
    }

    private static LImage? LImageSingleRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT location_state, location FROM image WHERE image_id = $id;";
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LImage(id, LStateColumn.LStateColumnRead(reader, 0));
    }
}
