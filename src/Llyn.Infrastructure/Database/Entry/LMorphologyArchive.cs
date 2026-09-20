using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LMorphologyArchive : LMorphologyVault
{
    private readonly LDatabase _lMorphologyArchiveDatabase;

    public LMorphologyArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lMorphologyArchiveDatabase = database;
    }

    public LFeature LFeatureCreate(LFeature feature)
    {
        ArgumentNullException.ThrowIfNull(feature);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(feature.LFeatureSpeechId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(feature.LFeatureCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(feature.LFeatureName);

        using LDatabaseSession session = _lMorphologyArchiveDatabase.LDatabaseSessionStart();
        long id;
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO morphology_feature (speech_value_parent, pack_code, name, position)
                VALUES ($speech, $pack, $name, $position)
                ON CONFLICT (speech_value_parent, pack_code)
                DO UPDATE SET name = excluded.name, position = excluded.position
                RETURNING morphology_feature_id;
                """;
            command.Parameters.AddWithValue("$speech", feature.LFeatureSpeechId);
            command.Parameters.AddWithValue("$pack", feature.LFeatureCode);
            command.Parameters.AddWithValue("$name", feature.LFeatureName);
            command.Parameters.AddWithValue("$position", feature.LFeaturePosition);
            id = Convert.ToInt64(command.ExecuteScalar());
        }

        session.LDatabaseSessionCommit();
        return feature with { LFeatureId = id };
    }

    public LMorphology LMorphologyCreate(LMorphology value)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value.LMorphologyFeatureId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value.LMorphologyCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(value.LMorphologyName);

        using LDatabaseSession session = _lMorphologyArchiveDatabase.LDatabaseSessionStart();
        long id;
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO morphology_value (morphology_feature_parent, pack_code, name, position)
                VALUES ($feature, $pack, $name, $position)
                ON CONFLICT (morphology_feature_parent, pack_code)
                DO UPDATE SET name = excluded.name, position = excluded.position
                RETURNING morphology_value_id;
                """;
            command.Parameters.AddWithValue("$feature", value.LMorphologyFeatureId);
            command.Parameters.AddWithValue("$pack", value.LMorphologyCode);
            command.Parameters.AddWithValue("$name", value.LMorphologyName);
            command.Parameters.AddWithValue("$position", value.LMorphologyPosition);
            id = Convert.ToInt64(command.ExecuteScalar());
        }

        session.LDatabaseSessionCommit();
        return value with { LMorphologyId = id };
    }

    public IReadOnlyList<LFeature> LFeatureRead(long speechValueId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(speechValueId);

        using LDatabaseSession session = _lMorphologyArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT morphology_feature_id, speech_value_parent, pack_code, name, position FROM morphology_feature
            WHERE speech_value_parent = $speech ORDER BY position, morphology_feature_id;
            """;
        command.Parameters.AddWithValue("$speech", speechValueId);

        List<LFeature> features = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            features.Add(LFeatureRowRead(reader));
        }

        return features;
    }

    public LFeature? LFeatureFind(long speechValueId, string name)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(speechValueId);
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        using LDatabaseSession session = _lMorphologyArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT morphology_feature_id, speech_value_parent, pack_code, name, position FROM morphology_feature
            WHERE speech_value_parent = $speech AND trim(name) = trim($name) COLLATE NOCASE
            ORDER BY position, morphology_feature_id LIMIT 1;
            """;
        command.Parameters.AddWithValue("$speech", speechValueId);
        command.Parameters.AddWithValue("$name", name);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LFeatureRowRead(reader) : null;
    }

    public LMorphology? LMorphologyRead(long id)
    {
        if (id <= 0)
        {
            return null;
        }

        using LDatabaseSession session = _lMorphologyArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT morphology_value_id, morphology_feature_parent, pack_code, name, position
            FROM morphology_value WHERE morphology_value_id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LMorphologyRowRead(reader) : null;
    }

    public IReadOnlyList<LMorphology> LMorphologyScan(long featureId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(featureId);

        using LDatabaseSession session = _lMorphologyArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT morphology_value_id, morphology_feature_parent, pack_code, name, position FROM morphology_value
            WHERE morphology_feature_parent = $feature ORDER BY position, morphology_value_id;
            """;
        command.Parameters.AddWithValue("$feature", featureId);

        List<LMorphology> values = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            values.Add(LMorphologyRowRead(reader));
        }

        return values;
    }

    public LMorphology? LMorphologyFind(long featureId, string name)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(featureId);
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        using LDatabaseSession session = _lMorphologyArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT morphology_value_id, morphology_feature_parent, pack_code, name, position FROM morphology_value
            WHERE morphology_feature_parent = $feature AND trim(name) = trim($name) COLLATE NOCASE
            ORDER BY position, morphology_value_id LIMIT 1;
            """;
        command.Parameters.AddWithValue("$feature", featureId);
        command.Parameters.AddWithValue("$name", name);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LMorphologyRowRead(reader) : null;
    }

    public LMorphology? LMorphologyCodeFind(string language, long speechCode, long featureCode, long code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        if (speechCode <= 0 || featureCode <= 0 || code <= 0)
        {
            return null;
        }

        using LDatabaseSession session = _lMorphologyArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT v.morphology_value_id, v.morphology_feature_parent, v.pack_code, v.name, v.position
            FROM morphology_value v
            JOIN morphology_feature f ON f.morphology_feature_id = v.morphology_feature_parent
            JOIN speech_value s ON s.speech_value_id = f.speech_value_parent
            WHERE s.language = $language AND s.pack_code = $speech AND f.pack_code = $feature AND v.pack_code = $code
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$speech", speechCode);
        command.Parameters.AddWithValue("$feature", featureCode);
        command.Parameters.AddWithValue("$code", code);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LMorphologyRowRead(reader) : null;
    }

    private static LFeature LFeatureRowRead(SqliteDataReader reader)
    {
        return new LFeature(
            reader.GetInt64(0),
            reader.GetInt64(1),
            reader.GetInt64(2),
            reader.GetString(3),
            reader.GetInt32(4));
    }

    private static LMorphology LMorphologyRowRead(SqliteDataReader reader)
    {
        return new LMorphology(
            reader.GetInt64(0),
            reader.GetInt64(1),
            reader.GetInt64(2),
            reader.GetString(3),
            reader.GetInt32(4));
    }
}
