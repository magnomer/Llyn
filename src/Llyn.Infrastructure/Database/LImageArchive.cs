using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LImageArchive
{
    private readonly LDatabase _lImageArchiveDatabase;

    public LImageArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lImageArchiveDatabase = database;
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
                RETURNING id;
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
        return LImageReferrerRead("sense_image", "sense_id", meaningId);
    }

    public IReadOnlyList<LImage> LImageCollocationRead(long collocationId)
    {
        return LImageReferrerRead("collocation_image", "collocation_id", collocationId);
    }

    public void LImageUpdate(LImage image)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(image.LImageId);

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE image SET location_state = $locationState, location = $location WHERE id = $id;";
            LStateColumn.LStateColumnApply(command, "location", image.LImageLocation);
            command.Parameters.AddWithValue("$id", image.LImageId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Image carries the id '{image.LImageId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public int LImageReferenceRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        return LImageReferenceRead(session.LDatabaseSessionConnection, id);
    }

    private static int LImageReferenceRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM sense_image WHERE image_id = $id)
                + (SELECT COUNT(*) FROM collocation_image WHERE image_id = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void LImageDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        int references = LImageReferenceRead(connection, id);
        if (references > 0)
        {
            throw new InvalidOperationException(
                $"Image {id} is still referenced {references} time(s); detach every reference before deleting it.");
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM image WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LImageMeaningAttach(long meaningId, long imageId, int position)
    {
        LImageReferenceAttach("sense_image", "sense_id", meaningId, imageId, position);
    }

    public void LImageCollocationAttach(long collocationId, long imageId, int position)
    {
        LImageReferenceAttach("collocation_image", "collocation_id", collocationId, imageId, position);
    }

    public void LImageMeaningDetach(long meaningId, long imageId)
    {
        LImageReferenceDetach("sense_image", "sense_id", meaningId, imageId);
    }

    public void LImageCollocationDetach(long collocationId, long imageId)
    {
        LImageReferenceDetach("collocation_image", "collocation_id", collocationId, imageId);
    }

    private void LImageReferenceAttach(string table, string column, long referrerId, long imageId, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(imageId);

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{column} = $owner";
        IReadOnlyList<long> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, referrerId, "image_id");

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, image_id, position)
                VALUES ($referrer, $image, $position)
                ON CONFLICT ({column}, image_id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$image", imageId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "image_id",
            LDatabaseOrder.LDatabaseOrderInsert(current, imageId, position));

        session.LDatabaseSessionCommit();
    }

    private void LImageReferenceDetach(string table, string column, long referrerId, long imageId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(imageId);

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{column} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE {column} = $referrer AND image_id = $image;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$image", imageId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "image_id",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrerId, "image_id"));

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LImage> LImageReferrerRead(string table, string column, long referrerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);

        using LDatabaseSession session = _lImageArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT image.id, image.location_state, image.location
            FROM {table} link
            JOIN image ON image.id = link.image_id
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
        command.CommandText = "SELECT location_state, location FROM image WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LImage(id, LStateColumn.LStateColumnRead(reader, 0));
    }
}
