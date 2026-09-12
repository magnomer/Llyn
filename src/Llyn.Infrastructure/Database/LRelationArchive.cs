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
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(relation.LRelationMeaningId);
        ArgumentException.ThrowIfNullOrWhiteSpace(relation.LRelationType);
        LRelationTargetValidate(relation);

        using LDatabaseSession session = _lRelationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LRelation stored = relation with
        {
            LRelationPosition = LRelationSiblingRead(connection, relation.LRelationMeaningId).Count,
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO relation (sense_id, position, relation_type, label, labels)
                VALUES ($sense, $position, $type, $label, $labels)
                RETURNING id;
                """;
            command.Parameters.AddWithValue("$sense", stored.LRelationMeaningId);
            command.Parameters.AddWithValue("$position", stored.LRelationPosition);
            command.Parameters.AddWithValue("$type", stored.LRelationType);
            command.Parameters.AddWithValue("$label", (object?)stored.LRelationLabel ?? DBNull.Value);
            command.Parameters.AddWithValue("$labels", (object?)stored.LRelationLabels ?? DBNull.Value);
            stored = stored with { LRelationId = (long)command.ExecuteScalar()! };
        }

        LRelationTargetInsert(connection, stored);

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LRelation> LRelationRead(long meaningId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(meaningId);

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
        command.Parameters.AddWithValue("$sense", meaningId);

        List<LRelation> relations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            relations.Add(new LRelation(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                LStateAnchor.LStateAnchorRead(reader.IsDBNull(6) ? null : reader.GetInt64(6)),
                LStateAnchor.LStateAnchorRead(reader.IsDBNull(7) ? null : reader.GetInt64(7))));
        }

        return relations;
    }

    public void LRelationUpdate(LRelation relation)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(relation.LRelationId);
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

    public void LRelationMove(long id, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lRelationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        long? meaningId = LRelationHolderRead(connection, id);
        if (meaningId is null)
        {
            return;
        }

        IReadOnlyList<long> order = LDatabaseOrder.LDatabaseOrderInsert(
            LRelationSiblingRead(connection, meaningId), id, position);
        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, "relation", "sense_id = $owner", meaningId, "id", order);

        session.LDatabaseSessionCommit();
    }

    public void LRelationDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lRelationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        long? meaningId = LRelationHolderRead(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM relation WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        if (meaningId is not null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "relation", "sense_id = $owner", meaningId,
                "id", LRelationSiblingRead(connection, meaningId));
        }

        session.LDatabaseSessionCommit();
    }

    private static long? LRelationHolderRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT sense_id FROM relation WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteScalar() as long?;
    }

    private static IReadOnlyList<long> LRelationSiblingRead(SqliteConnection connection, long? meaningId)
    {
        return LDatabaseOrder.LDatabaseOrderRead(connection, "relation", "sense_id = $owner", meaningId, "id");
    }

    private static void LRelationTargetValidate(LRelation relation)
    {
        bool hasEntry = !relation.LRelationTargetEntry.LStateAnchorEmpty;
        bool hasMeaning = !relation.LRelationTargetMeaning.LStateAnchorEmpty;
        if (hasEntry == hasMeaning)
        {
            throw new InvalidOperationException(
                "A relation must have exactly one target: a target Entry id XOR a target Meaning id.");
        }
    }

    private static void LRelationTargetInsert(SqliteConnection connection, LRelation relation)
    {
        using SqliteCommand command = connection.CreateCommand();
        if (!relation.LRelationTargetEntry.LStateAnchorEmpty)
        {
            command.CommandText =
                "INSERT INTO relation_entry (relation_id, entry_id) VALUES ($relation, $entry);";
            command.Parameters.AddWithValue("$relation", relation.LRelationId);
            command.Parameters.AddWithValue("$entry", relation.LRelationTargetEntry.LStateAnchorShow());
        }
        else
        {
            command.CommandText =
                "INSERT INTO relation_sense (relation_id, sense_id) VALUES ($relation, $sense);";
            command.Parameters.AddWithValue("$relation", relation.LRelationId);
            command.Parameters.AddWithValue("$sense", relation.LRelationTargetMeaning.LStateAnchorShow());
        }

        command.ExecuteNonQuery();
    }
}
