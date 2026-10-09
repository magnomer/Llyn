using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LSituationArchive : LSituationVault
{
    private readonly LDatabase _lSituationArchiveDatabase;
    private readonly LDatabaseLink _lSituationMeaningLink;
    private readonly LDatabaseLink _lSituationCollocationLink;

    public LSituationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSituationArchiveDatabase = database;
        _lSituationMeaningLink = new LDatabaseLink(database, "sense_situation", "sense_parent", "situation_ref");
        _lSituationCollocationLink = new LDatabaseLink(
            database, "collocation_situation", "collocation_parent", "situation_ref");
    }

    public LSituation LSituationCreate(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);

        LSituation stored = situation;

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO situation (
                    title_state, title, description_state, description,
                    kind_state, kind)
                VALUES (
                    $titleState, $title, $descriptionState, $description,
                    $kindState, $kind)
                RETURNING situation_id;
                """;
            LStateColumn.LStateColumnApply(command, "title", stored.LSituationTitle);
            LStateColumn.LStateColumnApply(command, "description", stored.LSituationDescription);
            LStateColumn.LStateColumnApply(command, "kind", stored.LSituationKind);
            stored = stored with { LSituationId = (long)command.ExecuteScalar()! };
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LSituation? LSituationRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        return LSituationSingleRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyList<LSituation> LSituationRead()
    {
        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT situation_id, title_state, title, description_state, description,
                   kind_state, kind
            FROM situation
            ORDER BY rowid;
            """;

        List<LSituation> situations = [];
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                situations.Add(new LSituation(
                    reader.GetInt64(0),
                    LStateColumn.LStateColumnRead(reader, 1),
                    LStateColumn.LStateColumnRead(reader, 3),
                    LStateColumn.LStateColumnRead(reader, 5)));
            }
        }

        return LSituationMediaRead(situations);
    }

    public IReadOnlyList<LSituation> LSituationMeaningRead(long meaningId)
    {
        return LSituationReferrerRead("sense_situation", "sense_parent", meaningId);
    }

    public IReadOnlyList<LSituation> LSituationCollocationRead(long collocationId)
    {
        return LSituationReferrerRead("collocation_situation", "collocation_parent", collocationId);
    }

    public void LSituationUpdate(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(situation.LSituationId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE situation
                SET title_state = $titleState, title = $title,
                    description_state = $descriptionState, description = $description,
                    kind_state = $kindState, kind = $kind
                WHERE situation_id = $id;
                """;
            LStateColumn.LStateColumnApply(command, "title", situation.LSituationTitle);
            LStateColumn.LStateColumnApply(command, "description", situation.LSituationDescription);
            LStateColumn.LStateColumnApply(command, "kind", situation.LSituationKind);
            command.Parameters.AddWithValue("$id", situation.LSituationId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Situation carries the id '{situation.LSituationId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LSituationMeaningAttach(long meaningId, long situationId, int position)
    {
        _lSituationMeaningLink.LDatabaseLinkAttach(meaningId, situationId, position);
    }

    public void LSituationCollocationAttach(long collocationId, long situationId, int position)
    {
        _lSituationCollocationLink.LDatabaseLinkAttach(collocationId, situationId, position);
    }

    public void LSituationMeaningDetach(long meaningId, long situationId)
    {
        _lSituationMeaningLink.LDatabaseLinkDetach(meaningId, situationId);
    }

    public void LSituationCollocationDetach(long collocationId, long situationId)
    {
        _lSituationCollocationLink.LDatabaseLinkDetach(collocationId, situationId);
    }

    public int LSituationReferenceRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        return LSituationReferenceRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyDictionary<long, int> LSituationReferenceRead()
    {
        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT situation_ref, COUNT(*) FROM (
                SELECT situation_ref FROM sense_situation
                UNION ALL
                SELECT situation_ref FROM collocation_situation
            )
            GROUP BY situation_ref;
            """;

        Dictionary<long, int> counts = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            counts[reader.GetInt64(0)] = reader.GetInt32(1);
        }

        return counts;
    }

    public IReadOnlyList<LUsage> LSituationUsageRead(long id)
    {
        return new LSituationUsage(_lSituationArchiveDatabase).LSituationUsageRead(id);
    }

    private static int LSituationReferenceRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM sense_situation WHERE situation_ref = $id)
                + (SELECT COUNT(*) FROM collocation_situation WHERE situation_ref = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void LSituationDelete(long id, bool detach)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        if (detach)
        {
            _lSituationMeaningLink.LDatabaseLinkDelete(id);
            _lSituationCollocationLink.LDatabaseLinkDelete(id);
        }

        int references = LSituationReferenceRead(connection, id);
        if (references > 0)
        {
            throw new InvalidOperationException(
                $"Situation {id} is still referenced {references} time(s); detach every reference before deleting it.");
        }

        new LImageArchive(_lSituationArchiveDatabase).LImageSituationClear(id);
        new LVideoArchive(_lSituationArchiveDatabase).LVideoSituationClear(id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM situation WHERE situation_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    private LSituation? LSituationSingleRead(SqliteConnection connection, long id)
    {
        LSituation read;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                SELECT title_state, title, description_state, description,
                       kind_state, kind
                FROM situation WHERE situation_id = $id;
                """;
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            read = new LSituation(
                id,
                LStateColumn.LStateColumnRead(reader, 0),
                LStateColumn.LStateColumnRead(reader, 2),
                LStateColumn.LStateColumnRead(reader, 4));
        }

        return LSituationMediaRead(read);
    }

    private IReadOnlyList<LSituation> LSituationReferrerRead(string table, string column, long referrerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT situation.situation_id, situation.title_state, situation.title,
                   situation.description_state, situation.description,
                   situation.kind_state, situation.kind
            FROM {table} link
            JOIN situation ON situation.situation_id = link.situation_ref
            WHERE link.{column} = $referrer
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LSituation> situations = [];
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                situations.Add(new LSituation(
                    reader.GetInt64(0),
                    LStateColumn.LStateColumnRead(reader, 1),
                    LStateColumn.LStateColumnRead(reader, 3),
                    LStateColumn.LStateColumnRead(reader, 5)));
            }
        }

        return LSituationMediaRead(situations);
    }

    private IReadOnlyList<LSituation> LSituationMediaRead(List<LSituation> situations)
    {
        if (situations.Count == 0)
        {
            return situations;
        }

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();

        Dictionary<long, List<LImageDraft>> images =
            new LImageArchive(_lSituationArchiveDatabase).LImageSituationScan();
        Dictionary<long, List<LVideoDraft>> videos =
            new LVideoArchive(_lSituationArchiveDatabase).LVideoSituationScan();

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
}
