using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed partial class LSituationArchive
{
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
}
