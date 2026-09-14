using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LSituationArchive
{
    private readonly LDatabase _lSituationArchiveDatabase;

    public LSituationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSituationArchiveDatabase = database;
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

    public void LSituationTitleUpdate(long situationId, LStateValue title)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(situationId);
        ArgumentNullException.ThrowIfNull(title);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE situation SET title_state = $titleState, title = $title WHERE situation_id = $id;";
            LStateColumn.LStateColumnApply(command, "title", title);
            command.Parameters.AddWithValue("$id", situationId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Situation carries the id '{situationId}'.");
            }
        }

        session.LDatabaseSessionCommit();
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
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LUsage> usages = [];
        usages.AddRange(LSituationUsageRead(
            connection,
            id,
            LOwner.LOwnerMeaning,
            """
            SELECT link.sense_parent, sense.entry_parent, entry.headword, entry.language,
                   sense.title_state, sense.title,
                   sense.definition_state, sense.definition
            FROM sense_situation link
            JOIN sense ON sense.sense_id = link.sense_parent
            JOIN entry ON entry.entry_id = sense.entry_parent
            WHERE link.situation_ref = $id
            ORDER BY entry.headword, sense.position;
            """));
        usages.AddRange(LSituationUsageRead(
            connection,
            id,
            LOwner.LOwnerCollocation,
            """
            SELECT link.collocation_parent, collocation.entry_parent, entry.headword, entry.language,
                   collocation.title_state, collocation.title,
                   collocation.expression_state, collocation.expression
            FROM collocation_situation link
            JOIN collocation ON collocation.collocation_id = link.collocation_parent
            JOIN entry ON entry.entry_id = collocation.entry_parent
            WHERE link.situation_ref = $id
            ORDER BY entry.headword, collocation.position;
            """));

        return usages;
    }

    private static IReadOnlyList<LUsage> LSituationUsageRead(
        SqliteConnection connection,
        long id,
        LOwner owner,
        string statement)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = statement;
        command.Parameters.AddWithValue("$id", id);

        List<LUsage> usages = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            LStateValue title = LStateColumn.LStateColumnRead(reader, 4);
            if (title.LStateValueEmpty)
            {
                title = LStateColumn.LStateColumnRead(reader, 6);
            }

            usages.Add(new LUsage(
                reader.GetInt64(0),
                owner,
                reader.GetInt64(1),
                reader.GetString(2),
                reader.GetString(3),
                title));
        }

        return usages;
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

    public void LSituationDelete(long id)
    {
        LSituationDelete(id, false);
    }

    public void LSituationDelete(long id, bool detach)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        if (detach)
        {
            LSituationLinkDelete(connection, "sense_situation", id);
            LSituationLinkDelete(connection, "collocation_situation", id);
        }

        int references = LSituationReferenceRead(connection, id);
        if (references > 0)
        {
            throw new InvalidOperationException(
                $"Situation {id} is still referenced {references} time(s); detach every reference before deleting it.");
        }

        LSituationMediaDelete(connection, "situation_image", id);
        LSituationMediaDelete(connection, "situation_video", id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM situation WHERE situation_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    private static void LSituationMediaDelete(SqliteConnection connection, string table, long situationId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {table} WHERE situation_parent = $situation;";
        command.Parameters.AddWithValue("$situation", situationId);
        command.ExecuteNonQuery();
    }

    private static void LSituationLinkDelete(SqliteConnection connection, string table, long situationId)
    {
        string column = table == "sense_situation" ? "sense_parent" : "collocation_parent";

        List<long> referrers = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"SELECT {column} FROM {table} WHERE situation_ref = $situation;";
            command.Parameters.AddWithValue("$situation", situationId);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                referrers.Add(reader.GetInt64(0));
            }
        }

        if (referrers.Count == 0)
        {
            return;
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE situation_ref = $situation;";
            command.Parameters.AddWithValue("$situation", situationId);
            command.ExecuteNonQuery();
        }

        string scope = $"{column} = $owner";
        foreach (long referrer in referrers)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, table, scope, referrer, "situation_ref",
                LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrer, "situation_ref"));
        }
    }

    public void LSituationMeaningAttach(long meaningId, long situationId, int position)
    {
        LSituationReferenceAttach("sense_situation", "sense_parent", meaningId, situationId, position);
    }

    public void LSituationCollocationAttach(long collocationId, long situationId, int position)
    {
        LSituationReferenceAttach("collocation_situation", "collocation_parent", collocationId, situationId, position);
    }

    public void LSituationMeaningDetach(long meaningId, long situationId)
    {
        LSituationReferenceDetach("sense_situation", "sense_parent", meaningId, situationId);
    }

    public void LSituationCollocationDetach(long collocationId, long situationId)
    {
        LSituationReferenceDetach("collocation_situation", "collocation_parent", collocationId, situationId);
    }

    private void LSituationReferenceAttach(
        string table,
        string column,
        long referrerId,
        long situationId,
        int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(situationId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{column} = $owner";
        IReadOnlyList<long> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, referrerId, "situation_ref");

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, situation_ref, position)
                VALUES ($referrer, $situation, $position)
                ON CONFLICT ({column}, situation_ref) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$situation", situationId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "situation_ref",
            LDatabaseOrder.LDatabaseOrderInsert(current, situationId, position));

        session.LDatabaseSessionCommit();
    }

    private void LSituationReferenceDetach(string table, string column, long referrerId, long situationId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(situationId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{column} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"DELETE FROM {table} WHERE {column} = $referrer AND situation_ref = $situation;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$situation", situationId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "situation_ref",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrerId, "situation_ref"));

        session.LDatabaseSessionCommit();
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
