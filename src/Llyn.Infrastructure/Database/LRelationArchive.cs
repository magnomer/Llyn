using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LRelationArchive
{
    private readonly LDatabase _lRelationArchiveDatabase;

    public LRelationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lRelationArchiveDatabase = database;
    }

    public LRelation LRelationCreate(LRelation relation)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.LRelationSenseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.LRelationType);
        LRelationTargetValidate(relation);

        using LDatabaseSession session = _lRelationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LRelation stored = relation with
        {
            LRelationId = LIdentity.LIdentityCreate(),
            LRelationPosition = LRelationSiblingRead(connection, relation.LRelationSenseId).Count,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO relation (id, sense_id, position, relation_type, label, labels)
                VALUES ($id, $sense, $position, $type, $label, $labels);
                """;
            command.Parameters.AddWithValue("$id", stored.LRelationId);
            command.Parameters.AddWithValue("$sense", stored.LRelationSenseId);
            command.Parameters.AddWithValue("$position", stored.LRelationPosition);
            command.Parameters.AddWithValue("$type", stored.LRelationType);
            command.Parameters.AddWithValue("$label", (object?)stored.LRelationLabel ?? DBNull.Value);
            command.Parameters.AddWithValue("$labels", (object?)stored.LRelationLabels ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        LRelationTargetInsert(connection, stored);

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LRelation> LRelationRead(string senseId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(senseId);

        using LDatabaseSession session = _lRelationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT r.id, r.sense_id, r.position, r.relation_type, r.label, r.labels,
                   re.entry_id, rs.sense_id
            FROM relation r
            LEFT JOIN relation_entry re ON re.relation_id = r.id
            LEFT JOIN relation_sense rs ON rs.relation_id = r.id
            WHERE r.sense_id = $sense
            ORDER BY r.position;
            """;
        command.Parameters.AddWithValue("$sense", senseId);

        List<LRelation> relations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            relations.Add(new LRelation(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.IsDBNull(6) ? null : reader.GetString(6),
                reader.IsDBNull(7) ? null : reader.GetString(7)));
        }

        return relations;
    }

    public void LRelationUpdate(LRelation relation)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.LRelationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.LRelationType);

        using LDatabaseSession session = _lRelationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE relation
                SET relation_type = $type, label = $label, labels = $labels
                WHERE id = $id;
                """;
            command.Parameters.AddWithValue("$type", relation.LRelationType);
            command.Parameters.AddWithValue("$label", (object?)relation.LRelationLabel ?? DBNull.Value);
            command.Parameters.AddWithValue("$labels", (object?)relation.LRelationLabels ?? DBNull.Value);
            command.Parameters.AddWithValue("$id", relation.LRelationId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No relation carries the id '{relation.LRelationId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LRelationMove(string id, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lRelationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string? senseId = LRelationHolderRead(connection, id);
        if (senseId is null)
        {
            return;
        }

        IReadOnlyList<string> order = LDatabaseOrder.LDatabaseOrderInsert(
            LRelationSiblingRead(connection, senseId), id, position);
        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "relation", "sense_id = $owner", senseId, "id", order);

        session.LDatabaseSessionCommit();
    }

    public void LRelationDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lRelationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string? senseId = LRelationHolderRead(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM relation WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        if (senseId is not null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "relation", "sense_id = $owner", senseId,
                "id", LRelationSiblingRead(connection, senseId));
        }

        session.LDatabaseSessionCommit();
    }

    private static string? LRelationHolderRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT sense_id FROM relation WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteScalar() as string;
    }

    private static IReadOnlyList<string> LRelationSiblingRead(SqliteConnection connection, string senseId)
    {
        return LDatabaseOrder.LDatabaseOrderRead(connection, "relation", "sense_id = $owner", senseId, "id");
    }

    private static void LRelationTargetValidate(LRelation relation)
    {
        bool hasEntry = !string.IsNullOrWhiteSpace(relation.LRelationTargetEntry);
        bool hasSense = !string.IsNullOrWhiteSpace(relation.LRelationTargetSense);
        if (hasEntry == hasSense)
        {
            throw new InvalidOperationException(
                "A relation must have exactly one target: a target Entry id XOR a target Meaning id.");
        }
    }

    private static void LRelationTargetInsert(SqliteConnection connection, LRelation relation)
    {
        using SqliteCommand command = connection.CreateCommand();
        if (!string.IsNullOrWhiteSpace(relation.LRelationTargetEntry))
        {
            command.CommandText =
                "INSERT INTO relation_entry (relation_id, entry_id) VALUES ($relation, $entry);";
            command.Parameters.AddWithValue("$relation", relation.LRelationId);
            command.Parameters.AddWithValue("$entry", relation.LRelationTargetEntry);
        }
        else
        {
            command.CommandText =
                "INSERT INTO relation_sense (relation_id, sense_id) VALUES ($relation, $sense);";
            command.Parameters.AddWithValue("$relation", relation.LRelationId);
            command.Parameters.AddWithValue("$sense", relation.LRelationTargetSense);
        }

        command.ExecuteNonQuery();
    }
}
