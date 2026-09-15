using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed partial class LSituationArchive
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
}
