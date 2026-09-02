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

        LSituation stored = situation with
        {
            LSituationId = string.IsNullOrWhiteSpace(situation.LSituationId)
                ? LIdentity.LIdentityCreate()
                : situation.LSituationId,
        };

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO situation (
                    id, title_state, title, description_state, description,
                    kind_state, kind, source_state, source_id)
                VALUES (
                    $id, $titleState, $title, $descriptionState, $description,
                    $kindState, $kind, $sourceState, $source);
                """;
            command.Parameters.AddWithValue("$id", stored.LSituationId);
            LStateColumn.LStateColumnApply(command, "title", stored.LSituationTitle);
            LStateColumn.LStateColumnApply(command, "description", stored.LSituationDescription);
            LStateColumn.LStateColumnApply(command, "kind", stored.LSituationKind);
            LStateColumn.LStateColumnApply(command, "source", stored.LSituationSource);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LSituation? LSituationRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        return LSituationSingleRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyList<LSituation> LSituationSenseRead(string senseId)
    {
        return LSituationReferrerRead("sense_situation", "sense_id", senseId);
    }

    public IReadOnlyList<LSituation> LSituationCollocationRead(string collocationId)
    {
        return LSituationReferrerRead("collocation_situation", "collocation_id", collocationId);
    }

    public void LSituationUpdate(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);
        ArgumentException.ThrowIfNullOrWhiteSpace(situation.LSituationId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE situation
                SET title_state = $titleState, title = $title,
                    description_state = $descriptionState, description = $description,
                    kind_state = $kindState, kind = $kind,
                    source_state = $sourceState, source_id = $source
                WHERE id = $id;
                """;
            LStateColumn.LStateColumnApply(command, "title", situation.LSituationTitle);
            LStateColumn.LStateColumnApply(command, "description", situation.LSituationDescription);
            LStateColumn.LStateColumnApply(command, "kind", situation.LSituationKind);
            LStateColumn.LStateColumnApply(command, "source", situation.LSituationSource);
            command.Parameters.AddWithValue("$id", situation.LSituationId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Situation carries the id '{situation.LSituationId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LSituationTitleUpdate(string situationId, LStateValue title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(situationId);
        ArgumentNullException.ThrowIfNull(title);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE situation SET title_state = $titleState, title = $title WHERE id = $id;";
            LStateColumn.LStateColumnApply(command, "title", title);
            command.Parameters.AddWithValue("$id", situationId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Situation carries the id '{situationId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LSituationSourceUpdate(string situationId, LStateValue source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(situationId);
        ArgumentNullException.ThrowIfNull(source);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE situation SET source_state = $sourceState, source_id = $source WHERE id = $id;";
            LStateColumn.LStateColumnApply(command, "source", source);
            command.Parameters.AddWithValue("$id", situationId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Situation carries the id '{situationId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public int LSituationReferenceRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        return LSituationReferenceRead(session.LDatabaseSessionConnection, id);
    }

    private static int LSituationReferenceRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM sense_situation WHERE situation_id = $id)
                + (SELECT COUNT(*) FROM collocation_situation WHERE situation_id = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void LSituationDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        int references = LSituationReferenceRead(connection, id);
        if (references > 0)
        {
            throw new InvalidOperationException(
                $"Situation {id} is still referenced {references} time(s); detach every reference before deleting it.");
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM situation WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LSituationSenseAttach(string senseId, string situationId, int position)
    {
        LSituationReferenceAttach("sense_situation", "sense_id", senseId, situationId, position);
    }

    public void LSituationCollocationAttach(string collocationId, string situationId, int position)
    {
        LSituationReferenceAttach("collocation_situation", "collocation_id", collocationId, situationId, position);
    }

    public void LSituationSenseDetach(string senseId, string situationId)
    {
        LSituationReferenceDetach("sense_situation", "sense_id", senseId, situationId);
    }

    public void LSituationCollocationDetach(string collocationId, string situationId)
    {
        LSituationReferenceDetach("collocation_situation", "collocation_id", collocationId, situationId);
    }

    private void LSituationReferenceAttach(
        string table,
        string column,
        string referrerId,
        string situationId,
        int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(situationId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{column} = $owner";
        IReadOnlyList<string> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, referrerId, "situation_id");

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, situation_id, position)
                VALUES ($referrer, $situation, $position)
                ON CONFLICT ({column}, situation_id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$situation", situationId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "situation_id",
            LDatabaseOrder.LDatabaseOrderInsert(current, situationId, position));

        session.LDatabaseSessionCommit();
    }

    private void LSituationReferenceDetach(string table, string column, string referrerId, string situationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(situationId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{column} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"DELETE FROM {table} WHERE {column} = $referrer AND situation_id = $situation;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$situation", situationId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "situation_id",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrerId, "situation_id"));

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LSituation> LSituationReferrerRead(string table, string column, string referrerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);

        using LDatabaseSession session = _lSituationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT situation.id, situation.title_state, situation.title,
                   situation.description_state, situation.description,
                   situation.kind_state, situation.kind,
                   situation.source_state, situation.source_id
            FROM {table} link
            JOIN situation ON situation.id = link.situation_id
            WHERE link.{column} = $referrer
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LSituation> situations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            situations.Add(new LSituation(
                reader.GetString(0),
                LStateColumn.LStateColumnRead(reader, 1),
                LStateColumn.LStateColumnRead(reader, 3),
                LStateColumn.LStateColumnRead(reader, 5),
                LStateColumn.LStateColumnRead(reader, 7)));
        }

        return situations;
    }

    private static LSituation? LSituationSingleRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT title_state, title, description_state, description,
                   kind_state, kind, source_state, source_id
            FROM situation WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LSituation(
            id,
            LStateColumn.LStateColumnRead(reader, 0),
            LStateColumn.LStateColumnRead(reader, 2),
            LStateColumn.LStateColumnRead(reader, 4),
            LStateColumn.LStateColumnRead(reader, 6));
    }
}
